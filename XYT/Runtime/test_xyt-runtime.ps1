$ErrorActionPreference = 'Stop'
$env:XYT_SKIP_GIT = '1'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$pwsh = (Get-Command pwsh -ErrorAction Stop).Source
$runtime = Join-Path $PSScriptRoot 'xyt-runtime.ps1'
$tmp = Join-Path $root ('obj\xyt-runtime-selftest-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tmp | Out-Null
$errors = [Collections.Generic.List[string]]::new()
$passed = $false
$guiScript = Join-Path $tmp 'gui-host.ps1'

$guiSource = @'
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
$eventDirectory = Join-Path $env:XYT_SELFTEST_EVENT_DIRECTORY 'runtime'
New-Item -ItemType Directory -Force -Path $eventDirectory | Out-Null
$eventPath = Join-Path $eventDirectory 'renderer-runtime-000.jsonl'
$identity = "HEAD=$env:XYE_SOURCE_HEAD;DIRTY=$env:XYE_SOURCE_DIRTY;TREE=$env:XYE_SOURCE_FINGERPRINT;BUILD=SELFTEST;VERSION=SELFTEST"
function Write-Event([string]$code, [uint32]$generation, [Nullable[long]]$frame = $null) {
    $width=if($generation -eq 0){776}else{500};$height=if($generation -eq 0){650}else{491}
    $item = [ordered]@{ EventCode=$code; Channel='Runtime'; Level='Info'; RunId=$env:XYT_CANDIDATE_ID; SessionId='selftest'; TimestampUtc=[DateTimeOffset]::UtcNow.ToString('o'); BuildIdentity=$identity; FrameId=$frame; Generation=$generation; RequestedWidth=1100; RequestedHeight=700; ActualWidth=$width; ActualHeight=$height; SwapchainGeneration=$generation }
    Add-Content -LiteralPath $eventPath -Value ($item | ConvertTo-Json -Compress) -Encoding utf8
}
$form = [Windows.Forms.Form]::new()
$form.Text = 'XYT Runtime Selftest'
$form.ShowInTaskbar = $true
$form.WindowState = [Windows.Forms.FormWindowState]::Normal
$form.ClientSize = [Drawing.Size]::new(776,650)
$form.add_SizeChanged({
    if ($env:XYT_SELFTEST_OMIT_POST_RESIZE_PRESENT -ne '1') {
        Write-Event 'ResizeStarted' 0
        Write-Event 'SwapchainRecreated' 1
        Write-Event 'ResizeCompleted' 1
        Write-Event 'FramePresented' 1 2
    } else {
        Write-Event 'ResizeStarted' 0
        Write-Event 'SwapchainRecreated' 1
        Write-Event 'ResizeCompleted' 1
    }
})
$form.add_FormClosing({ param($sender,$args) if ($env:XYT_SELFTEST_IGNORE_CLOSE -eq '1') { $args.Cancel = $true } })
$form.add_FormClosed({
    Write-Event 'PresentLoopStopped' 1
    Write-Event 'SwapchainReleased' 1
    Write-Event 'RendererSessionClosed' 1
    [Console]::Out.WriteLine('呈现 泵已停止')
    [Console]::Out.WriteLine('RenderPass + Framebuffer 释放成功')
    [Console]::Out.WriteLine('Swapchain 释放成功')
    [Console]::Out.WriteLine('逻辑设备 释放成功')
    [Console]::Out.WriteLine('Surface 已释放')
    [Console]::Out.WriteLine('实例 已销毁')
})
$form.Show()
[Windows.Forms.Application]::DoEvents()
Write-Event 'RendererSessionStarted' 0
Write-Event 'SwapchainCreated' 0
Write-Event 'FramePresented' 0 1
[Console]::Out.WriteLine('【VulkanSurface】创建 Vulkan Surface 成功；窗口句柄：0x1234')
if ($env:XYT_SELFTEST_MARKER_MODE -ne 'SurfaceOnly') {
    [Console]::Out.WriteLine('【VulkanDevice】开始创建 LogicalDevice')
    [Console]::Out.WriteLine('【VulkanDevice】Queue 获取成功（Graphics + Present）')
    [Console]::Out.WriteLine('【VulkanSwapchain】Swapchain 创建成功；选择物理尺寸=776x650')
    [Console]::Out.WriteLine('【VulkanClearFrame】首帧 Present 成功；imageIndex=0')
}
[Windows.Forms.Application]::Run($form)
'@

function Invoke-Case([string]$Name, [string]$Capability, [string]$ScriptPath, [int]$ExpectedExit,
    [int]$TimeoutSeconds = 3, [hashtable]$Environment = @{}, [string[]]$ExtraArguments = @()) {
    $quote = { param([string]$value) "'" + $value.Replace("'","''") + "'" }
    $command = "& $(& $quote $runtime) -Capability $(& $quote $Capability) -CandidateId $(& $quote "SELFTEST-$Name") " +
        "-AppCommand $(& $quote $scriptPath) -TimeoutSeconds $TimeoutSeconds -CloseTimeoutSeconds 2 " +
        "-ResizeTimeoutSeconds 2 -StdoutDrainTimeoutSeconds 2 -StderrDrainTimeoutSeconds 2 " +
        "-EvidenceWriteTimeoutSeconds 2 -EvidenceRoot $(& $quote $tmp)"
    if ($scriptPath -like '*powershell.exe') { $command += " -AppArgument @('-STA','-NoProfile','-ExecutionPolicy','Bypass','-File',$(& $quote $guiScript))" }
    if ($ExtraArguments.Count) { $command += ' ' + ($ExtraArguments -join ' ') }
    foreach ($key in $Environment.Keys) { [Environment]::SetEnvironmentVariable($key,[string]$Environment[$key],'Process') }
    try { & $pwsh -NoProfile -Command $command *> (Join-Path $tmp "$Name.console.log"); $actualExit = $LASTEXITCODE }
    finally { foreach ($key in $Environment.Keys) { [Environment]::SetEnvironmentVariable($key,$null,'Process') } }
    if ($actualExit -ne $ExpectedExit) { $errors.Add("$Name exit=$actualExit expected=$ExpectedExit") }
    $recordPath = Get-ChildItem -LiteralPath $tmp -Filter "$Capability-*.json" -File |
        Where-Object Name -notlike '*.start.json' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $recordPath) {
        $console = Get-Content -Tail 20 -LiteralPath (Join-Path $tmp "$Name.console.log") -ErrorAction SilentlyContinue
        $errors.Add("$Name missing final record: $($console -join ' | ')")
        return $null
    }
    Get-Content -Raw -LiteralPath $recordPath.FullName | ConvertFrom-Json
}

