[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$ledger = Join-Path $PSScriptRoot ('ledger-' + [guid]::NewGuid().ToString('N') + '.jsonl')

function Assert-True([bool]$value, [string]$message) { if (-not $value) { throw $message } }
function Assert-Equal($actual, $expected, [string]$message) {
    if ($actual -ne $expected) { throw "${message}: actual=[$actual] expected=[$expected]" }
}
function New-Event([string]$id, [string]$kind, [string]$at) {
    [pscustomobject]@{
        schemaVersion = 'Handclap.Event/1'; eventId = $id; timestampUtc = $at
        kind = $kind; actor = 'tester'; scope = 'governance'
        correlationId = 'corr-1'; subject = $null; data = @{ value = $id }
    }
}

try {
    . (Join-Path $PSScriptRoot '..\core\HandclapEvent.ps1')
    . (Join-Path $PSScriptRoot '..\ledger\HandclapLedger.ps1')
    . (Join-Path $PSScriptRoot '..\ledger\HandclapQuery.ps1')
    $t0 = '2026-10-02T00:00:00.0000000Z'; $t1 = '2026-10-02T00:01:00.0000000Z'
    $e1 = New-Event ([guid]::NewGuid().ToString('D')) 'CREATED' $t0
    $e2 = New-Event ([guid]::NewGuid().ToString('D')) 'TRANSFERRED' $t1
    [void](Add-HandclapEvent -LedgerPath $ledger -Event $e1)
    $before = (Get-Content -Raw $ledger)
    [void](Add-HandclapEvent -LedgerPath $ledger -Event $e2)
    $lines = @(Get-Content $ledger)
    Assert-Equal $lines.Count 2 'append must retain prior event'
    Assert-True ($lines[0] -eq $before.TrimEnd("`r", "`n")) 'append overwrote prior line'
    Assert-Equal (@(Get-HandclapEvents -LedgerPath $ledger).Count) 2 'read count'
    $readEvents = @(Read-HandclapLedger -LedgerPath $ledger)
    Assert-Equal $readEvents[0].timestampUtc.GetType().FullName 'System.String' 'read timestampUtc type'
    Assert-Equal $readEvents[0].timestampUtc $t0 'read timestampUtc value'
    Assert-Equal (@(Get-HandclapEvents -LedgerPath $ledger -Kind TRANSFERRED).Count) 1 'kind query'
    Assert-Equal (@(Get-HandclapEvents -LedgerPath $ledger -From $t1 -To $t1).Count) 1 'time query'
    $queryBefore = (Get-Content -Raw $ledger)
    [void](Get-HandclapEvents -LedgerPath $ledger -EventId evt-1)
    Assert-Equal (Get-Content -Raw $ledger) $queryBefore 'query mutated ledger'
    Add-Content -LiteralPath $ledger -Value '{bad json}'
    try { [void](Get-HandclapEvents -LedgerPath $ledger); throw 'bad JSON was hidden' } catch { Assert-True ($_.Exception.Message -match 'JSON|ledger') 'bad JSON error missing' }
    $source = Get-Content -Raw (Join-Path $PSScriptRoot '..\ledger\HandclapLedger.ps1')
    $querySource = Get-Content -Raw (Join-Path $PSScriptRoot '..\ledger\HandclapQuery.ps1')
    $forbidden = '(?i)(?<![A-Za-z0-9_.-])git(?:\.exe)?(?![A-Za-z0-9_.-])|state\.json|task-registry|work-release|candidate|ownership-lock'
    Assert-True ($source -notmatch $forbidden -and $querySource -notmatch $forbidden) 'forbidden dependency found'
    Assert-True (-not (Test-Path (Join-Path $root '.git\xye-handclap\events.jsonl'))) 'formal ledger polluted'
    'LEDGER SELFTEST PASS'
} finally { if (Test-Path $ledger) { Remove-Item -LiteralPath $ledger -Force } }
