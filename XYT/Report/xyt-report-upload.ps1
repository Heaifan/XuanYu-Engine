function Test-XytReportDocument([string]$Path, [bool]$Aggregate) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw 'REPORT UPLOAD BLOCKED: file does not exist.' }
    $record=Get-Content -LiteralPath $Path -Raw|ConvertFrom-Json
    $required=if($Aggregate){@('Schema','Kind','Period','WindowStart','WindowEnd','Summary')}else{@('Version','Commit','Branch','TestMode','TestSetVersion','Timestamp','Status')}
    foreach($field in $required){Assert-XytReportText $field ([string]$record.$field)}
    if($Aggregate -and $record.Kind -ne 'Aggregate'){throw 'REPORT UPLOAD BLOCKED: invalid aggregate kind.'}
    if(-not $Aggregate -and $record.Kind -ne 'Run'){throw 'REPORT UPLOAD BLOCKED: invalid report kind.'}
    $record
}

function Invoke-XytReportUpload([string]$RepositoryRoot, [string]$ReportPath, [string]$AggregatePath, [switch]$DryRun) {
    if(-not $DryRun){throw 'REPORT UPLOAD BLOCKED: parent-managed publication required.'}
    $record=Test-XytReportDocument $ReportPath $false
    if($AggregatePath){[void](Test-XytReportDocument $AggregatePath $true)}
    [pscustomobject]@{Status='PASS';Mode='DRY_RUN';ReportId=$record.ReportId;ReportPath=$ReportPath}
}