try {
    $env:XYT_SELFTEST_EVENT_DIRECTORY = Join-Path $tmp 'events'
    $env:XYT_CANDIDATE_ID = 'SELFTEST'
    Set-Content -LiteralPath $guiScript -Value $guiSource -Encoding utf8
    $windowsPowerShell = Join-Path $env:WINDIR 'System32\WindowsPowerShell\v1.0\powershell.exe'
    if (-not (Test-Path -LiteralPath $windowsPowerShell)) { throw 'Windows PowerShell required for the STA GUI fixture.' }

    $gui = Invoke-Case 'gui-close' 'P3-01' $windowsPowerShell 0 5
    if ($gui.Status -ne 'PASS' -or -not $gui.CloseRequested -or -not $gui.ProcessExitedAfterClose -or $gui.ForcedTermination) { $errors.Add('gui-close did not prove marker-triggered graceful exit') }
    if (-not $gui.PresentLoopStopped -or -not $gui.ResourceReleaseEvidence) { $errors.Add('gui-close did not prove PresentLoop stop and resource release') }

    $stalled = Invoke-Case 'missing-marker' 'P3-01' $windowsPowerShell 1 1 @{ XYT_SELFTEST_MARKER_MODE='SurfaceOnly' }
    if ($stalled.Status -ne 'TIMEOUT' -or $stalled.HarnessStatus -ne 'MARKER_TIMEOUT' -or $stalled.CloseRequested -or -not $stalled.ForcedTermination) { $errors.Add('missing-marker did not time out before close request') }

    $ignored = Invoke-Case 'ignored-close' 'P3-01' $windowsPowerShell 1 2 @{ XYT_SELFTEST_IGNORE_CLOSE='1' }
    if ($ignored.Status -ne 'TIMEOUT' -or $ignored.HarnessStatus -ne 'CLOSE_EXIT_TIMEOUT' -or -not $ignored.CloseRequested -or -not $ignored.ForcedTermination) { $errors.Add('ignored-close did not report close-exit timeout') }

    $resize = Invoke-Case 'resize-present' 'P3-02' $windowsPowerShell 0 5
    if ($resize.Status -ne 'PASS' -or -not $resize.ResizeEvidence -or -not $resize.SwapchainRecreated -or -not $resize.PostResizePresent) { $errors.Add('resize-present did not prove changed generation and later FramePresented') }

    $noPostPresent = Invoke-Case 'resize-no-present' 'P3-02' $windowsPowerShell 1 5 @{ XYT_SELFTEST_OMIT_POST_RESIZE_PRESENT='1' }
    if ($noPostPresent.Status -ne 'FAIL' -or $noPostPresent.PostResizePresent -or $noPostPresent.CloseRequested -eq $false) { $errors.Add('resize without post-rebuild present was not rejected and closed') }

    if ($errors.Count) { throw ($errors -join [Environment]::NewLine) }
    'XYT RUNTIME HARNESS SELFTEST: PASS'
    $passed = $true
    exit 0
} finally {
    if ($passed) { Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue }
    else { Write-Warning "Selftest evidence retained at $tmp" }
}
