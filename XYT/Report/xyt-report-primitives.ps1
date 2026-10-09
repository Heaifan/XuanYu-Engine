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
    [pscustomobject]@{ Version=$version; Commit=$commit; Branch=$branch; Dirty=$dirty }
}

function ConvertTo-XytReportUtc([string]$Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { return [DateTime]::UtcNow }
    ([DateTimeOffset]::Parse($Value, [Globalization.CultureInfo]::InvariantCulture, [Globalization.DateTimeStyles]::AssumeUniversal)).UtcDateTime
}

function Get-XytReportRoot([string]$RepositoryRoot, [string]$Root) {
    if ([string]::IsNullOrWhiteSpace($Root)) { return (Join-Path $RepositoryRoot 'docs\reports\xyt') }
    [IO.Path]::GetFullPath($Root)
}
