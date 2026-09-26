$ErrorActionPreference = 'Stop'
$resolver = Join-Path $PSScriptRoot 'resolve-dotnet.ps1'
$dotnet = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $resolver
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) {
    exit $(if ($LASTEXITCODE -ne 0) { $LASTEXITCODE } else { 1 })
}

$forwarded = [Collections.Generic.List[string]]::new()
for ($index = 0; $index -lt $args.Count; $index++) {
    if ($args[$index] -in @('-m', '-nr', '-p', '-clp') -and $index + 1 -lt $args.Count) {
        $forwarded.Add("$($args[$index]):$($args[$index + 1])")
        $index++
    } else { $forwarded.Add($args[$index]) }
}
$quoted = @($forwarded | ForEach-Object { "'" + $_.Replace("'", "''") + "'" }) -join ' '
$command = "& '" + $dotnet.Replace("'", "''") + "' " + $quoted
Invoke-Expression $command
exit $LASTEXITCODE
