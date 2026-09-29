[CmdletBinding()]
param()

function Assert-XytReportText([string]$Name, [string]$Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { throw "$Name is required." }
}

function Invoke-XytReportGit([string]$RepositoryRoot, [string[]]$Arguments) {
    $output = & git -C $RepositoryRoot @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) { throw "git $($Arguments -join ' ') failed: $($output -join ' ')" }
    ($output -join "`n").Trim()
}

function Get-XytReportIdentity([string]$RepositoryRoot) {
    $versionPath = Join-Path $RepositoryRoot 'scripts\resolve-version.ps1'
    if (-not (Test-Path -LiteralPath $versionPath)) { throw 'Version resolver is missing.' }
    $version = (& $versionPath).Trim()
    $commit = Invoke-XytReportGit $RepositoryRoot @('rev-parse','HEAD')
    $branch = Invoke-XytReportGit $RepositoryRoot @('branch','--show-current')
    $dirty = -not [string]::IsNullOrWhiteSpace((Invoke-XytReportGit $RepositoryRoot @('status','--porcelain')))
    [pscustomobject]@{ Version = $version; Commit = $commit; Branch = $branch; Dirty = $dirty }
}

function ConvertTo-XytReportUtc([string]$Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { return [DateTime]::UtcNow }
    ([DateTimeOffset]::Parse($Value, [Globalization.CultureInfo]::InvariantCulture, [Globalization.DateTimeStyles]::AssumeUniversal)).UtcDateTime
}

function Get-XytReportRoot([string]$RepositoryRoot, [string]$Root) {
    if ([string]::IsNullOrWhiteSpace($Root)) { return (Join-Path $RepositoryRoot 'docs\reports\xyt') }
    [IO.Path]::GetFullPath($Root)
}

