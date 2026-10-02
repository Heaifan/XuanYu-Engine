$ErrorActionPreference = 'Stop'
$core = Join-Path $PSScriptRoot '..\core\HandclapEvent.ps1'
$ack = Join-Path $PSScriptRoot '..\features\HandclapAck.ps1'
$context = Join-Path $PSScriptRoot '..\features\HandclapContext.ps1'
. $core

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "SELFTEST FAILED: $Message" }
}

$ledger = [System.Collections.Generic.List[object]]::new()
function Add-HandclapEvent {
    param($Ledger, $Event)
    $Ledger.Add($Event)
    return $Event
}

. $ack
. $context
$target = New-HandclapEvent -Kind 'FACT_RECORDED' -Actor 'A' -Scope 'governance'
$first = New-HandclapAck -Ledger $ledger -TargetEvent $target -Actor 'B' -Comment 'read'
$second = New-HandclapAck -Ledger $ledger -TargetEvent $target -Actor 'C'
Assert-True ($first.kind -eq 'ACK_RECORDED' -and $first.subject -eq $target.eventId) 'ACK binding'
Assert-True ($first.actor -eq 'B' -and $first.data.comment -eq 'read') 'ACK actor/comment'
Assert-True ($ledger.Count -eq 2 -and $second.eventId -ne $first.eventId) 'ACK history'

$transfer = New-HandclapContextTransfer -Ledger $ledger -Actor 'A' -Scope 'governance' `
    -Subject 'background' -CorrelationId 'corr-1' -Summary 'shared fact' -References @('E1','E2')
Assert-True ($transfer.kind -eq 'CONTEXT_TRANSFERRED') 'context kind'
Assert-True ($transfer.data.summary -eq 'shared fact' -and $transfer.data.references.Count -eq 2) 'context payload'
Assert-True ($transfer.actor -eq 'A' -and $transfer.scope -eq 'governance' -and $transfer.subject -eq 'background') 'context fields'
foreach ($event in $ledger) {
    $authorityFields = @('approved','released','authority','authorized','gatePass','ownershipGranted') |
        ? { $event.PSObject.Properties.Name -contains $_ }
    Assert-True (@($authorityFields).Count -eq 0) 'authority fields'
}

foreach ($file in @($ack,$context)) {
    Assert-True ((Get-Content $file).Count -le 100) "5+100: $file"
    $source = Get-Content -Raw $file
    Assert-True ($source -notmatch '(?i)state\.json|task-registry|work-release|ownership-locks|candidate\.json|git\s|gate\s|workspace') "forbidden dependency: $file"
}
'HANDCLAP ACK-CONTEXT SELFTEST PASS 8/8'
