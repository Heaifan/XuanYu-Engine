$ErrorActionPreference = 'Stop'

function Add-HandclapContextRecord($Ledger, $Event) {
    if ($null -eq (Get-Command Add-HandclapEvent -CommandType Function -ErrorAction SilentlyContinue)) {
        throw 'Add-HandclapEvent dependency is required.'
    }
    return Add-HandclapEvent -Ledger $Ledger -Event $Event
}

function New-HandclapContextTransfer {
    param(
        [Parameter(Mandatory)]$Ledger,
        [Parameter(Mandatory)][string]$Actor,
        [Parameter(Mandatory)][string]$Scope,
        [Parameter(Mandatory)][string]$Subject,
        [Parameter(Mandatory)][string]$CorrelationId,
        [Parameter(Mandatory)][string]$Summary,
        [Parameter(Mandatory)][string[]]$References
    )
    $data = [pscustomobject]@{ summary = $Summary; references = @($References) }
    return New-HandclapEvent -Kind 'CONTEXT_TRANSFERRED' -Actor $Actor -Scope $Scope `
        -Subject $Subject -CorrelationId $CorrelationId -Data $data |
        ForEach-Object { Add-HandclapContextRecord $Ledger $_ }
}