function Write-XytReportFiles([string]$RepositoryRoot, [string]$OutputRoot, [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability, [string]$Status, [string[]]$Evidence, [string]$Timestamp) {
    Assert-XytReportText 'TestMode' $TestMode
    Assert-XytReportText 'TestSetVersion' $TestSetVersion
    Assert-XytReportText 'AffectedCapability' ($AffectedCapability -join ',')
    $identity = Get-XytReportIdentity $RepositoryRoot
    $time = ConvertTo-XytReportUtc $Timestamp
    $folder = Join-Path (Get-XytReportRoot $RepositoryRoot $OutputRoot) ('runs\{0:yyyy}\{0:MM}' -f $time)
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    $reportId = '{0:yyyyMMddTHHmmssZ}-{1}' -f $time, ([guid]::NewGuid().ToString('N').Substring(0, 8))
    $record = [ordered]@{
        Schema = 'XYT-T-Report.v1'; Kind = 'Run'; ReportId = $reportId; Timestamp = $time.ToString('o')
        Version = $identity.Version; Commit = $identity.Commit; Branch = $identity.Branch; Dirty = $identity.Dirty
        IdentityClass = if ($identity.Dirty) { 'DIRTY_RUNTIME_PROBE' } else { 'FORMAL' }
        TestMode = $TestMode; TestSetVersion = $TestSetVersion
        AffectedCapabilities = @($AffectedCapability | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
        Status = $Status; Evidence = @($Evidence | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    }
    $jsonPath = Join-Path $folder ($reportId + '.json')
    $markdownPath = Join-Path $folder ($reportId + '.md')
    $record | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8
    @('# XYT T Report', '', "- ReportId: $reportId", "- Version: $($record.Version)", "- Commit: $($record.Commit)", "- Branch: $($record.Branch)", "- Dirty: $($record.Dirty)", "- TestMode: $TestMode", "- TestSetVersion: $TestSetVersion", "- AffectedCapabilities: $($record.AffectedCapabilities -join ', ')", "- Status: $Status", "- Timestamp: $($record.Timestamp)", '', '## Evidence', '', ($record.Evidence -join "`n")) | Set-Content -LiteralPath $markdownPath -Encoding UTF8
    [pscustomobject]@{ ReportId = $reportId; JsonPath = $jsonPath; MarkdownPath = $markdownPath; Record = $record }
}

function Get-XytReportWindow([string]$Period, [DateTime]$Anchor) {
    if ($Period -eq 'month') { $start = [DateTime]::new($Anchor.Year, $Anchor.Month, 1, 0, 0, 0, [DateTimeKind]::Utc) }
    elseif ($Period -eq 'quarter') { $month = (($Anchor.Month - 1) - (($Anchor.Month - 1) % 3)) + 1; $start = [DateTime]::new($Anchor.Year, $month, 1, 0, 0, 0, [DateTimeKind]::Utc) }
    else { $start = [DateTime]::new($Anchor.Year, 1, 1, 0, 0, 0, [DateTimeKind]::Utc) }
    $end = if ($Period -eq 'month') { $start.AddMonths(1) } elseif ($Period -eq 'quarter') { $start.AddMonths(3) } else { $start.AddYears(1) }
    [pscustomobject]@{ Start = $start; End = $end }
}

function Read-XytRunReports([string]$InputRoot, [DateTime]$Start, [DateTime]$End) {
    @(Get-ChildItem -LiteralPath $InputRoot -Filter '*.json' -Recurse -File -ErrorAction SilentlyContinue | ForEach-Object {
        try { $item = Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json } catch { return }
        if ($item.Kind -eq 'Run') {
            $time = [DateTime]::Parse($item.Timestamp).ToUniversalTime()
            if ($time -ge $Start -and $time -lt $End) { $item }
        }
    })
}

function Write-XytReportAggregate([string]$RepositoryRoot, [string]$InputRoot, [string]$OutputRoot, [string]$Period, [string]$At) {
    Assert-XytReportText 'Period' $Period
    $window = Get-XytReportWindow $Period (ConvertTo-XytReportUtc $At)
    $input = Get-XytReportRoot $RepositoryRoot $InputRoot
    $runs = @(Read-XytRunReports $input $window.Start $window.End)
    $summary = [ordered]@{ Total = $runs.Count; PASS = @($runs | Where-Object Status -eq 'PASS').Count; FAIL = @($runs | Where-Object Status -eq 'FAIL').Count; BLOCKED = @($runs | Where-Object Status -eq 'BLOCKED').Count; TIMEOUT = @($runs | Where-Object Status -eq 'TIMEOUT').Count; FLAKY = @($runs | Where-Object Status -eq 'FLAKY').Count }
    $label = if ($Period -eq 'month') { '{0:yyyy-MM}' -f $window.Start } elseif ($Period -eq 'quarter') { '{0}-Q{1}' -f $window.Start.Year, ([int](($window.Start.Month - 1) / 3) + 1) } else { '{0:yyyy}' -f $window.Start }
    $folder = Join-Path (Get-XytReportRoot $RepositoryRoot $OutputRoot) 'aggregates'; New-Item -ItemType Directory -Path $folder -Force | Out-Null
    $jsonPath = Join-Path $folder "$label-$Period.json"; $markdownPath = Join-Path $folder "$label-$Period.md"
    $aggregate = [ordered]@{ Schema = 'XYT-T-Aggregate.v1'; Kind = 'Aggregate'; Period = $Period; WindowStart = $window.Start.ToString('o'); WindowEnd = $window.End.ToString('o'); Summary = $summary; Reports = @($runs) }
    $aggregate | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $jsonPath -Encoding UTF8
    @('# XYT Aggregate Report', '', "- Period: $Period", "- WindowStart: $($aggregate.WindowStart)", "- WindowEnd: $($aggregate.WindowEnd)", "- Total: $($summary.Total)", "- PASS: $($summary.PASS)", "- FAIL: $($summary.FAIL)", "- BLOCKED: $($summary.BLOCKED)", "- TIMEOUT: $($summary.TIMEOUT)", "- FLAKY: $($summary.FLAKY)", '', '## Reports', '', (($runs | ForEach-Object { "- $($_.ReportId) | $($_.Status) | $($_.Commit)" }) -join "`n")) | Set-Content -LiteralPath $markdownPath -Encoding UTF8
    [pscustomobject]@{ JsonPath = $jsonPath; MarkdownPath = $markdownPath; Aggregate = $aggregate }
}

function Test-XytReportDocument([string]$Path, [bool]$Aggregate) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw 'REPORT UPLOAD BLOCKED: file does not exist.' }
    $record = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    $required = if ($Aggregate) { @('Schema','Kind','Period','WindowStart','WindowEnd','Summary') } else { @('Version','Commit','Branch','TestMode','TestSetVersion','Timestamp','Status') }
    foreach ($field in $required) { Assert-XytReportText $field ([string]$record.$field) }
    if ($Aggregate -and $record.Kind -ne 'Aggregate') { throw 'REPORT UPLOAD BLOCKED: invalid aggregate kind.' }
    if (-not $Aggregate -and $record.Kind -ne 'Run') { throw 'REPORT UPLOAD BLOCKED: invalid report kind.' }
    $record
}

function Invoke-XytReportUpload([string]$RepositoryRoot, [string]$ReportPath, [string]$AggregatePath, [switch]$DryRun) {
    $record = Test-XytReportDocument $ReportPath $false
    if ($AggregatePath) { [void](Test-XytReportDocument $AggregatePath $true) }
    if ($DryRun) { return [pscustomobject]@{ Status = 'PASS'; Mode = 'DRY_RUN'; ReportId = $record.ReportId; ReportPath = $ReportPath } }
    $identity = Get-XytReportIdentity $RepositoryRoot
    if ($identity.Dirty) { throw 'REPORT UPLOAD BLOCKED: clean workspace required.' }
    $paths = @($ReportPath); if ($AggregatePath) { $paths += $AggregatePath }
    $relative = @()
    foreach ($path in $paths) {
        $full = [IO.Path]::GetFullPath($path)
        if (-not $full.StartsWith($RepositoryRoot, [StringComparison]::OrdinalIgnoreCase)) { throw 'REPORT UPLOAD BLOCKED: path is outside repository.' }
        $relative += $full.Substring($RepositoryRoot.Length).TrimStart('\','/')
    }
    & git -C $RepositoryRoot add -- @relative
    & git -C $RepositoryRoot diff --cached --check -- @relative
    if ($LASTEXITCODE -ne 0) { throw 'REPORT UPLOAD BLOCKED: staged report has whitespace errors.' }
    [pscustomobject]@{ Status = 'READY_TO_PUSH'; ReportId = $record.ReportId; Paths = $relative }
}

function Invoke-XytReportTest([string]$RepositoryRoot, [string]$TestCommand, [string]$OutputRoot, [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability, [string]$Timestamp) {
    Assert-XytReportText 'TestCommand' $TestCommand
    $root = Get-XytReportRoot $RepositoryRoot $OutputRoot
    $evidenceRoot = Join-Path $root 'evidence'; New-Item -ItemType Directory -Path $evidenceRoot -Force | Out-Null
    $evidence = Join-Path $evidenceRoot ("{0:yyyyMMddTHHmmssZ}-{1}.log" -f (ConvertTo-XytReportUtc $Timestamp), ([guid]::NewGuid().ToString('N').Substring(0, 8)))
    $output = @(& cmd.exe /d /c $TestCommand 2>&1); $code = $LASTEXITCODE; $output | Set-Content -LiteralPath $evidence -Encoding UTF8
    $status = if ($code -eq 0) { 'PASS' } else { 'FAIL' }
    Invoke-XytReport -Operation Report -RepositoryRoot $RepositoryRoot -OutputRoot $OutputRoot -TestMode $TestMode -TestSetVersion $TestSetVersion -AffectedCapability $AffectedCapability -Status $status -Evidence @($evidence) -Timestamp $Timestamp
}

function Invoke-XytReport {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)][ValidateSet('Report','Aggregate','Upload')][string]$Operation,
        [Parameter(Mandatory)][string]$RepositoryRoot,
        [string]$OutputRoot, [string]$InputRoot, [ValidateSet('month','quarter','year')][string]$Period, [string]$At,
        [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability, [ValidateSet('PASS','FAIL','BLOCKED','TIMEOUT','FLAKY')][string]$Status = 'PASS', [string[]]$Evidence, [string]$Timestamp,
        [string]$ReportPath, [string]$AggregatePath, [switch]$DryRun
    )
    switch ($Operation) {
        'Report' { Write-XytReportFiles $RepositoryRoot $OutputRoot $TestMode $TestSetVersion $AffectedCapability $Status $Evidence $Timestamp }
        'Aggregate' { Write-XytReportAggregate $RepositoryRoot $InputRoot $OutputRoot $Period $At }
        'Upload' { Invoke-XytReportUpload -RepositoryRoot $RepositoryRoot -ReportPath $ReportPath -AggregatePath $AggregatePath -DryRun:$DryRun }
    }
}
