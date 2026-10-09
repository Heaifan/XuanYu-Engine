$ErrorActionPreference = 'Stop'
$runner = Join-Path $PSScriptRoot 'xyt-runner.ps1'
$temp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-runner-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temp | Out-Null
function Invoke-Runner([string]$Name, [string[]]$Files, [string[]]$Additional = @()) {
    $inputPath = Join-Path $temp "$Name.txt"; $outputPath = Join-Path $temp "$Name.json"
    $Files | Set-Content -LiteralPath $inputPath -Encoding UTF8
    $args=@('-NoProfile','-ExecutionPolicy','Bypass','-File',$runner,'-ChangedListPath',$inputPath,'-OutputPath',$outputPath)
    if($Additional.Count){$args+=@('-AgentTests',$Additional)}
    & powershell.exe @args | Out-Null
    [pscustomobject]@{ ExitCode=$LASTEXITCODE; Result=(Get-Content -Raw $outputPath | ConvertFrom-Json) }
}
try {
    $core = Invoke-Runner 'core' @('XuanYu.Core/Space/ViewProjectionState.cs')
    if ($core.ExitCode -ne 0 -or @($core.Result.requiredTests) -notcontains 'XuanYu.Core.Tests/') { throw 'Core route lost the Core test suite.' }
    $xyt = Invoke-Runner 'xyt' @('XYT/Execution/xyt-executor.ps1') @('extra/Required.Tests')
    if ($xyt.ExitCode -ne 0 -or @($xyt.Result.requiredTests) -notcontains 'extra/Required.Tests') { throw 'XYT fixed or agent-added tests were lost.' }
    if (@($xyt.Result.requiredTests | Where-Object { $_ -like 'XYT/*selftest.ps1' }).Count -lt 1) { throw 'XYT module selftests were not selected.' }
    $unknown = Invoke-Runner 'unknown' @('unmapped/file.txt')
    if ($unknown.ExitCode -ne 2 -or $unknown.Result.status -ne 'REVIEW_REQUIRED') { throw 'Unknown path was not blocked for review.' }
    'XYT RUNNER SELFTEST: PASS'
} finally { Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue }
