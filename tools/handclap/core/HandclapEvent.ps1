$ErrorActionPreference = 'Stop'

function New-HandclapEvent {
    param(
        [Parameter(Mandatory)][string]$Kind,
        [Parameter(Mandatory)][string]$Actor,
        [Parameter(Mandatory)][string]$Scope,
        [string]$CorrelationId = $null,
        [string]$Subject = $null,
        $Data = $null
    )
    if ([string]::IsNullOrWhiteSpace($Kind)) { throw 'kind must be non-empty.' }
    if ([string]::IsNullOrWhiteSpace($Actor)) { throw 'actor must be non-empty.' }
    if ([string]::IsNullOrWhiteSpace($Scope)) { throw 'scope must be non-empty.' }
    $record = [pscustomobject]@{}
    $values = [ordered]@{
        schemaVersion = 'Handclap.Event/1'
        eventId = [guid]::NewGuid().ToString('D')
        timestampUtc = [DateTimeOffset]::UtcNow.ToString('o')
        kind = $Kind
        actor = $Actor
        scope = $Scope
        correlationId = $CorrelationId
        subject = $Subject
        data = if ($null -eq $Data) { $null } else { (($Data | ConvertTo-Json -Depth 32 -Compress) | ConvertFrom-Json) }
    }
    foreach ($entry in $values.GetEnumerator()) {
        $captured = $entry.Value
        Add-Member -InputObject $record -MemberType ScriptProperty -Name $entry.Key -Value ({ $captured }.GetNewClosure())
    }
    [void](Assert-HandclapEvent $record)
    return $record
}

function Assert-HandclapEvent($Event) {
    if ($null -eq $Event) { throw 'Event is required.' }
    $required = @('schemaVersion','eventId','timestampUtc','kind','actor','scope','correlationId','subject','data')
    $actual = @($Event.PSObject.Properties.Name)
    if (@($actual | Where-Object { $_ -notin $required }).Count -gt 0 -or $actual.Count -ne $required.Count) {
        throw 'Event schema contains an unknown or missing field.'
    }
    if ($Event.schemaVersion -ne 'Handclap.Event/1') { throw 'Unsupported event schema.' }
    foreach ($name in @('eventId','timestampUtc','kind','actor','scope')) {
        if ([string]::IsNullOrWhiteSpace([string]$Event.$name)) { throw "$name must be non-empty." }
    }
    try { $id = [guid]::Parse([string]$Event.eventId); $time = [DateTimeOffset]::Parse([string]$Event.timestampUtc) }
    catch { throw 'eventId or timestampUtc is invalid.' }
    if ($id -eq [guid]::Empty -or $time.Offset -ne [TimeSpan]::Zero) { throw 'eventId/timestampUtc must be valid UTC values.' }
    foreach ($name in @('approved','released','authorized','gatePass','ownershipGranted')) {
        if ($null -ne $Event.PSObject.Properties[$name]) { throw "Authority field is forbidden: $name" }
    }
    return $true
}
