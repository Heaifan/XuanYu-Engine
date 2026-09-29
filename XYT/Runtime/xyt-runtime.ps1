[CmdletBinding()]
param(
    [ValidateSet('P3-01','P3-02','P3-03','P3-04')][string]$Capability='P3-01',
    [string]$CandidateId='', [string]$AppCommand='', [string[]]$AppArgument=@(), [string]$AppScript='',
    [ValidateRange(1,3600)][int]$TimeoutSeconds=30,
    [ValidateRange(1,3600)][int]$StdoutDrainTimeoutSeconds=5,
    [ValidateRange(1,3600)][int]$StderrDrainTimeoutSeconds=5,
    [ValidateRange(1,3600)][int]$EvidenceWriteTimeoutSeconds=5,
    [string]$EvidenceRoot='', [switch]$FakeHarnessFault, [switch]$FakeStdoutDrainTimeout, [switch]$FakeStderrDrainTimeout, [switch]$NoGui, [switch]$ResizePulse
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if(!$EvidenceRoot){$EvidenceRoot=Join-Path $root '.xyt\runtime'}
New-Item -ItemType Directory -Force -Path $EvidenceRoot | Out-Null
$started=[DateTimeOffset]::UtcNow; $id="$Capability-$(Get-Date -Format yyyyMMddHHmmssfff)"
if(!$CandidateId){$CandidateId=$env:XYT_CANDIDATE_ID}; if(!$CandidateId){$CandidateId="CAND-$id"}
$json=Join-Path $EvidenceRoot "$id.json"; $startJson=Join-Path $EvidenceRoot "$id.start.json"; $stdoutLog=Join-Path $EvidenceRoot "$id.log"; $stderrLog=Join-Path $EvidenceRoot "$id.stderr.log"; $buildLog=Join-Path $EvidenceRoot "$id-build.log"
$appExePath='UNKNOWN'; $appBuildTimestamp='UNKNOWN'; $p=$null; $stdoutTask=$null; $stderrTask=$null; $out=''; $err=''; $exit=$null
$status='HARNESS_FAILURE'; $reason=''; $harnessStatus='STARTING'; $evidenceFailure=$false
function Read-Version { try{ $v=& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'scripts\resolve-version.ps1'); if($v){([string]$v).Trim()}else{'UNKNOWN'} }catch{'UNKNOWN'} }
function Markers([string]$cap){$base=@('【VulkanSurface】创建 Vulkan Surface 成功','逻辑设备 创建成功','Swapchain 创建成功','首帧 呈现 成功');switch($cap){'P3-01'{$base};'P3-02'{$base+@('【VulkanSwapchain】Swapchain 重建成功')};'P3-03'{$base+@('DEM','Terrain','Camera')};'P3-04'{$base+@('管理模式','区域编辑','PointerCapture','Toolbar','DEM')}}}
function Stop-Tree([Diagnostics.Process]$process){if($process -and !$process.HasExited){try{$process.Kill($true)}catch{};try{$process.WaitForExit(2000)|Out-Null}catch{}}}
function Write-Bounded([string]$Path,[string]$Text,[int]$Seconds){$job=Start-Job -ScriptBlock {param($p,$t)[IO.File]::WriteAllText($p,$t,(New-Object Text.UTF8Encoding($false)))} -ArgumentList $Path,$Text;$done=Wait-Job -Job $job -Timeout ($Seconds);if(!$done){Stop-Job -Job $job -Force -ErrorAction SilentlyContinue;Remove-Job -Job $job -Force -ErrorAction SilentlyContinue;return $false};Receive-Job -Job $job -ErrorAction Stop|Out-Null;Remove-Job -Job $job -Force -ErrorAction SilentlyContinue;return $true}
function Read-Bounded([Threading.Tasks.Task[string]]$Task,[int]$Seconds){if(!$Task.Wait($Seconds*1000)){return [pscustomobject]@{Completed=$false;Text=''}};try{return [pscustomobject]@{Completed=$true;Text=[string]$Task.Result}}catch{return [pscustomobject]@{Completed=$true;Text=''}}}
function Ensure-CanonicalApp {
    $bootstrap=& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'scripts\xye-bootstrap.ps1') 2>&1;if($LASTEXITCODE -ne 0){throw 'CANONICAL_BOOTSTRAP_FAILED'}
    $project=Join-Path $root 'XuanYu.Editor.App\XuanYu.Editor.App.csproj';& (Join-Path $root 'scripts\xye-dotnet.ps1') restore $project '--configfile' (Join-Path $root 'NuGet.Config') '-nologo' *> $buildLog;if($LASTEXITCODE -ne 0){throw 'CANONICAL_RESTORE_FAILED'}
    & (Join-Path $root 'scripts\xye-dotnet.ps1') build $project '--no-restore' '-t:Rebuild' '-nologo' '-m:1' '-nr:false' '-p:UseSharedCompilation=false' *> $buildLog;if($LASTEXITCODE -ne 0){throw 'CANONICAL_BUILD_FAILED'}
    $script:appExePath=Join-Path $root 'XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe';if(!(Test-Path -LiteralPath $appExePath -PathType Leaf)){throw 'CANONICAL_APP_EXE_MISSING'};$script:appBuildTimestamp=(Get-Item -LiteralPath $appExePath).LastWriteTime.ToString('o');$script:AppCommand=$appExePath;$script:AppArgument=@()
}
function Start-Target {
    if(!$AppCommand){Ensure-CanonicalApp};$targetArgs=if($AppScript){@('-NoProfile','-NonInteractive','-Command',$AppScript)}else{$AppArgument}
    $psi=[Diagnostics.ProcessStartInfo]::new();$psi.FileName=$AppCommand;$psi.WorkingDirectory=$root;$psi.UseShellExecute=$false;$psi.RedirectStandardOutput=$true;$psi.RedirectStandardError=$true;$psi.Arguments=($targetArgs|%{'"'+($_ -replace '"','\"')+'"'}) -join ' '
    $process=[Diagnostics.Process]::new();$process.StartInfo=$psi;if(!$process.Start()){throw 'HARNESS_START_FAILED'};$script:stdoutTask=$process.StandardOutput.ReadToEndAsync();$script:stderrTask=$process.StandardError.ReadToEndAsync();$process
}
function Invoke-Resize([Diagnostics.Process]$process) {
    Add-Type @'
using System; using System.Runtime.InteropServices; public static class XytNative { [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h,IntPtr i,int x,int y,int w,int he,uint f); }
'@;for($i=0;$i -lt 20;$i++){$process.Refresh();if($process.MainWindowHandle -ne 0){[XytNative]::SetWindowPos($process.MainWindowHandle,[IntPtr]::Zero,0,0,1100,700,0x0004);return $true};Start-Sleep -Milliseconds 250};$false
}
function New-Record([string]$phase) {
    $ended=[DateTimeOffset]::UtcNow;$missing=@(Markers $Capability|?{$out -notmatch [regex]::Escape($_)});$gpu=$env:XYT_GPU;if(!$gpu){$gpu='UNKNOWN'}
    [ordered]@{Schema='XYT-P3-R1/3';RecordPhase=$phase;Version=(Read-Version);Commit='UNKNOWN';Branch='UNKNOWN';DirtyState='UNKNOWN';AppExePath=$appExePath;AppBuildTimestamp=$appBuildTimestamp;GPU=$gpu;VulkanAdapter=($(if($out -match '已选择物理设备：([^；\r\n]+)'){$Matches[1]}else{'UNKNOWN'}));WindowHost=($(if($out -match '窗口句柄：(0x[0-9A-F]+)'){$Matches[1]}else{'UNKNOWN'}));VulkanDevice=($out -match '逻辑设备 创建成功');Swapchain=($out -match 'Swapchain 创建成功');PresentEvidence=($out -match '首帧 呈现 成功');CandidateId=$CandidateId;RuntimeTestId=$id;Capability=$Capability;StartTime=$started.ToString('o');EndTime=$(if($phase -eq 'START'){''}else{$ended.ToString('o')});DurationMs=$(if($phase -eq 'START'){''}else{[int](($ended-$started).TotalMilliseconds)});Status=$status;Result=$status;Reason=$reason;HarnessStatus=$harnessStatus;EvidencePath=$stdoutLog;StderrEvidencePath=$stderrLog;BuildEvidencePath=$buildLog;HarnessFailure=($status -in @('HARNESS_FAILURE','EVIDENCE_FAILURE'));BlockedBy=$reason;ExitCode=$exit;RequiredMarkers=@(Markers $Capability);MissingMarkers=$missing}
}
function Save-Record([object]$Record,[switch]$InjectFailure){$text=$Record|ConvertTo-Json -Depth 8;if(!$InjectFailure -and (Write-Bounded $json $text $EvidenceWriteTimeoutSeconds)){return $true};$fallback=Join-Path $EvidenceRoot "$id.failure.json";return (Write-Bounded $fallback $text $EvidenceWriteTimeoutSeconds)}
$status='HARNESS_FAILURE';$reason='';$harnessStatus='START_RECORD_PENDING';$start=New-Record 'START';$startText=$start|ConvertTo-Json -Depth 8;$startWritten=(Write-Bounded $json $startText $EvidenceWriteTimeoutSeconds) -and (Write-Bounded $startJson $startText $EvidenceWriteTimeoutSeconds)
if(!$startWritten){$status='EVIDENCE_FAILURE';$reason='START_RECORD_WRITE_TIMEOUT';$harnessStatus='EVIDENCE_WRITE_TIMEOUT'}
try {
    if(!$startWritten){throw 'START_RECORD_WRITE_TIMEOUT'};if($NoGui){$status='BLOCKED_NO_GUI';$reason='NO_GUI_WITNESS_ENVIRONMENT';$harnessStatus='BLOCKED_NO_GUI';throw $reason};$p=Start-Target
    if($ResizePulse -or $Capability -eq 'P3-02'){Start-Sleep -Seconds 2;if(!(Invoke-Resize $p)){$reason='WINDOW_HOST_HANDLE_UNAVAILABLE'}}
    $deadline=[DateTime]::UtcNow.AddSeconds($TimeoutSeconds);while(!$p.HasExited -and [DateTime]::UtcNow -lt $deadline){Start-Sleep -Milliseconds 50}
    if(!$p.HasExited){$status='TIMEOUT';$reason='TARGET_PROCESS_TIMEOUT';$harnessStatus='TARGET_TIMEOUT';Stop-Tree $p}else{$exit=$p.ExitCode;$harnessStatus='TARGET_EXITED'}
    $stdout=if($FakeStdoutDrainTimeout){[pscustomobject]@{Completed=$false;Text=''}}else{Read-Bounded $stdoutTask $StdoutDrainTimeoutSeconds};$stderr=if($FakeStderrDrainTimeout){[pscustomobject]@{Completed=$false;Text=''}}else{Read-Bounded $stderrTask $StderrDrainTimeoutSeconds}
    if($stdout.Completed){$out=$stdout.Text}else{$status='TIMEOUT';$reason='STDOUT_DRAIN_TIMEOUT';$harnessStatus='STDOUT_DRAIN_TIMEOUT'};if($stderr.Completed){$err=$stderr.Text}else{$status='TIMEOUT';$reason='STDERR_DRAIN_TIMEOUT';$harnessStatus='STDERR_DRAIN_TIMEOUT'}
    if($stdout.Completed -and !(Write-Bounded $stdoutLog $out $EvidenceWriteTimeoutSeconds)){$status='EVIDENCE_FAILURE';$reason='STDOUT_EVIDENCE_WRITE_TIMEOUT';$harnessStatus='EVIDENCE_WRITE_TIMEOUT'};if($stderr.Completed -and !(Write-Bounded $stderrLog $err $EvidenceWriteTimeoutSeconds)){$status='EVIDENCE_FAILURE';$reason='STDERR_EVIDENCE_WRITE_TIMEOUT';$harnessStatus='EVIDENCE_WRITE_TIMEOUT'}
    if($status -eq 'TIMEOUT'){}elseif($status -eq 'EVIDENCE_FAILURE'){throw $reason}elseif($exit -ne 0){$status='CRASH';$reason=if($err.Trim()){$err.Trim()}else{"TARGET_EXIT_CODE_$exit"};$harnessStatus='TARGET_CRASH'}else{$missing=@(Markers $Capability|?{$out -notmatch [regex]::Escape($_)});if($missing.Count){$status='FAIL';$reason='MISSING_MARKERS: '+($missing -join ', ')}else{$status='PASS';$reason='REQUIRED_MARKERS_PRESENT'}}
}catch{if(!$reason){$reason=$_.Exception.Message};if($reason -eq 'NO_GUI_WITNESS_ENVIRONMENT'){$status='BLOCKED_NO_GUI'}elseif($status -eq 'HARNESS_FAILURE'){$status='HARNESS_FAILURE'}}finally{if($p -and !$p.HasExited){Stop-Tree $p};if($p){$p.Dispose()}}
$harnessStatus=if($harnessStatus -eq 'START_RECORD_PENDING'){'HARNESS_FAILURE'}else{$harnessStatus};$record=New-Record 'END';if($FakeHarnessFault){$status='EVIDENCE_FAILURE';$reason='EVIDENCE_WRITE_INJECTED_FAILURE';$harnessStatus='EVIDENCE_WRITE_FAILURE';$record=New-Record 'END'};$saved=Save-Record $record -InjectFailure:$FakeHarnessFault
if(!$saved){$status='EVIDENCE_FAILURE';$reason='FINAL_EVIDENCE_WRITE_TIMEOUT';$harnessStatus='EVIDENCE_WRITE_TIMEOUT';$record=New-Record 'FAILURE';[void](Save-Record $record)}
Write-Output ($record|ConvertTo-Json -Depth 8);if($status -in @('HARNESS_FAILURE','EVIDENCE_FAILURE')){exit 3};if($status -in @('FAIL','CRASH','TIMEOUT')){exit 1};if($status -eq 'BLOCKED_NO_GUI'){exit 2};exit 0
