function Get-XytReportWindow([string]$Period, [DateTime]$Anchor) {
    if ($Period -eq 'month') { $start=[DateTime]::new($Anchor.Year,$Anchor.Month,1,0,0,0,[DateTimeKind]::Utc) }
    elseif ($Period -eq 'quarter') { $month=(($Anchor.Month-1)-(($Anchor.Month-1)%3))+1; $start=[DateTime]::new($Anchor.Year,$month,1,0,0,0,[DateTimeKind]::Utc) }
    else { $start=[DateTime]::new($Anchor.Year,1,1,0,0,0,[DateTimeKind]::Utc) }
    $end=if($Period -eq 'month'){$start.AddMonths(1)}elseif($Period -eq 'quarter'){$start.AddMonths(3)}else{$start.AddYears(1)}
    [pscustomobject]@{Start=$start;End=$end}
}

function Read-XytRunReports([string]$InputRoot, [DateTime]$Start, [DateTime]$End) {
    @(Get-ChildItem -LiteralPath $InputRoot -Filter '*.json' -Recurse -File -ErrorAction SilentlyContinue | ForEach-Object {
        try {$item=Get-Content -LiteralPath $_.FullName -Raw|ConvertFrom-Json} catch {return}
        if($item.Kind -eq 'Run'){$time=[DateTime]::Parse($item.Timestamp).ToUniversalTime();if($time -ge $Start -and $time -lt $End){$item}}
    })
}

function Write-XytReportAggregate([string]$RepositoryRoot, [string]$InputRoot, [string]$OutputRoot, [string]$Period, [string]$At) {
    Assert-XytReportText 'Period' $Period; $window=Get-XytReportWindow $Period (ConvertTo-XytReportUtc $At)
    $input=Get-XytReportRoot $RepositoryRoot $InputRoot; $runs=@(Read-XytRunReports $input $window.Start $window.End)
    $summary=[ordered]@{Total=$runs.Count;PASS=@($runs|Where-Object Status -eq 'PASS').Count;FAIL=@($runs|Where-Object Status -eq 'FAIL').Count;BLOCKED=@($runs|Where-Object Status -eq 'BLOCKED').Count;TIMEOUT=@($runs|Where-Object Status -eq 'TIMEOUT').Count;FLAKY=@($runs|Where-Object Status -eq 'FLAKY').Count}
    $label=if($Period -eq 'month'){'{0:yyyy-MM}' -f $window.Start}elseif($Period -eq 'quarter'){'{0}-Q{1}' -f $window.Start.Year,([int](($window.Start.Month-1)/3)+1)}else{'{0:yyyy}' -f $window.Start}
    $folder=Join-Path (Get-XytReportRoot $RepositoryRoot $OutputRoot) 'aggregates'; New-Item -ItemType Directory -Path $folder -Force|Out-Null
    $jsonPath=Join-Path $folder "$label-$Period.json"; $markdownPath=Join-Path $folder "$label-$Period.md"
    $aggregate=[ordered]@{Schema='XYT-T-Aggregate.v1';Kind='Aggregate';Period=$Period;WindowStart=$window.Start.ToString('o');WindowEnd=$window.End.ToString('o');Summary=$summary;Reports=@($runs)}
    $aggregate|ConvertTo-Json -Depth 10|Set-Content -LiteralPath $jsonPath -Encoding UTF8
    @('# XYT Aggregate Report','','- Period: '+$Period,'- WindowStart: '+$aggregate.WindowStart,'- WindowEnd: '+$aggregate.WindowEnd,'- Total: '+$summary.Total,'- PASS: '+$summary.PASS,'- FAIL: '+$summary.FAIL,'- BLOCKED: '+$summary.BLOCKED,'- TIMEOUT: '+$summary.TIMEOUT,'- FLAKY: '+$summary.FLAKY,'','## Reports','',(($runs|ForEach-Object {"- $($_.ReportId) | $($_.Status) | $($_.Commit)"}) -join "`n")) | Set-Content -LiteralPath $markdownPath -Encoding UTF8
    [pscustomobject]@{JsonPath=$jsonPath;MarkdownPath=$markdownPath;Aggregate=$aggregate}
}
