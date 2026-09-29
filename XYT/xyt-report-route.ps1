function Invoke-XytReportOperation([hashtable]$Arguments) {
    $module = Join-Path $RepoRoot 'XYT\Report\xyt-report.ps1'
    if (-not (Test-Path -LiteralPath $module -PathType Leaf)) {
        throw 'Report module is missing.'
    }
    . $module
    switch ($Arguments.Command.ToLowerInvariant()) {
        'report' {
            $result = Invoke-XytReport -Operation Report -RepositoryRoot $RepoRoot -OutputRoot $Arguments.OutputRoot -TestMode $Arguments.TestMode -TestSetVersion $Arguments.TestSetVersion -AffectedCapability @($Arguments.AffectedCapability) -Status $Arguments.Status -Evidence $Arguments.Evidence -Timestamp $Arguments.Timestamp
            Write-Output "XYT REPORT CREATED: $($result.JsonPath)"
            Write-Output "Markdown: $($result.MarkdownPath)"
        }
        'aggregate' {
            $result = Invoke-XytReport -Operation Aggregate -RepositoryRoot $RepoRoot -InputRoot $Arguments.InputRoot -OutputRoot $Arguments.OutputRoot -Period $Arguments.Period -At $Arguments.At
            Write-Output "XYT AGGREGATE CREATED: $($result.JsonPath)"
            Write-Output "Markdown: $($result.MarkdownPath)"
        }
        'upload' {
            $result = Invoke-XytReport -Operation Upload -RepositoryRoot $RepoRoot -ReportPath $Arguments.ReportPath -AggregatePath $Arguments.AggregatePath -DryRun:$Arguments.DryRun
            $result | ConvertTo-Json -Depth 8 -Compress
        }
        'test' {
            $result = Invoke-XytReportTest -RepositoryRoot $RepoRoot -TestCommand $Arguments.TestCommand -OutputRoot $Arguments.OutputRoot -TestMode $Arguments.TestMode -TestSetVersion $Arguments.TestSetVersion -AffectedCapability @($Arguments.AffectedCapability) -Timestamp $Arguments.Timestamp
            Write-Output "XYT TEST REPORT CREATED: $($result.JsonPath)"
        }
    }
}
