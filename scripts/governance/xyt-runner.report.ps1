function Write-XytResult($Result, $OutputPath) {
    $json = $Result | ConvertTo-Json -Depth 10
    if ($OutputPath) { $json | Set-Content -LiteralPath $OutputPath -Encoding UTF8 }
    else { $json }
    Write-Output "XYT STATUS = $($Result.status)"
    Write-Output "XYT REQUIRED TESTS = $(@($Result.requiredTests).Count)"
    Write-Output "XYT SUPPLEMENTAL TESTS = $(@($Result.supplementalTests).Count)"
    if (@($Result.disputes).Count) { Write-Output ('XYT DISPUTES = ' + (@($Result.disputes) -join '; ')) }
}
