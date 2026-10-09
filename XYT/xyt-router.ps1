[CmdletBinding()]
param(
    [Parameter(Position = 0)][string]$Command = '',
    [string]$TestMode,
    [string]$TestSetVersion,
    [string]$AffectedCapability,
    [string]$Status = 'PASS',
    [string[]]$Evidence,
    [string]$Timestamp,
    [string]$OutputRoot,
    [string]$InputRoot,
    [string]$Period,
    [string]$At,
    [string]$ReportPath,
    [string]$AggregatePath,
    [string]$TestCommand,
    [switch]$DryRun
)
$ErrorActionPreference = 'Stop'
$RepoRoot = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'xyt-process.ps1')
. (Join-Path $PSScriptRoot 'xyt-report-route.ps1')
. (Join-Path $PSScriptRoot 'xyt-closeout.ps1')
$arguments = @{}
foreach ($item in $PSBoundParameters.GetEnumerator()) {
    $arguments[$item.Key] = $item.Value
}
$arguments.RepoRoot = $RepoRoot
try {
    switch ($Command.ToLowerInvariant()) {
        '' {
            Write-Output 'XYT STARTUP REPORT'
            Write-Output 'Mode: QUICK_VALIDATION'
            Write-Output 'Status: EMPTY_RUN'
        }
        'fast' { Invoke-XytFast }
        'quick' { Invoke-XytFast }
        'module' {
            Write-Output 'XYT STARTUP REPORT'
            Write-Output 'Mode: MODULE_CLOSEOUT'
            Write-Output 'Status: READY'
            Invoke-XytModule
        }
        'global' {
            Write-Output 'XYT STARTUP REPORT'
            Write-Output 'Mode: GLOBAL_CLOSEOUT'
            Write-Output 'Status: READY'
            Invoke-XytGlobal
        }
        'witness' { Invoke-XytWitness }
        'runtime' { Invoke-XytRuntime }
        'ipo' { Invoke-XytIpo }
        'report' { Invoke-XytReportOperation $arguments }
        'aggregate' { Invoke-XytReportOperation $arguments }
        'upload' { Invoke-XytReportOperation $arguments }
        'test' { Invoke-XytReportOperation $arguments }
        '全局收口' {
            Write-Output 'XYT STARTUP REPORT'
            Write-Output 'Mode: GLOBAL_CLOSEOUT'
            Write-Output 'Status: READY'
            Invoke-XytGlobal
        }
        default {
            Write-Output 'XYT STARTUP REPORT'
            Write-Output 'Mode: INVALID_MODE'
            Write-Output 'Status: INVALID_MODE'
            exit 2
        }
    }
}
catch {
    Write-Error $_
    exit 1
}
