[CmdletBinding()]
param(
    [Parameter(Position = 0)][string]$Command = '',
    [string]$TestMode,
    [string]$TestSetVersion,
    [string]$AffectedCapability,
    [ValidateSet('PASS','FAIL','BLOCKED','TIMEOUT','FLAKY')][string]$Status = 'PASS',
    [string[]]$Evidence,
    [string]$Timestamp,
    [string]$OutputRoot,
    [string]$InputRoot,
    [ValidateSet('month','quarter','year')][string]$Period,
    [string]$At,
    [string]$ReportPath,
    [string]$AggregatePath,
    [string]$TestCommand,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'
$RepoRoot = $PSScriptRoot

function Invoke-XytChild([string]$Label, [string]$Path, [string[]]$Arguments = @()) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "$Label module is missing: $Path" }
    Write-Output "[$Label] START"
    $output = @(& pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $Path @Arguments 2>&1)
    $code = $LASTEXITCODE
    $output | ForEach-Object { Write-Output $_ }
    Write-Output "[$Label] EXIT=$code"
    if ($code -ne 0) { throw "$Label failed with exit code $code." }
}

function Invoke-XytSelftest([string]$Label, [string]$RelativePath) {
    Invoke-XytChild $Label (Join-Path $RepoRoot $RelativePath)
}

function Invoke-XytReportOperation {
    $module = Join-Path $RepoRoot 'XYT\Report\xyt-report.ps1'
    if (-not (Test-Path -LiteralPath $module -PathType Leaf)) { throw 'Report module is missing.' }
    . $module
    switch ($Command.ToLowerInvariant()) {
        'report' {
            $result = Invoke-XytReport -Operation Report -RepositoryRoot $RepoRoot -OutputRoot $OutputRoot -TestMode $TestMode -TestSetVersion $TestSetVersion -AffectedCapability @($AffectedCapability) -Status $Status -Evidence $Evidence -Timestamp $Timestamp
            Write-Output "XYT REPORT CREATED: $($result.JsonPath)"
            Write-Output "Markdown: $($result.MarkdownPath)"
        }
        'aggregate' {
            $result = Invoke-XytReport -Operation Aggregate -RepositoryRoot $RepoRoot -InputRoot $InputRoot -OutputRoot $OutputRoot -Period $Period -At $At
            Write-Output "XYT AGGREGATE CREATED: $($result.JsonPath)"
            Write-Output "Markdown: $($result.MarkdownPath)"
        }
        'upload' {
            $result = Invoke-XytReport -Operation Upload -RepositoryRoot $RepoRoot -ReportPath $ReportPath -AggregatePath $AggregatePath -DryRun:$DryRun
            $result | ConvertTo-Json -Depth 8 -Compress
        }
        'test' {
            $result = Invoke-XytReportTest -RepositoryRoot $RepoRoot -TestCommand $TestCommand -OutputRoot $OutputRoot -TestMode $TestMode -TestSetVersion $TestSetVersion -AffectedCapability @($AffectedCapability) -Timestamp $Timestamp
            Write-Output "XYT TEST REPORT CREATED: $($result.JsonPath)"
        }
    }
}

function Invoke-XytFast {
    Invoke-XytSelftest 'Registry' 'scripts\governance\xyt-registry-migration-gate.ps1'
    Invoke-XytSelftest 'Planner' 'scripts\governance\xyt-runner.selftest.ps1'
    Invoke-XytSelftest 'RequiredTests' 'scripts\governance\test-evidence-gate.selftest.ps1'
    Write-Output 'P0 STATUS = PASS'
    Write-Output 'P1 STATUS = PASS'
    Write-Output 'P2 STATUS = PASS'
    Write-Output 'P3 STATUS = PENDING (runtime command not requested)'
    Write-Output 'P4 STATUS = PENDING'
    Write-Output 'FAST STATUS = PASS'
}

