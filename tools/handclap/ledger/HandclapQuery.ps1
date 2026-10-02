[CmdletBinding()]
param()

. (Join-Path $PSScriptRoot 'HandclapLedger.ps1')

function Get-HandclapEvents {
    param(
        [string]$LedgerPath, [string]$EventId, [string]$Kind, [string]$Actor,
        [string]$Scope, [string]$CorrelationId, [datetimeoffset]$From,
        [datetimeoffset]$To
    )
    $events = @(Read-HandclapLedger -LedgerPath $LedgerPath)
    foreach ($event in $events) {
        if ($EventId -and $event.eventId -ne $EventId) { continue }
        if ($Kind -and $event.kind -ne $Kind) { continue }
        if ($Actor -and $event.actor -ne $Actor) { continue }
        if ($Scope -and $event.scope -ne $Scope) { continue }
        if ($CorrelationId -and $event.correlationId -ne $CorrelationId) { continue }
        $at = [datetimeoffset]::MinValue
        if (-not [datetimeoffset]::TryParse([string]$event.timestampUtc, [ref]$at)) {
            throw "Handclap event $($event.eventId) has invalid timestampUtc."
        }
        if ($PSBoundParameters.ContainsKey('From') -and $at -lt $From) { continue }
        if ($PSBoundParameters.ContainsKey('To') -and $at -gt $To) { continue }
        $event
    }
}
