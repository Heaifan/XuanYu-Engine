[CmdletBinding()]
param(
    [ValidateSet('P3-01','P3-02','P3-03','P3-04')][string]$Capability='P3-01',
    [string]$CandidateId='', [string]$AppCommand='', [string[]]$AppArgument=@(), [string]$AppScript='',
    [ValidateRange(1,3600)][int]$TimeoutSeconds=30,
    [ValidateRange(1,3600)][int]$ResizeTimeoutSeconds=20,
    [ValidateRange(1,3600)][int]$CloseTimeoutSeconds=15,
    [ValidateRange(1,3600)][int]$StdoutDrainTimeoutSeconds=5,
    [ValidateRange(1,3600)][int]$StderrDrainTimeoutSeconds=5,
    [ValidateRange(1,3600)][int]$EvidenceWriteTimeoutSeconds=5,
    [string]$EvidenceRoot='', [switch]$FakeHarnessFault, [switch]$FakeStdoutDrainTimeout,
    [switch]$FakeStderrDrainTimeout, [switch]$NoGui, [switch]$ResizePulse
)
$ErrorActionPreference='Stop'
$root=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
if(!$EvidenceRoot){
    $EvidenceRoot=Join-Path $root 'artifacts\diagnostics\xyt-runtime'
}
New-Item -ItemType Directory -Force -Path $EvidenceRoot | Out-Null
$started=[DateTimeOffset]::UtcNow; $id="$Capability-$(Get-Date -Format yyyyMMddHHmmssfff)"
if(!$CandidateId){$CandidateId=$env:XYT_CANDIDATE_ID}; if(!$CandidateId){$CandidateId="CAND-$id"}
$json=Join-Path $EvidenceRoot "$id.json"; $startJson=Join-Path $EvidenceRoot "$id.start.json"
$stdoutLog=Join-Path $EvidenceRoot "$id.log"; $stderrLog=Join-Path $EvidenceRoot "$id.stderr.log"
$buildLog=Join-Path $EvidenceRoot "$id-build.log"
$appExePath='UNKNOWN'; $appBuildTimestamp='UNKNOWN'; $runtimeEventDirectory=''; $runtimeEventPath=''
$process=$null; $stdoutLines=[Collections.Generic.List[string]]::new(); $stderrLines=[Collections.Generic.List[string]]::new()
$stdoutTask=$null; $stderrTask=$null; $stdoutEnded=$false; $stderrEnded=$false
$status='HARNESS_FAILURE'; $reason=''; $harnessStatus='STARTING'; $exit=$null
$closeRequested=$false; $closeAccepted=$false; $exitedAfterClose=$false; $forcedTermination=$false
$resizeRequested=$false; $initialPresent=$false; $resizeEvidence=$false; $swapchainRecreated=$false; $postResizePresent=$false
$resizeRequestedUtc=$null
$resizeEventBaselineCount=-1
$surfaceEvidence=$false; $deviceEvidence=$false; $swapchainEvidence=$false; $presentLoopStopped=$false; $resourceReleaseEvidence=$false
$invalidEventIdentity=$false; $windowHandle='UNKNOWN'; $adapter='UNKNOWN'; $eventState=$null

function Read-Version {
    try {
        $value=& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'scripts\resolve-version.ps1')
        if($value){([string]$value).Trim()}else{'UNKNOWN'}
    } catch {'UNKNOWN'}
}

function Ensure-CanonicalApp {
    $bootstrap=& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root 'scripts\xye-bootstrap.ps1') 2>&1
    if($LASTEXITCODE -ne 0){throw 'CANONICAL_BOOTSTRAP_FAILED'}
    $script:AppCommand=Join-Path $root 'XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe'
    if(!(Test-Path -LiteralPath $script:AppCommand -PathType Leaf)){throw 'CANONICAL_APP_EXE_MISSING; run scripts\xye-build.ps1 once before XYT.'}
    $script:AppArgument=@(); $script:AppScript=''
}

function Initialize-RendererEventEnvironment {
    $appPath=[IO.Path]::GetFullPath($AppCommand)
    $appDirectory=Split-Path -Parent $appPath
    $script:runtimeEventDirectory=Join-Path (Join-Path $EvidenceRoot $CandidateId) 'RendererEvents'
    if($script:runtimeEventDirectory){New-Item -ItemType Directory -Force -Path $script:runtimeEventDirectory | Out-Null}
}

