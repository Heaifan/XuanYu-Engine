[CmdletBinding()]
param()

$helper = Join-Path $PSScriptRoot 'knowledge-preflight.ps1'
$required = @('K-REN-004', 'L-REN-001', 'EXP-ARCH-001', 'EXP-GOVERNANCE-002')

foreach ($phase in @('Planning', 'Execution')) {
    $output = (& $helper -Phase $phase -Domain Rendering -Keywords Grid, Depth, Reverse-Z | Out-String)
    foreach ($id in $required) {
        if ($output -notmatch [regex]::Escape($id)) {
            throw "$phase query did not find $id"
        }
    }
    if ($output -notmatch "Phase: $($phase.ToUpperInvariant())") {
        throw "$phase query reported the wrong phase"
    }
}

$unknown = (& $helper -Phase Execution -Domain NotARegisteredDomain -Keywords NotARealKeyword | Out-String)
if ($unknown -notmatch 'Result: NO MATCHES') {
    throw 'Unknown query did not report NO MATCHES'
}

Write-Output 'KNOWLEDGE PREFLIGHT SELFTEST: PASS'
