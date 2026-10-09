function Write-XytReportFiles([string]$RepositoryRoot, [string]$OutputRoot, [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability, [string]$Status, [string[]]$Evidence, [string]$Timestamp) {
    Assert-XytReportText 'TestMode' $TestMode
    Assert-XytReportText 'TestSetVersion' $TestSetVersion
    Assert-XytReportText 'AffectedCapability' ($AffectedCapability -join ',')
    $identity = Get-XytReportIdentity $RepositoryRoot; $time = ConvertTo-XytReportUtc $Timestamp
    $folder = Join-Path (Get-XytReportRoot $RepositoryRoot $OutputRoot) ('runs\{0:yyyy}\{0:MM}' -f $time)
    New-Item -ItemType Directory -Path $folder -Force | Out-Null
    $reportId = '{0:yyyyMMddTHHmmssZ}-{1}' -f $time, ([guid]::NewGuid().ToString('N').Substring(0, 8))
    $record = [ordered]@{
        Schema='XYT-T-Report.v1'; Kind='Run'; ReportId=$reportId; Timestamp=$time.ToString('o')
        Version=$identity.Version; Commit=$identity.Commit; Branch=$identity.Branch; Dirty=$identity.Dirty
        IdentityClass=$(if ($identity.Dirty) {'DIRTY_RUNTIME_PROBE'} else {'FORMAL'})
        TestMode=$TestMode; TestSetVersion=$TestSetVersion
        AffectedCapabilities=@($AffectedCapability | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
        Status=$Status; Evidence=@($Evidence | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
    }
    $jsonPath=Join-Path $folder ($reportId+'.json'); $markdownPath=Join-Path $folder ($reportId+'.md')
    $record | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8
    @('# XYT T Report','','- ReportId: '+$reportId,'- Version: '+$record.Version,'- Commit: '+$record.Commit,'- Branch: '+$record.Branch,'- Dirty: '+$record.Dirty,'- TestMode: '+$TestMode,'- TestSetVersion: '+$TestSetVersion,'- AffectedCapabilities: '+($record.AffectedCapabilities -join ', '),'- Status: '+$Status,'- Timestamp: '+$record.Timestamp,'','## Evidence','',($record.Evidence -join "`n")) | Set-Content -LiteralPath $markdownPath -Encoding UTF8
    [pscustomobject]@{ ReportId=$reportId; JsonPath=$jsonPath; MarkdownPath=$markdownPath; Record=$record }
}