function Start-Target {
    if(!$AppCommand){Ensure-CanonicalApp}
    $script:appExePath=[IO.Path]::GetFullPath($AppCommand)
    try{$script:appBuildTimestamp=(Get-Item -LiteralPath $script:appExePath).LastWriteTimeUtc.ToString('o')}catch{$script:appBuildTimestamp='UNKNOWN'}
    Initialize-RendererEventEnvironment
    $targetArgs=if($AppScript){@('-NoProfile','-NonInteractive','-Command',$AppScript)}else{$AppArgument}
    $psi=[Diagnostics.ProcessStartInfo]::new();$psi.FileName=$AppCommand;$psi.WorkingDirectory=$root
    $psi.UseShellExecute=$false;$psi.RedirectStandardOutput=$true;$psi.RedirectStandardError=$true
    $psi.Arguments=($targetArgs|ForEach-Object {'"'+($_ -replace '"','\"')+'"'}) -join ' '
    if($runtimeEventDirectory){
        $psi.Environment['XYE_RENDERER_PROBE']='1'
    }
    $script:process=[Diagnostics.Process]::new();$process.StartInfo=$psi
    if(!$process.Start()){throw 'HARNESS_START_FAILED'}
    $script:stdoutTask=$process.StandardOutput.ReadLineAsync();$script:stderrTask=$process.StandardError.ReadLineAsync()
    $process.Refresh();$script:windowHandle=('0x{0:X}' -f $process.MainWindowHandle.ToInt64())
    $process
}

function Pump-Output {
    if($stdoutTask -and $stdoutTask.IsCompleted){
        $line=$stdoutTask.GetAwaiter().GetResult()
        if($null -eq $line){$script:stdoutEnded=$true;$script:stdoutTask=$null}
        else {$stdoutLines.Add([string]$line);$script:stdoutTask=$process.StandardOutput.ReadLineAsync()}
    }
    if($stderrTask -and $stderrTask.IsCompleted){
        $line=$stderrTask.GetAwaiter().GetResult()
        if($null -eq $line){$script:stderrEnded=$true;$script:stderrTask=$null}
        else {$stderrLines.Add([string]$line);$script:stderrTask=$process.StandardError.ReadLineAsync()}
    }
}

function Get-LogEvidence {
    $text=$stdoutLines -join [Environment]::NewLine
    [pscustomobject]@{
        Surface=($text -match '创建 Vulkan Surface 成功')
        Device=(($text -match '开始创建\s+(?:LogicalDevice|逻辑设备)') -and
            ($text -match '(?:Queue|队列)\s+获取成功（(?:Graphics\s*\+\s*Present|图形\s*\+\s*呈现)）'))
        Swapchain=($text -match 'Swapchain 创建成功')
        FirstPresent=($text -match '首帧\s+(?:Present|呈现)\s+成功')
        Window=($text -match '窗口句柄：(0x[0-9A-F]+)')
        Adapter=($text -match '已选择物理设备：([^；\r\n]+)')
    }
}

function Read-RendererEvents {
    $events=[Collections.Generic.List[object]]::new()
    if(!$runtimeEventDirectory -or !(Test-Path -LiteralPath $runtimeEventDirectory)){return @()}
    $expectedHead=$env:XYE_SOURCE_HEAD; $expectedDirty=$env:XYE_SOURCE_DIRTY
    foreach($file in Get-ChildItem -LiteralPath $runtimeEventDirectory -Recurse -File -Filter 'renderer-*.jsonl' -ErrorAction SilentlyContinue){
        foreach($line in Get-Content -LiteralPath $file.FullName -ErrorAction SilentlyContinue){
            if([string]::IsNullOrWhiteSpace($line)){continue}
            if($line -notmatch '"EventCode"\s*:\s*"(?:FramePresented|ResizeStarted|ResizeCompleted|SwapchainCreated|SwapchainRecreated|PresentLoopStopped|SwapchainReleased|RendererSessionClosed)"'){continue}
            try{$item=$line|ConvertFrom-Json -ErrorAction Stop}catch{continue}
            if($item.RunId -ne $CandidateId){$script:invalidEventIdentity=$true;continue}
            $identity=[string]$item.BuildIdentity
            if($expectedHead -and $identity -notlike "*HEAD=$expectedHead;*"){$script:invalidEventIdentity=$true;continue}
            if($expectedDirty -and $identity -notlike "*DIRTY=$expectedDirty;*"){$script:invalidEventIdentity=$true;continue}
            $item|Add-Member -NotePropertyName EvidenceFile -NotePropertyValue $file.FullName -Force
            $events.Add($item)
        }
    }
    @($events)
}