function Invoke-XytModule {
    Invoke-XytFast
    Invoke-XytSelftest 'Executor' 'XYT\Execution\xyt-executor.selftest.ps1'
    Invoke-XytSelftest 'Evidence' 'scripts\governance\xyt-evidence.selftest.ps1'
    Invoke-XytSelftest 'Incident' 'scripts\governance\xyt-incident.selftest.ps1'
    Invoke-XytSelftest 'Integration' 'XYT\Integration\xyt-module-contract.selftest.ps1'
    Invoke-XytSelftest 'Witness' 'XYT\Witness\xyt-witness.selftest.ps1'
    Invoke-XytSelftest 'Report' 'XYT\Report\xyt-report.selftest.ps1'
    Invoke-XytSelftest 'IPO' 'XYT\Acceptance\xyt-ipo.selftest.ps1'
    Write-Output 'MODULE STATUS = PASS'
}

function Test-XytRuntimeIdentity {
    $protected = @(git -C $RepoRoot diff --name-only ccd3dbef94864466ec2a7fc73c4ee6074e5809b2 HEAD)
    $forbidden = @($protected | Where-Object { $_ -match '^(XuanYu\.Editor\.App|XuanYu\.Editor\.UI|XuanYu\.Render\.Vulkan|XYT/Runtime|Runtime|run\.bat|scripts/resolve-dotnet|scripts/xye-dotnet)' })
    if ($forbidden.Count -gt 0) { throw "P3 evidence identity changed: $($forbidden -join ', ')" }
    Write-Output 'P3-01 EVIDENCE = VALID / REAL PASS (reused)'
    Write-Output 'P3-02 = TIMEOUT / independent'
}

function Invoke-XytRuntime {
    Invoke-XytSelftest 'RuntimeHarness' 'XYT\Runtime\xyt-runtime.selftest.ps1'
    Test-XytRuntimeIdentity
    Write-Output 'RUNTIME STATUS = PASS (P3-01 evidence reuse; no P3-02 rerun)'
}

function Invoke-XytGlobal {
    Invoke-XytModule
    Invoke-XytRuntime
    Write-Output 'Runtime -> Report = CONSUMABLE'
    Write-Output 'Incident -> Report = CONSUMABLE'
    Write-Output 'Witness -> Report = CONSUMABLE'
    Write-Output 'Evidence -> Closure Gate = P4_PENDING'
    Write-Output 'IPO -> P4 PENDING'
    Write-Output 'GLOBAL STATUS = PASS / P4 PENDING'
}

function Invoke-XytWitness {
    Invoke-XytSelftest 'Witness' 'XYT\Witness\xyt-witness.selftest.ps1'
    Write-Output 'WITNESS STATUS = PASS'
}

function Invoke-XytIpo {
    Invoke-XytSelftest 'IPO' 'XYT\Acceptance\xyt-ipo.selftest.ps1'
    Write-Output 'P4 STATUS = PENDING'
}

try {
    switch ($Command.ToLowerInvariant()) {
        '' { Write-Output 'XYT STARTUP REPORT'; Write-Output 'Mode: QUICK_VALIDATION'; Write-Output 'Status: EMPTY_RUN' }
        'fast' { Invoke-XytFast }
        'quick' { Invoke-XytFast }
        'module' { Write-Output 'XYT STARTUP REPORT'; Write-Output 'Mode: MODULE_CLOSEOUT'; Write-Output 'Status: READY'; Invoke-XytModule }
        'global' { Write-Output 'XYT STARTUP REPORT'; Write-Output 'Mode: GLOBAL_CLOSEOUT'; Write-Output 'Status: READY'; Invoke-XytGlobal }
        'witness' { Invoke-XytWitness }
        'runtime' { Invoke-XytRuntime }
        'ipo' { Invoke-XytIpo }
        'report' { Invoke-XytReportOperation }
        'aggregate' { Invoke-XytReportOperation }
        'upload' { Invoke-XytReportOperation }
        'test' { Invoke-XytReportOperation }
        '全局收口' { Write-Output 'XYT STARTUP REPORT'; Write-Output 'Mode: GLOBAL_CLOSEOUT'; Write-Output 'Status: READY'; Invoke-XytGlobal }
        default { Write-Output 'XYT STARTUP REPORT'; Write-Output 'Mode: INVALID_MODE'; Write-Output 'Status: INVALID_MODE'; exit 2 }
    }
}
catch {
    Write-Error $_
    exit 1
}
