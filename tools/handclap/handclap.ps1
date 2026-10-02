[CmdletBinding()]
param(
    [Parameter(Position = 0)][string]$Command = 'help',
    [string]$RepositoryRoot = (Get-Location).Path,
    [string]$LedgerPath,
    [string]$Kind = 'COMMENTED',
    [string]$Actor = $env:USERNAME,
    [string]$Scope = 'governance',
    [string]$Subject,
    [string]$CorrelationId = ([guid]::NewGuid().ToString('D')),
    [string]$Comment,
    [string]$EventId,
    [string]$Target = 'next-owner',
    [string]$Summary = 'collaboration context transfer',
    [string[]]$References = @('none')
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$core = Join-Path $PSScriptRoot 'core\HandclapEvent.ps1'
$ledger = Join-Path $PSScriptRoot 'ledger\HandclapLedger.ps1'
$query = Join-Path $PSScriptRoot 'ledger\HandclapQuery.ps1'
$ack = Join-Path $PSScriptRoot 'features\HandclapAck.ps1'
$context = Join-Path $PSScriptRoot 'features\HandclapContext.ps1'
. $core
. $ledger
. $query
$script:LedgerWriter = ${function:Add-HandclapEvent}

function Add-HandclapEvent {
    param($Ledger, $Event)
    & $script:LedgerWriter -LedgerPath $Ledger -Event $Event
}
. $ack
. $context

function Get-Ledger { if ($LedgerPath) { return $LedgerPath }
    Join-Path $root '.git\xye-handclap\events.jsonl' }
function Write-Result($Value) { $Value | ConvertTo-Json -Depth 20 -Compress }
function Require-Text([string]$Value, [string]$Name) {
    if ([string]::IsNullOrWhiteSpace($Value)) { throw "$Name is required." }
}

try {
    $path = Get-Ledger
    switch ($Command.ToLowerInvariant()) {
        'help' {
            'Handclap commands: event, history, ack, context, help'
        }
        'event' {
            Require-Text $Actor 'Actor'; Require-Text $Scope 'Scope'; Require-Text $Kind 'Kind'
            $event = New-HandclapEvent -Kind $Kind -Actor $Actor -Scope $Scope `
                -CorrelationId $CorrelationId -Subject $Subject -Data ([pscustomobject]@{ comment = $Comment })
            Write-Result (Add-HandclapEvent -Ledger $path -Event $event)
        }
        'history' {
            @(Read-HandclapLedger -LedgerPath $path) | ForEach-Object { Write-Result $_ }
        }
        'ack' {
            Require-Text $EventId 'EventId'; Require-Text $Actor 'Actor'; Require-Text $Scope 'Scope'
            $targetEvent = [pscustomobject]@{ eventId = $EventId; scope = $Scope; correlationId = $CorrelationId }
            Write-Result (New-HandclapAck -Ledger $path -TargetEvent $targetEvent `
                -Actor $Actor -Comment $Comment)
        }
        'context' {
            Require-Text $Actor 'Actor'; Require-Text $Scope 'Scope'; Require-Text $Target 'Target'
            Write-Result (New-HandclapContextTransfer -Ledger $path -Actor $Actor -Scope $Scope `
                -Subject $Target -CorrelationId $CorrelationId -Summary $Summary -References $References)
        }
        default { throw "UNKNOWN_COMMAND: $Command" }
    }
    exit 0
} catch {
    Write-Error $_.Exception.Message
    exit 1
}
