$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$runner = Join-Path $PSScriptRoot 'xyt-runner.ps1'
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('xyt-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tmp | Out-Null
try {
    function Invoke-Runner($name, $files, $agentTests = @(), $supplementPath = '') {
        $input = Join-Path $tmp "$name.txt"
        if (@($files).Count -eq 0) { Set-Content -LiteralPath $input -Value '' -Encoding UTF8 }
        else { $files | Set-Content -LiteralPath $input -Encoding UTF8 }
        $out = Join-Path $tmp "$name.json"
        & $runner -ChangedListPath $input -OutputPath $out -AgentTests $agentTests -SupplementPath $supplementPath | Out-Null
        $script:LastRunnerExit = $LASTEXITCODE
        return (Get-Content -Raw $out | ConvertFrom-Json)
    }
    function Invoke-Empty-DiffRange($name) {
        $out = Join-Path $tmp "$name.json"
        & $runner -DiffRange 'HEAD..HEAD' -OutputPath $out | Out-Null
        $script:LastRunnerExit = $LASTEXITCODE
        return (Get-Content -Raw $out | ConvertFrom-Json)
    }
    $a = Invoke-Runner 'fixed' @('scripts/governance/test-evidence-gate.ps1')
    if ($a.status -ne 'PASS' -or @($a.requiredTests).Count -eq 0) { throw 'fixed mapping failed' }
    if ($a.changedFiles[0].mapping -ne 'fixed-pattern') { throw 'XYT ownership must resolve from its fixed mapping' }
    $b = Invoke-Runner 'agent-add' @('XuanYu.World/Map/MapMarker.cs') @('World.Tests/Map/MapMarkerTests.cs')
    if (@($b.requiredTests) -notcontains 'World.Tests/Map/MapMarkerTests.cs') { throw 'agent test was not retained' }
    if (@($b.requiredTests) -notcontains 'XuanYu.World.Tests/') { throw 'agent input removed fixed required test' }
    $c = Invoke-Runner 'dispute' @('unknown/feature.xyz')
    if ($c.status -ne 'REVIEW_REQUIRED' -or $script:LastRunnerExit -ne 2 -or @($c.disputes).Count -eq 0) { throw 'unknown mapping was not disputed with exit 2' }
    $empty = Invoke-Runner 'empty-scope' @()
    if ($empty.status -ne 'REVIEW_REQUIRED' -or $script:LastRunnerExit -ne 2 -or
        @($empty.disputes | Where-Object { $_ -like '*changed-file scope is empty*' }).Count -eq 0) {
        throw 'empty changed-file scope must be disputed with exit 2'
    }
    $emptyDiff = Invoke-Empty-DiffRange 'empty-diff-range'
    if ($emptyDiff.status -ne 'REVIEW_REQUIRED' -or $script:LastRunnerExit -ne 2 -or
        @($emptyDiff.disputes | Where-Object { $_ -like '*changed-file scope is empty*' }).Count -eq 0) {
        throw 'empty DiffRange must be disputed with exit 2'
    }
    $d = Invoke-Runner 'supplement' @('XuanYu.Editor/MapEditing/MapGeometryHitTester.cs') @() (Join-Path $tmp 'supplement.json')
    if (@($d.supplementalTests).Count -ne 0) { throw 'missing supplement should stay empty' }
    $supplement = [pscustomobject]@{ items = @([pscustomobject]@{ test = 'extra/test'; reason = 'AI risk review'; source = 'agent-judgment' }) }
    $supplement | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $tmp 'valid.json') -Encoding UTF8
    $e = Invoke-Runner 'valid-supplement' @('XuanYu.Editor/MapEditing/MapGeometryHitTester.cs') @() (Join-Path $tmp 'valid.json')
    if (@($e.supplementalTests) -notcontains 'extra/test' -or @($e.requiredTests) -notcontains 'extra/test') { throw 'valid supplement was not added' }
    'XYT RUNNER SELFTEST: PASS'
    exit 0
}
finally { Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue }
