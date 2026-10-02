$ErrorActionPreference = 'Stop'

function Add-HandclapRecord($Ledger, $Event) {
    if ($null -eq (Get-Command Add-HandclapEvent -CommandType Function -ErrorAction SilentlyContinue)) {
        throw 'Add-HandclapEvent dependency is required.'
    }
    return Add-HandclapEvent -Ledger $Ledger -Event $Event
}

function New-HandclapAck {
    param(
        [Parameter(Mandatory)]$Ledger,
        [Parameter(Mandatory)]$TargetEvent,
        [Parameter(Mandatory)][string]$Actor,
        [string]$Comment = $null
    )
    if ($null -eq $TargetEvent.eventId) { throw 'Target event is required.' }
    $data = [pscustomobject]@{ comment = $Comment }
    return New-HandclapEvent -Kind 'ACK_RECORDED' -Actor $Actor -Scope $TargetEvent.scope `
        -Subject $TargetEvent.eventId -CorrelationId $TargetEvent.correlationId -Data $data |
        ForEach-Object { Add-HandclapRecord $Ledger $_ }
}
