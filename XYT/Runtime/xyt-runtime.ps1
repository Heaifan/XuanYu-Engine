[CmdletBinding()]
param(
    [ValidateSet('P3-01','P3-02','P3-03','P3-04')][string]$Capability='P3-01',
    [string]$AppCommand='', [string[]]$AppArgument=@(), [string]$AppScript='', [int]$TimeoutSeconds=30,
    [string]$EvidenceRoot='', [switch]$FakeHarnessFault, [switch]$ResizePulse
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$schema=Join-Path $PSScriptRoot 'xyt-runtime.schema.json'
if(!$EvidenceRoot){$EvidenceRoot=Join-Path $root '.xyt\runtime'}
New-Item -ItemType Directory -Force -Path $EvidenceRoot | Out-Null
$started=[DateTimeOffset]::UtcNow; $id="$Capability-$(Get-Date -Format yyyyMMddHHmmss)"
$log=Join-Path $EvidenceRoot "$id.log"; $json=Join-Path $EvidenceRoot "$id.json"
$appExePath='UNKNOWN'; $appBuildTimestamp='UNKNOWN'; $buildLog=Join-Path $EvidenceRoot "$id-build.log"
function Read-Version { try{ $v=& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'scripts\resolve-version.ps1'); if($v){([string]$v).Trim()}else{'UNKNOWN'} }catch{'UNKNOWN'} }
function Markers([string]$cap){
    $base=@('【VulkanSurface】创建 Vulkan Surface 成功','逻辑设备 创建成功','Swapchain 创建成功','首帧 呈现 成功')
    switch($cap){
        'P3-01' { $base }
        'P3-02' { $base+@('【VulkanSwapchain】Swapchain 重建成功') }
        'P3-03' { $base+@('DEM','Terrain','Camera') }
        'P3-04' { $base+@('管理模式','区域编辑','PointerCapture','Toolbar','DEM') }
    }
}
function Stop-Tree([Diagnostics.Process]$process){
    if($process -and !$process.HasExited){ try{$process.Kill($true)}catch{}; $process.WaitForExit(2000)|Out-Null }
}
function Ensure-CanonicalApp {
    $bootstrap=& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'scripts\xye-bootstrap.ps1') 2>&1
    if($LASTEXITCODE -ne 0){throw 'CANONICAL_BOOTSTRAP_FAILED'}
    $project=Join-Path $root 'XuanYu.Editor.App\XuanYu.Editor.App.csproj'
    & (Join-Path $root 'scripts\xye-dotnet.ps1') restore $project '--configfile' (Join-Path $root 'NuGet.Config') '-nologo' *> $buildLog
    if($LASTEXITCODE -ne 0){throw 'CANONICAL_RESTORE_FAILED'}
    & (Join-Path $root 'scripts\xye-dotnet.ps1') build $project '--no-restore' '-t:Rebuild' '-nologo' '-m:1' '-nr:false' '-p:UseSharedCompilation=false' *> $buildLog
    if($LASTEXITCODE -ne 0){throw 'CANONICAL_BUILD_FAILED'}
    $script:appExePath=Join-Path $root 'XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe'
    if(!(Test-Path -LiteralPath $appExePath -PathType Leaf)){throw 'CANONICAL_APP_EXE_MISSING'}
    $script:appBuildTimestamp=(Get-Item -LiteralPath $appExePath).LastWriteTime.ToString('o')
    $script:AppCommand=$appExePath; $script:AppArgument=@()
}
function Start-Target {
    if(!$AppCommand){
        Ensure-CanonicalApp
    }
    $targetArgs=if($AppScript){@('-NoProfile','-NonInteractive','-Command',$AppScript)}else{$AppArgument}
    $psi=[Diagnostics.ProcessStartInfo]::new(); $psi.FileName=$AppCommand; $psi.WorkingDirectory=$root
    $psi.UseShellExecute=$false; $psi.RedirectStandardOutput=$true; $psi.RedirectStandardError=$false
    $psi.Arguments=($targetArgs | % { '"'+($_ -replace '"','\"')+'"' }) -join ' '
    $p=[Diagnostics.Process]::new(); $p.StartInfo=$psi; if(!$p.Start()){throw 'HARNESS_START_FAILED'}
    $script:RuntimeRead=$p.StandardOutput.ReadToEndAsync(); $p
}
function Invoke-Resize([Diagnostics.Process]$process) {
    Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class XytNative {
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr h, IntPtr i, int x, int y, int w, int he, uint f);
}
'@
    for($i=0;$i -lt 20;$i++){ $process.Refresh(); if($process.MainWindowHandle -ne 0){ [XytNative]::SetWindowPos($process.MainWindowHandle,[IntPtr]::Zero,0,0,1100,700,0x0004); return $true }; Start-Sleep -Milliseconds 250 }; $false
}
$result='UNCLASSIFIED'; $harnessFailure=$false; $blocked=''; $out=''; $exit=$null; $p=$null
try {
    $p=Start-Target
    if($ResizePulse -or $Capability -eq 'P3-02'){ Start-Sleep -Seconds 2; if(!(Invoke-Resize $p)){$blocked='Window host handle unavailable'} }
    $deadline=[DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    while(!$p.HasExited -and [DateTime]::UtcNow -lt $deadline){Start-Sleep -Milliseconds 100}
    if(!$p.HasExited){$result='TIMEOUT'; Stop-Tree $p} else {$exit=$p.ExitCode}
    Start-Sleep -Milliseconds 100; $out=$RuntimeRead.GetAwaiter().GetResult(); $out | Set-Content -LiteralPath $log -Encoding UTF8
    if($FakeHarnessFault){throw 'HARNESS_EVIDENCE_WRITE_FAILURE'}
    if($result -eq 'UNCLASSIFIED' -and $exit -ne 0){$result='FAIL'}
    if($result -eq 'FAIL' -and $out -match '找不到指定的文件|not found|No such file'){$result='BLOCKED_BY';$blocked='Runtime App executable unavailable'}
    if($result -eq 'FAIL' -and $exit -eq 1 -and $AppArgument -contains '--no-build' -and $AppArgument -match 'XuanYu.Editor.App.csproj'){
        $exe=Join-Path $root 'XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe'
        if(!(Test-Path -LiteralPath $exe)){$result='BLOCKED_BY';$blocked='Runtime App executable unavailable'}
    }
    $missing=@(Markers $Capability | ? { $out -notmatch [regex]::Escape($_) })
    if($blocked){$result='BLOCKED_BY'} elseif($result -eq 'UNCLASSIFIED' -and $missing.Count){$result='BLOCKED_BY';$blocked='Missing required real-runtime evidence marker'} elseif($result -eq 'UNCLASSIFIED'){$result='PASS'}
} catch { $harnessFailure=$true; $result='UNCLASSIFIED'; $blocked=$_.Exception.Message }
finally { if($p -and !$p.HasExited){Stop-Tree $p}; if($p){$p.Dispose()} }
$ended=[DateTimeOffset]::UtcNow
$gpu=$env:XYT_GPU; if(!$gpu){$gpu='UNKNOWN'}
$adapter=if($out -match '已选择物理设备：([^；\r\n]+)'){$Matches[1]}else{'UNKNOWN'}
$windowHost=if($out -match '窗口句柄：(0x[0-9A-F]+)'){$Matches[1]}else{'UNKNOWN'}
$commit='UNKNOWN'; $branch='UNKNOWN'; $status=''
if(!$env:XYT_SKIP_GIT){try{$commit=((& git -C $root rev-parse HEAD 2>$null)|Out-String).Trim();$branch=((& git -C $root branch --show-current 2>$null)|Out-String).Trim();$status=((& git -C $root status --short 2>$null)|Out-String).Trim()}catch{}}
$dirty=if(!$commit -or $commit -eq 'UNKNOWN'){'UNKNOWN'}elseif($status){'YES'}else{'NO'}
$device=($out -match [regex]::Escape('逻辑设备 创建成功'))
$swapchain=($out -match [regex]::Escape('Swapchain 创建成功'))
$present=($out -match [regex]::Escape('首帧 呈现 成功'))
$gpu=$env:XYT_GPU; if(!$gpu -and $adapter -ne 'UNKNOWN'){$gpu=$adapter}; if(!$gpu){$gpu='UNKNOWN'}
$fatal=($out -match 'DeviceLost|Unhandled exception|Fatal|崩溃')
$missing=@(Markers $Capability | ? { $out -notmatch [regex]::Escape($_) })
if($result -eq 'TIMEOUT' -and $Capability -eq 'P3-01' -and !$fatal -and !$missing.Count){$result='PASS';$blocked=''}
$record=[ordered]@{Schema='XYT-P3-R1/2';Version=(Read-Version);Commit=$commit;Branch=$branch;DirtyState=$dirty;AppExePath=$appExePath;AppBuildTimestamp=$appBuildTimestamp;GPU=$gpu;VulkanAdapter=$adapter;WindowHost=$windowHost;VulkanDevice=$device;Swapchain=$swapchain;PresentEvidence=$present;RuntimeTestId=$id;Capability=$Capability;StartTime=$started.ToString('o');DurationMs=[int](($ended-$started).TotalMilliseconds);Result=$result;EvidencePath=$log;BuildEvidencePath=$buildLog;HarnessFailure=$harnessFailure;BlockedBy=$blocked;ExitCode=$exit;RequiredMarkers=@(Markers $Capability);MissingMarkers=$missing}
$record|ConvertTo-Json -Depth 8|Set-Content -LiteralPath $json -Encoding UTF8
Write-Output ($record|ConvertTo-Json -Depth 8)
if($harnessFailure){exit 3}; if($result -in @('FAIL','TIMEOUT')){exit 1}; if($result -eq 'BLOCKED_BY'){exit 2}; exit 0