function Get-EventEvidence {
    $events=@(Read-RendererEvents)
    $postResizeEvents=[Collections.Generic.List[object]]::new()
    if($script:resizeEventBaselineCount -ge 0){for($i=$script:resizeEventBaselineCount;$i -lt $events.Count;$i++){$postResizeEvents.Add($events[$i])}}
    $frames=@($events|Where-Object EventCode -eq 'FramePresented'|Sort-Object FrameId)
    $firstFrame=$frames|Select-Object -First 1
    $generation=if($firstFrame -and $null -ne $firstFrame.SwapchainGeneration){[uint32]$firstFrame.SwapchainGeneration}else{[uint32]::MaxValue}
    $resizeStart=@($postResizeEvents|Where-Object EventCode -eq 'ResizeStarted')
    $resizeComplete=@($postResizeEvents|Where-Object { $_.EventCode -eq 'ResizeCompleted' -and $null -ne $_.Generation -and [uint32]$_.Generation -gt $generation })
    $recreated=@($postResizeEvents|Where-Object { $_.EventCode -eq 'SwapchainRecreated' -and $null -ne $_.Generation -and [uint32]$_.Generation -gt $generation })
    $laterFrame=@($postResizeEvents|Where-Object { $_.EventCode -eq 'FramePresented' -and $null -ne $_.SwapchainGeneration -and [uint32]$_.SwapchainGeneration -gt $generation -and $firstFrame -and [long]$_.FrameId -gt [long]$firstFrame.FrameId })
    $initialSwapchain=$events|Where-Object EventCode -eq 'SwapchainCreated'|Select-Object -First 1
    $resized=$false
    foreach($complete in $resizeComplete){
        $start=$resizeStart|Where-Object { $_.TimestampUtc -le $complete.TimestampUtc }|Sort-Object TimestampUtc -Descending|Select-Object -First 1
        $changed=($start -and $null -ne $complete.ActualWidth -and $null -ne $complete.ActualHeight -and
            (($complete.ActualWidth -ne $start.ActualWidth) -or ($complete.ActualHeight -ne $start.ActualHeight)))
        if($resizeStart.Count -gt 0 -and $changed){$resized=$true;break}
    }
    if(!$resized -and $initialSwapchain){
        foreach($swapchain in $recreated){
            if($null -ne $swapchain.ActualWidth -and $null -ne $swapchain.ActualHeight -and
                (($swapchain.ActualWidth -ne $initialSwapchain.ActualWidth) -or ($swapchain.ActualHeight -ne $initialSwapchain.ActualHeight))){$resized=$true;break}
        }
    }
    $codes=@($events|ForEach-Object EventCode)
    [pscustomobject]@{
        Events=$events;InitialFrame=$firstFrame;InitialGeneration=$generation
        ResizeEvidence=$resized;SwapchainRecreated=($recreated.Count -gt 0);PostResizePresent=($laterFrame.Count -gt 0)
        PresentLoopStopped=($codes -contains 'PresentLoopStopped');SwapchainReleased=($codes -contains 'SwapchainReleased')
        SessionClosed=($codes -contains 'RendererSessionClosed')
        EventFiles=@($events|Select-Object -ExpandProperty EvidenceFile -Unique)
    }
}

