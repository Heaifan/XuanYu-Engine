$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$entry = Join-Path $PSScriptRoot 'xyt-ipo.ps1'
$temp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-ipo-' + [guid]::NewGuid())
New-Item -ItemType Directory -Path $temp | Out-Null

function Invoke-Ipo([string[]]$Arguments) {
    $output = & pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $entry @Arguments 2>&1
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Output = ($output -join "`n") }
}

try {
    $one = Join-Path $temp 'one.json'
    $result = Invoke-Ipo @('-Capability', 'mode-switch', '-ChangeSet', 'context transition', '-RequiredP4Scope', 'mode switching', '-Version', 'v1', '-Commit', 'abc', '-Branch', 'main', '-OutputPath', $one)
    if ($result.ExitCode -ne 0) { throw "Single capability failed: $($result.Output)" }
    $oneData = Get-Content -Raw $one | ConvertFrom-Json
    $oneItem = @($oneData.Items)[0]
    if (@($oneData.Items).Count -ne 1) { throw 'Single capability did not produce one item.' }
    if ($oneItem.判定 -ne 'P4 PENDING') { throw 'Default verdict is not P4 PENDING.' }
    if (($oneItem.'过程 P' -join ' ') -notmatch '①.*②.*③') { throw 'Process is not numbered.' }

    $many = Join-Path $temp 'many.json'
    $result = Invoke-Ipo @('-Capability', 'mode-switch,region-editing,mode-switch', '-ChangeSet', 'A,B', '-RequiredP4Scope', 'A,B', '-Version', 'v1', '-Commit', 'abc', '-Branch', 'main', '-OutputPath', $many)
    if ($result.ExitCode -ne 0) { throw "Multiple capability failed: $($result.Output)" }
    $manyData = Get-Content -Raw $many | ConvertFrom-Json
    if (@($manyData.Items).Count -ne 2) { throw 'Capability deduplication/minimal set failed.' }
    if ((@($manyData.Items | Where-Object { $_.判定 -ne 'P4 PENDING' })).Count -ne 0) { throw 'User verdict was auto-filled.' }
    if ((@($manyData.Items | Where-Object { (@($_.'过程 P') -join ' ') -notmatch '①.*②.*③' })).Count -ne 0) { throw 'Process numbering missing.' }
    Write-Output 'XYT IPO SELFTEST PASS'
} finally {
    if (Test-Path $temp) { Remove-Item -LiteralPath $temp -Recurse -Force }
}
