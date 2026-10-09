function Invoke-XytReportTest([string]$RepositoryRoot, [string]$TestCommand, [string]$OutputRoot, [string]$TestMode, [string]$TestSetVersion, [string[]]$AffectedCapability, [string]$Timestamp) {
    Assert-XytReportText 'TestCommand' $TestCommand
    $root=Get-XytReportRoot $RepositoryRoot $OutputRoot; $evidenceRoot=Join-Path $root 'evidence'
    New-Item -ItemType Directory -Path $evidenceRoot -Force|Out-Null
    $name="{0:yyyyMMddTHHmmssZ}-{1}.log" -f (ConvertTo-XytReportUtc $Timestamp),([guid]::NewGuid().ToString('N').Substring(0,8))
    $evidence=Join-Path $evidenceRoot $name; $output=@(& cmd.exe /d /c $TestCommand 2>&1); $code=$LASTEXITCODE
    $output|Set-Content -LiteralPath $evidence -Encoding UTF8
    $status=if($code -eq 0){'PASS'}else{'FAIL'}
    Invoke-XytReport -Operation Report -RepositoryRoot $RepositoryRoot -OutputRoot $OutputRoot -TestMode $TestMode -TestSetVersion $TestSetVersion -AffectedCapability $AffectedCapability -Status $status -Evidence @($evidence) -Timestamp $Timestamp
}
