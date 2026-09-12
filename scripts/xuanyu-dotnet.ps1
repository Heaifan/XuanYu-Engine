$ErrorActionPreference = 'Stop'
$resolver = Join-Path $PSScriptRoot 'resolve-dotnet.ps1'
$shell = (Get-Process -Id $PID).Path
$dotnet = & $shell -NoProfile -ExecutionPolicy Bypass -File $resolver
if ($LASTEXITCODE -ne 0 -or @($dotnet).Count -ne 1) { exit 1 }
$valueOptions = @('-m', '-nr', '-p', '-property', '-clp', '-verbosity', '-v', '--framework', '-f', '--configuration', '-c', '--runtime', '-r', '--output', '-o', '--arch', '--os', '--source', '--configfile')
$forwarded = [Collections.Generic.List[string]]::new()
for ($i = 0; $i -lt $args.Count; $i++) {
    $arg = [string]$args[$i]
    if ($valueOptions -contains $arg -and $i + 1 -lt $args.Count -and -not ([string]$args[$i + 1]).StartsWith('-')) {
        $forwarded.Add("${arg}:$($args[$i + 1])")
        $i++
        continue
    }
    $forwarded.Add($arg)
}
& $dotnet $forwarded.ToArray()
exit $LASTEXITCODE