function Get-MissingEvidence([object]$log,[object]$event,[bool]$includeResize){
    $missing=[Collections.Generic.List[string]]::new()
    if(!$log.Surface){$missing.Add('VulkanSurface')};if(!$log.Device){$missing.Add('VulkanDevice+GraphicsPresentQueue')}
    if(!$log.Swapchain){$missing.Add('SwapchainCreated')};if(!$log.FirstPresent){$missing.Add('FirstPresent')}
    if(!$event.InitialFrame){$missing.Add('FramePresentedEvent')}
    if($includeResize){if(!$event.ResizeEvidence){$missing.Add('SwapchainExtentChangedAfterResizeRequest')};if(!$event.SwapchainRecreated){$missing.Add('SwapchainRecreated')};if(!$event.PostResizePresent){$missing.Add('PostResizeFramePresented') }}
    if(!$event.PresentLoopStopped){$missing.Add('PresentLoopStopped')};if(!$event.SwapchainReleased){$missing.Add('SwapchainReleased')}
    if(!$event.SessionClosed){$missing.Add('RendererSessionClosed')}
    $text=$stdoutLines -join [Environment]::NewLine
    if($text -notmatch 'RenderPass\s*\+\s*Framebuffer 释放成功'){$missing.Add('RenderPassFramebufferReleased')}
    if($text -notmatch '(?:LogicalDevice|逻辑设备) 释放成功'){$missing.Add('LogicalDeviceReleased')}
    if($text -notmatch '(?:VulkanSurface】释放 Vulkan Surface 成功|Surface 已释放)'){$missing.Add('VulkanSurfaceReleased')}
    if($text -notmatch '(?:VulkanInstance】释放 Vulkan 实例成功|实例 已销毁)'){$missing.Add('VulkanInstanceReleased')}
    @($missing)
}

function Invoke-Resize([Diagnostics.Process]$target){
    if(-not ('XytNative' -as [type])){
        Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class XytNative {
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr i, int x, int y, int w, int he, uint f);
}
'@
    }
    for($i=0;$i -lt 20;$i++){
        $target.Refresh()
        if($target.MainWindowHandle -ne 0){return [XytNative]::SetWindowPos($target.MainWindowHandle,[IntPtr]::Zero,0,0,1100,700,0x0004)}
        Start-Sleep -Milliseconds 250
    }
    $false
}

function Request-WindowClose([Diagnostics.Process]$target){
    $target.Refresh();if($target.HasExited){return $false}
    if($target.MainWindowHandle -eq 0){return $false}
    $target.CloseMainWindow()
}

function Stop-Tree([Diagnostics.Process]$target){
    if($target -and !$target.HasExited){try{$target.Kill($true);$target.WaitForExit(3000)|Out-Null;return $true}catch{return $false}}
    $false
}

function Wait-ForTargetExit([Diagnostics.Process]$target,[int]$seconds){
    $deadline=[DateTime]::UtcNow.AddSeconds($seconds)
    while(!$target.HasExited -and [DateTime]::UtcNow -lt $deadline){Pump-Output;Start-Sleep -Milliseconds 40}
    Pump-Output
    $target.HasExited
}

function Drain-Streams([int]$stdoutSeconds,[int]$stderrSeconds){
    $stdoutDeadline=[DateTime]::UtcNow.AddSeconds($stdoutSeconds);$stderrDeadline=[DateTime]::UtcNow.AddSeconds($stderrSeconds)
    while((!$stdoutEnded -and [DateTime]::UtcNow -lt $stdoutDeadline) -or (!$stderrEnded -and [DateTime]::UtcNow -lt $stderrDeadline)){
        Pump-Output;Start-Sleep -Milliseconds 20
    }
    $stdoutEnded -and $stderrEnded
}

function Write-Bounded([string]$Path,[string]$Text,[int]$Seconds){
    $job=Start-Job -ScriptBlock {param($p,$t)[IO.File]::WriteAllText($p,$t,(New-Object Text.UTF8Encoding($false)))} -ArgumentList $Path,$Text
    $done=Wait-Job -Job $job -Timeout $Seconds
    if(!$done){Stop-Job -Job $job -Force -ErrorAction SilentlyContinue;Remove-Job -Job $job -Force -ErrorAction SilentlyContinue;return $false}
    Receive-Job -Job $job -ErrorAction Stop|Out-Null;Remove-Job -Job $job -Force -ErrorAction SilentlyContinue;return $true
}

