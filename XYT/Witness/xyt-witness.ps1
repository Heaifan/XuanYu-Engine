[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

function Test-XytWitnessText([object]$Value) {
    return -not [string]::IsNullOrWhiteSpace([string]$Value)
}

function Resolve-XytWitness {
    param([Parameter(Mandatory)][psobject]$Record)
    $required = @('WitnessId','BugId','IncidentId','Capability','PreFixCommit','PostFixCommit',
        'TestId','PreFixTestId','PostFixTestId','TestSetVersion','PreFixResult','PostFixResult')
    $missing = @($required | Where-Object { -not (Test-XytWitnessText $Record.$_) })
    $status = 'INCOMPLETE'
    $reason = 'Required identity or result evidence is missing.'
    if ($missing.Count -gt 0) { $reason = 'Missing: ' + ($missing -join ', ') }
    elseif ($Record.PreFixTestId -ne $Record.TestId -or $Record.PostFixTestId -ne $Record.TestId) {
        $status = 'INVALID'; $reason = 'PRE-FIX and POST-FIX do not use the same Test ID.'
    }
    elseif ($Record.PreFixResult -eq 'PASS') {
        $status = 'INVALID'; $reason = 'PRE-FIX unexpectedly passed; RED was not proven.'
    }
    elseif ($Record.PreFixResult -ne 'FAIL') { $reason = 'PRE-FIX result is not FAIL.' }
    elseif ($Record.PostFixResult -eq 'FAIL') { $reason = 'POST-FIX still fails; GREEN was not proven.' }
    elseif ($Record.PostFixResult -eq 'NOT_EXECUTED') {
        if (Test-XytWitnessText $Record.PreFixEvidence) {
            $status = 'RED_CONFIRMED'; $reason = 'PRE-FIX FAIL is evidenced; POST-FIX is not executed.'
        }
        else { $reason = 'PRE-FIX FAIL evidence is missing.' }
    }
    elseif ($Record.PostFixResult -eq 'PASS') {
        if ((Test-XytWitnessText $Record.PreFixEvidence) -and (Test-XytWitnessText $Record.PostFixEvidence)) {
            $status = 'GREEN_CONFIRMED'; $reason = 'Same Test ID is FAIL on PRE-FIX and PASS on POST-FIX.'
        }
        else { $reason = 'Both PRE-FIX and POST-FIX evidence are required.' }
    }
    return [pscustomobject]([ordered]@{
        WitnessId=$Record.WitnessId; BugId=$Record.BugId; IncidentId=$Record.IncidentId
        Capability=$Record.Capability; PreFixCommit=$Record.PreFixCommit; PostFixCommit=$Record.PostFixCommit
        TestId=$Record.TestId; PreFixTestId=$Record.PreFixTestId; PostFixTestId=$Record.PostFixTestId
        TestSetVersion=$Record.TestSetVersion; PreFixResult=$Record.PreFixResult; PostFixResult=$Record.PostFixResult
        PreFixEvidence=$Record.PreFixEvidence; PostFixEvidence=$Record.PostFixEvidence
        Status=$status; Reason=$reason; GeneratedAtUtc=[DateTime]::UtcNow.ToString('o')
    })
}

function ConvertTo-XytWitnessMarkdown([psobject]$Report) {
    $lines = @('# XYT Regression Witness Report','',"- Witness ID: $($Report.WitnessId)",
        "- Bug ID: $($Report.BugId)","- Incident ID: $($Report.IncidentId)",
        "- Capability: $($Report.Capability)","- Test ID: $($Report.TestId)",
        "- Test Set Version: $($Report.TestSetVersion)","- PRE-FIX Commit: $($Report.PreFixCommit)",
        "- POST-FIX Commit: $($Report.PostFixCommit)",'',"- PRE-FIX: $($Report.PreFixResult)",
        "- POST-FIX: $($Report.PostFixResult)","- Status: **$($Report.Status)**", "- Reason: $($Report.Reason)",'',
        '## Evidence','',"- PRE-FIX: $($Report.PreFixEvidence)","- POST-FIX: $($Report.PostFixEvidence)",'')
    return ($lines -join [Environment]::NewLine)
}

function Write-XytWitnessReport {
    param([Parameter(Mandatory)][string]$InputPath, [Parameter(Mandatory)][string]$JsonPath,
        [Parameter(Mandatory)][string]$MarkdownPath)
    if (-not (Test-Path -LiteralPath $InputPath -PathType Leaf)) { throw "Witness input not found: $InputPath" }
    $record = Get-Content -Raw -LiteralPath $InputPath | ConvertFrom-Json
    $report = Resolve-XytWitness -Record $record
    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $JsonPath -Encoding utf8
    ConvertTo-XytWitnessMarkdown $report | Set-Content -LiteralPath $MarkdownPath -Encoding utf8
    return $report
}
