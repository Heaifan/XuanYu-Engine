[CmdletBinding()]
param()
. (Join-Path $PSScriptRoot 'xyt-report-primitives.ps1')
. (Join-Path $PSScriptRoot 'xyt-report-files.ps1')
. (Join-Path $PSScriptRoot 'xyt-report-aggregate.ps1')
. (Join-Path $PSScriptRoot 'xyt-report-upload.ps1')
. (Join-Path $PSScriptRoot 'xyt-report-execution.ps1')

function Invoke-XytReport {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)][ValidateSet('Report','Aggregate','Upload')][string]$Operation,
        [Parameter(Mandatory)][string]$RepositoryRoot,
        [string]$OutputRoot, [string]$InputRoot, [ValidateSet('month','quarter','year')][string]$Period, [string]$At,
        [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability,
        [ValidateSet('PASS','FAIL','BLOCKED','TIMEOUT','FLAKY')][string]$Status = 'PASS', [string[]]$Evidence, [string]$Timestamp,
        [string]$ReportPath, [string]$AggregatePath, [switch]$DryRun
    )
    switch ($Operation) {
        'Report' { Write-XytReportFiles $RepositoryRoot $OutputRoot $TestMode $TestSetVersion $AffectedCapability $Status $Evidence $Timestamp }
        'Aggregate' { Write-XytReportAggregate $RepositoryRoot $InputRoot $OutputRoot $Period $At }
        'Upload' { Invoke-XytReportUpload -RepositoryRoot $RepositoryRoot -ReportPath $ReportPath -AggregatePath $AggregatePath -DryRun:$DryRun }
    }
}