function Read-Bounded([Threading.Tasks.Task[string]]$Task,[int]$Seconds){
    if(!$Task.Wait($Seconds*1000)){return [pscustomobject]@{Completed=$false;Text=''}}
    try{return [pscustomobject]@{Completed=$true;Text=[string]$Task.Result}}catch{return [pscustomobject]@{Completed=$true;Text=''}}
}

function Required-Names([string]$cap){
    switch($cap){
        'P3-01' {@('VulkanSurface','VulkanDevice+GraphicsPresentQueue','SwapchainCreated','FramePresented','GracefulExit','PresentLoopStopped','ResourcesReleased')}
        'P3-02' {@('VulkanSurface','VulkanDevice+GraphicsPresentQueue','SwapchainCreated','FramePresented','ResizeCompleted','SwapchainRecreated','PostResizeFramePresented','GracefulExit','PresentLoopStopped','ResourcesReleased')}
        'P3-03' {@('DEM','Terrain','Camera')}
        'P3-04' {@('管理模式','区域编辑','PointerCapture','Toolbar','DEM')}
    }
}

function New-Record([string]$phase){
    $log=Get-LogEvidence; $events=Get-EventEvidence
    $script:surfaceEvidence=$log.Surface; $script:deviceEvidence=$log.Device; $script:swapchainEvidence=$log.Swapchain
    $script:initialPresent=([bool]$log.FirstPresent -and [bool]$events.InitialFrame)
    $script:resizeEvidence=$events.ResizeEvidence; $script:swapchainRecreated=$events.SwapchainRecreated
    $script:postResizePresent=$events.PostResizePresent; $script:presentLoopStopped=$events.PresentLoopStopped
    $deviceRelease=($stdoutLines -join [Environment]::NewLine) -match '(?:LogicalDevice|逻辑设备) 释放成功'
    $script:resourceReleaseEvidence=($events.SwapchainReleased -and $events.SessionClosed -and $deviceRelease -and
        (($stdoutLines -join [Environment]::NewLine) -match '(?:Surface 已释放|VulkanSurface】释放 Vulkan Surface 成功)') -and
        (($stdoutLines -join [Environment]::NewLine) -match '(?:实例 已销毁|VulkanInstance】释放 Vulkan 实例成功)'))
    $adapterName=if(($stdoutLines -join [Environment]::NewLine) -match '已选择物理设备：([^；\r\n]+)'){$Matches[1]}else{'UNKNOWN'}
    $handle=if(($stdoutLines -join [Environment]::NewLine) -match '窗口句柄：(0x[0-9A-F]+)'){$Matches[1]}else{$windowHandle}
    $sourceHead=if($env:XYE_SOURCE_HEAD){$env:XYE_SOURCE_HEAD}else{'UNKNOWN'}
    $branch=if($env:XYE_SOURCE_BRANCH){$env:XYE_SOURCE_BRANCH}else{'UNKNOWN'}
    $dirty=if($env:XYE_SOURCE_DIRTY){$env:XYE_SOURCE_DIRTY}else{'UNKNOWN'}
    $eventPath=if($events.EventFiles.Count){$events.EventFiles -join ';'}else{$runtimeEventDirectory}
    $missing=Get-MissingEvidence $log $events ($Capability -eq 'P3-02')
    $end=[DateTimeOffset]::UtcNow
    [ordered]@{
        Schema='XYT-P3-R1/4';RecordPhase=$phase;Version=(Read-Version);Commit=$sourceHead;Branch=$branch;DirtyState=$dirty
        AppExePath=$appExePath;AppBuildTimestamp=$appBuildTimestamp;GPU=$(if($env:XYT_GPU){$env:XYT_GPU}else{'UNKNOWN'})
        VulkanAdapter=$adapterName;WindowHost=$handle;VulkanSurface=$surfaceEvidence;VulkanDevice=$deviceEvidence
        Swapchain=$swapchainEvidence;PresentEvidence=$initialPresent;FramePresentedEvidence=([bool]$events.InitialFrame)
        ResizeRequested=$resizeRequested;ResizeEvidence=$resizeEvidence;SwapchainRecreated=$swapchainRecreated
        ResizeRequestedUtc=$(if($resizeRequestedUtc){$resizeRequestedUtc.ToString('o')}else{''})
        PostResizePresent=$postResizePresent;CloseRequested=$closeRequested;CloseRequestAccepted=$closeAccepted
        ProcessExitedAfterClose=$exitedAfterClose;ForcedTermination=$forcedTermination
        PresentLoopStopped=$presentLoopStopped;ResourceReleaseEvidence=$resourceReleaseEvidence
        RendererEventPath=$eventPath;EventIdentityValid=(-not $invalidEventIdentity)
        CandidateId=$CandidateId;RuntimeTestId=$id;Capability=$Capability;StartTime=$started.ToString('o')
        EndTime=$(if($phase -eq 'START'){''}else{$end.ToString('o')})
        DurationMs=$(if($phase -eq 'START'){''}else{[int](($end-$started).TotalMilliseconds)})
        Status=$status;Result=$status;Reason=$reason;HarnessStatus=$harnessStatus
        EvidencePath=$stdoutLog;StderrEvidencePath=$stderrLog;BuildEvidencePath=$buildLog
        HarnessFailure=($status -in @('HARNESS_FAILURE','EVIDENCE_FAILURE'));BlockedBy=$reason;ExitCode=$exit
        RequiredMarkers=@(Required-Names $Capability);MissingMarkers=$missing
    }
}

