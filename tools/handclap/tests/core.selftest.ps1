$ErrorActionPreference = 'Stop'
$core = Join-Path $PSScriptRoot '..\core\HandclapEvent.ps1'
. $core

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "SELFTEST FAILED: $Message" }
}

function Assert-Throws([scriptblock]$Action, [string]$Message) {
    try { & $Action } catch { return }
    throw "SELFTEST FAILED: $Message"
}

$payload = [pscustomobject]@{
    name = 'fact'
    nested = [pscustomobject]@{ value = 'original' }
    external = [pscustomobject]@{ approved = $true }
}
$event = New-HandclapEvent -Kind 'CREATED' -Actor 'test' -Scope 'governance' `
    -CorrelationId 'corr-1' -Subject 'subject-1' -Data $payload
$second = New-HandclapEvent -Kind 'CREATED' -Actor 'test' -Scope 'governance'

Assert-True ($event.eventId -and $event.eventId -ne $second.eventId) 'eventId must be unique'
$parsed = [DateTimeOffset]::Parse($event.timestampUtc)
Assert-True ($parsed.Offset -eq [TimeSpan]::Zero) 'timestampUtc must be UTC'
[void](Assert-HandclapEvent $event)
Assert-Throws { $event.eventId = 'changed' } 'eventId must be read-only'
Assert-True ($event.data.name -eq 'fact' -and $event.data.nested.value -eq 'original') 'payload roundtrip'
$payload.nested.value = 'changed'
Assert-True ($event.data.nested.value -eq 'original') 'payload must be copied'

foreach ($field in @('Kind', 'Actor', 'Scope')) {
    $args = @{ Kind = 'K'; Actor = 'A'; Scope = 'S' }
    $args[$field] = ''
    Assert-Throws { New-HandclapEvent @args } "required field: $field"
}

$forbidden = @('approved', 'released', 'authorized', 'gatePass', 'ownershipGranted')
foreach ($name in $forbidden) {
    $bad = [pscustomobject]@{ schemaVersion = 'Handclap.Event/1'; eventId = 'x'; timestampUtc = $event.timestampUtc; kind = 'K'; actor = 'A'; scope = 'S'; $name = $true }
    Assert-Throws { Assert-HandclapEvent $bad } "forbidden authority field: $name"
}

$source = Get-Content -Raw $core
Assert-True ($source -notmatch '(?im)^\s*function\s+(Set-State|Advance-|Grant-|Release-|Approve-|Lock-)') 'authority API added'
'HANDCLAP CORE SELFTEST PASS 7/7'
