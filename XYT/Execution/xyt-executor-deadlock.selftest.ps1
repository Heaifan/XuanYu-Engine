$ErrorActionPreference = 'Stop'
$executor = Join-Path $PSScriptRoot 'xyt-executor.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('xyt-h3f-' + [guid]::NewGuid().ToString('N'))
$passed = 0; $total = 5
function Run-Plan($Tests, [string]$Name) {
    $plan = Join-Path $root "$Name.json"; $out = Join-Path $root "$Name.out"
    [ordered]@{ planId = $Name; tests = $Tests } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $plan
    $lines = @(& pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $executor -PlanPath $plan -OutputPath $out 2>&1)
    [pscustomobject]@{ Code = $LASTEXITCODE; Text = ((Get-Content $out -ErrorAction SilentlyContinue) + $lines -join "`n"); Json = (Get-Content -Raw $out | ConvertFrom-Json) }
}
function Assert([bool]$Condition, [string]$Message) { if (!$Condition) { throw $Message }; $script:passed++ }
function T([string]$Id, [string[]]$Depends) { [ordered]@{ testId = $Id; dependsOn = $Depends; command = 'exit 0'; timeoutSeconds = 20 } }
try {
    New-Item -ItemType Directory -Force -Path $root | Out-Null
    $r = Run-Plan @((T A @('B')), (T B @('A'))) 'H3F-01'
    Assert (($r.Code -eq 2) -and ($r.Text -match 'CIRCULAR_TEST_DEPENDENCY') -and ($r.Text -match 'A -> B -> A')) 'H3F-01 contract failed'
    $r = Run-Plan @((T A @('C')), (T B @('A')), (T C @('B'))) 'H3F-02'
    Assert (($r.Code -eq 2) -and ($r.Text -match 'CIRCULAR_TEST_DEPENDENCY') -and ($r.Text -match 'A -> C -> B -> A|A -> B -> C -> A')) 'H3F-02 contract failed'
    $r = Run-Plan @((T A @('B')), (T B @('C')), (T C @())) 'H3F-03'
    Assert (($r.Code -eq 0) -and ($r.Json.status -eq 'PASS') -and (-not ($r.Text -match 'CIRCULAR_TEST_DEPENDENCY'))) ("H3F-03 non-cycle contract failed code={0}: {1}" -f $r.Code, $r.Text)
    $r = Run-Plan @([ordered]@{ testId = 'A'; dependsOn = @('Missing'); command = 'exit 0'; timeoutSeconds = 20 }) 'H3F-04'
    Assert (($r.Code -eq 0) -and ($r.Json.status -eq 'UNCLASSIFIED') -and (-not ($r.Text -match 'CIRCULAR_TEST_DEPENDENCY'))) 'H3F-04 error classification changed'
    $r = Run-Plan @((T A @('B')), (T B @('A'))) 'H3F-05'
    Assert ($r.Text -match '"errorCode":\s+"CIRCULAR_TEST_DEPENDENCY"' -and $r.Text -match '"cyclePath":\s+"A -> B -> A"') 'H3F-05 typed error was lost by wrapper'
    "H3F SELFTEST PASS $passed/$total"
} finally { if (Test-Path $root) { Start-Sleep -Milliseconds 150; Remove-Item -LiteralPath $root -Recurse -Force -ErrorAction SilentlyContinue } }