function Save-Record([object]$Record,[switch]$InjectFailure){
    $text=$Record|ConvertTo-Json -Depth 10
    if(!$InjectFailure -and (Write-Bounded $json $text $EvidenceWriteTimeoutSeconds)){return $true}
    $fallback=Join-Path $EvidenceRoot "$id.failure.json"
    Write-Bounded $fallback $text $EvidenceWriteTimeoutSeconds
}

function Test-StartupReady {
    $log=Get-LogEvidence
    $log.Surface -and $log.Device -and $log.Swapchain -and $log.FirstPresent
}

function Run-GuiCapability {
    $markerDeadline=[DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    while(!$process.HasExited -and [DateTime]::UtcNow -lt $markerDeadline){
        Pump-Output
        if(Test-StartupReady){$script:initialPresent=$true;break}
        Start-Sleep -Milliseconds 40
    }
    if(-not $initialPresent){
        if($process.HasExited){$script:exit=$process.ExitCode;$script:status=if($exit -ne 0){'CRASH'}else{'FAIL'};$script:reason='TARGET_EXITED_BEFORE_STARTUP_EVIDENCE';$script:harnessStatus='TARGET_EXITED_BEFORE_MARKERS'}
        else {$script:status='TIMEOUT';$script:reason='MARKER_ACQUISITION_TIMEOUT';$script:harnessStatus='MARKER_TIMEOUT'}
        return
    }
    if($Capability -eq 'P3-02'){
        $frameDeadline=[DateTime]::UtcNow.AddSeconds($ResizeTimeoutSeconds)
        while(!$process.HasExited -and [DateTime]::UtcNow -lt $frameDeadline){
            Pump-Output;$script:eventState=Get-EventEvidence
            if($eventState.InitialFrame){break};Start-Sleep -Milliseconds 50
        }
        if(!$eventState.InitialFrame){$script:status='FAIL';$script:reason='INITIAL_FRAME_EVENT_MISSING';$script:harnessStatus='FRAME_EVENT_MISSING'}
        elseif(!$process.HasExited){
            $initialGeneration=$eventState.InitialGeneration
            $script:resizeRequestedUtc=[DateTimeOffset]::UtcNow
            $script:resizeEventBaselineCount=@(Read-RendererEvents).Count
            $script:resizeRequested=Invoke-Resize $process
            if(!$resizeRequested){$script:status='FAIL';$script:reason='WINDOW_RESIZE_REQUEST_FAILED';$script:harnessStatus='RESIZE_REQUEST_FAILED'}
            else {
                $resizeDeadline=[DateTime]::UtcNow.AddSeconds($ResizeTimeoutSeconds)
                while(!$process.HasExited -and [DateTime]::UtcNow -lt $resizeDeadline){
                    Pump-Output;$script:eventState=Get-EventEvidence
                    if($eventState.ResizeEvidence -and $eventState.SwapchainRecreated -and $eventState.PostResizePresent){break}
                    Start-Sleep -Milliseconds 50
                }
                $script:eventState=Get-EventEvidence
                $script:resizeEvidence=$eventState.ResizeEvidence;$script:swapchainRecreated=$eventState.SwapchainRecreated
                $script:postResizePresent=$eventState.PostResizePresent
                if(!$resizeEvidence -or !$swapchainRecreated -or !$postResizePresent){
                    $script:status='FAIL';$script:reason='RESIZE_RECREATE_POST_PRESENT_EVIDENCE_MISSING';$script:harnessStatus='RESIZE_EVIDENCE_INCOMPLETE'
                }
            }
        }
    }
    if($process.HasExited){$script:status='FAIL';if(!$reason){$script:reason='TARGET_EXITED_BEFORE_CLOSE_REQUEST';$script:harnessStatus='TARGET_EXITED_BEFORE_CLOSE'};return}
    $script:closeRequested=$true
    $script:closeAccepted=Request-WindowClose $process
    if(!$closeAccepted){$script:status='FAIL';$script:reason='WINDOW_CLOSE_REQUEST_NOT_ACCEPTED';$script:harnessStatus='CLOSE_REQUEST_FAILED';return}
    if(Wait-ForTargetExit $process $CloseTimeoutSeconds){
        $script:exit=$process.ExitCode;$script:exitedAfterClose=$true;$harnessStatus='TARGET_EXITED_AFTER_CLOSE'
        if($exit -ne 0){$script:status='CRASH';$script:reason="TARGET_EXIT_CODE_$exit"}
        elseif($status -eq 'HARNESS_FAILURE'){$script:status='PASS'}
    } else {$script:status='TIMEOUT';$script:reason='CLOSE_REQUEST_EXIT_TIMEOUT';$script:harnessStatus='CLOSE_EXIT_TIMEOUT'}
}

function Run-LegacyCapability {
    $deadline=[DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    while(!$process.HasExited -and [DateTime]::UtcNow -lt $deadline){Pump-Output;Start-Sleep -Milliseconds 50}
    if(!$process.HasExited){$script:status='TIMEOUT';$script:reason='TARGET_PROCESS_TIMEOUT';$script:harnessStatus='TARGET_TIMEOUT';return}
    $script:exit=$process.ExitCode;$script:harnessStatus='TARGET_EXITED'
    if($exit -ne 0){$script:status='CRASH';$script:reason="TARGET_EXIT_CODE_$exit";return}
    $text=$stdoutLines -join [Environment]::NewLine
    $missing=@(Markers $Capability|Where-Object {$text -notmatch [regex]::Escape($_)})
    if($missing.Count){$script:status='FAIL';$script:reason='MISSING_MARKERS: '+($missing -join ', ')}else{$script:status='PASS';$script:reason='REQUIRED_MARKERS_PRESENT'}
}

function Markers([string]$cap){
    switch($cap){
        'P3-03' {@('DEM','Terrain','Camera')}
        'P3-04' {@('管理模式','区域编辑','PointerCapture','Toolbar','DEM')}
    }
}

$status='HARNESS_FAILURE';$reason='';$harnessStatus='START_RECORD_PENDING';$start=New-Record 'START'
$startText=$start|ConvertTo-Json -Depth 10
$startWritten=(Write-Bounded $json $startText $EvidenceWriteTimeoutSeconds) -and (Write-Bounded $startJson $startText $EvidenceWriteTimeoutSeconds)
if(!$startWritten){$status='EVIDENCE_FAILURE';$reason='START_RECORD_WRITE_TIMEOUT';$harnessStatus='EVIDENCE_WRITE_TIMEOUT'}
try {
    if(!$startWritten){throw 'START_RECORD_WRITE_TIMEOUT'}
    if($NoGui){$status='BLOCKED_NO_GUI';$reason='NO_GUI_WITNESS_ENVIRONMENT';$harnessStatus='BLOCKED_NO_GUI';throw $reason}
    $process=Start-Target
    if($Capability -in @('P3-01','P3-02')){Run-GuiCapability}
    else {Run-LegacyCapability}
} catch {
    if(!$reason){$reason=$_.Exception.Message}
    if($reason -eq 'NO_GUI_WITNESS_ENVIRONMENT'){$status='BLOCKED_NO_GUI'}
    elseif($status -eq 'HARNESS_FAILURE'){$status='HARNESS_FAILURE'}
} finally {
    if($process -and !$process.HasExited){$script:forcedTermination=Stop-Tree $process}
    if($process){
        try{[void](Drain-Streams $StdoutDrainTimeoutSeconds $StderrDrainTimeoutSeconds)}catch{}
        try{if($process.HasExited -and $null -eq $exit){$script:exit=$process.ExitCode}}catch{}
        $process.Dispose()
    }
}

$stdoutSaved=Write-Bounded $stdoutLog ($stdoutLines -join [Environment]::NewLine) $EvidenceWriteTimeoutSeconds
$stderrSaved=Write-Bounded $stderrLog ($stderrLines -join [Environment]::NewLine) $EvidenceWriteTimeoutSeconds
if(!$stdoutSaved -or !$stderrSaved){$status='EVIDENCE_FAILURE';$reason='TARGET_OUTPUT_EVIDENCE_WRITE_FAILED';$harnessStatus='OUTPUT_EVIDENCE_WRITE_FAILURE'}

if($status -notin @('HARNESS_FAILURE','EVIDENCE_FAILURE','BLOCKED_NO_GUI','TIMEOUT','CRASH')){
    if($FakeStdoutDrainTimeout){$status='TIMEOUT';$reason='STDOUT_DRAIN_TIMEOUT';$harnessStatus='STDOUT_DRAIN_TIMEOUT'}
    elseif($FakeStderrDrainTimeout){$status='TIMEOUT';$reason='STDERR_DRAIN_TIMEOUT';$harnessStatus='STDERR_DRAIN_TIMEOUT'}
    elseif($Capability -in @('P3-01','P3-02')){
        $eventState=Get-EventEvidence;$logState=Get-LogEvidence
        if($invalidEventIdentity){$status='FAIL';$reason='STRUCTURED_EVENT_IDENTITY_MISMATCH';$harnessStatus='EVENT_IDENTITY_MISMATCH'}
        else {
            $missing=@(Get-MissingEvidence $logState $eventState ($Capability -eq 'P3-02'))
            if($missing.Count){$status='FAIL';$reason='REQUIRED_RUNTIME_EVIDENCE_MISSING: '+($missing -join ', ');$harnessStatus='EVIDENCE_INCOMPLETE'}
            elseif(!$closeRequested -or !$closeAccepted -or !$exitedAfterClose -or $forcedTermination){$status='FAIL';$reason='GRACEFUL_CLOSE_NOT_PROVEN';$harnessStatus='CLOSE_EVIDENCE_INCOMPLETE'}
            else {$status='PASS';$reason='RUNTIME_EVENTS_AND_GRACEFUL_CLOSE_VERIFIED';$harnessStatus='TARGET_EXITED_AFTER_CLOSE'}
        }
    }
}
if($status -eq 'TIMEOUT' -and $reason -eq 'MARKER_ACQUISITION_TIMEOUT' -and !$forcedTermination -and $process -and !$process.HasExited){$script:forcedTermination=Stop-Tree $process}
$harnessStatus=if($harnessStatus -eq 'START_RECORD_PENDING'){'HARNESS_FAILURE'}else{$harnessStatus}
$record=New-Record 'END'
if($FakeHarnessFault){$status='EVIDENCE_FAILURE';$reason='EVIDENCE_WRITE_INJECTED_FAILURE';$harnessStatus='EVIDENCE_WRITE_FAILURE';$record=New-Record 'END'}
$saved=Save-Record $record -InjectFailure:$FakeHarnessFault
if(!$saved){$status='EVIDENCE_FAILURE';$reason='FINAL_EVIDENCE_WRITE_TIMEOUT';$harnessStatus='EVIDENCE_WRITE_TIMEOUT';$record=New-Record 'FAILURE';[void](Save-Record $record)}
Write-Output ($record|ConvertTo-Json -Depth 10)
if($status -in @('HARNESS_FAILURE','EVIDENCE_FAILURE')){exit 3}
if($status -in @('FAIL','CRASH','TIMEOUT')){exit 1}
if($status -eq 'BLOCKED_NO_GUI'){exit 2}
exit 0
