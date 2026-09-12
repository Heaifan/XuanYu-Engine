param([switch]$NoFallback)
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Resolve-Candidate([string]$candidate) {
    if ([string]::IsNullOrWhiteSpace($candidate)) { return $null }
    try {
        $item = Get-Item -LiteralPath $candidate -ErrorAction Stop
        if ($item.PSIsContainer) { $candidate = Join-Path $item.FullName 'dotnet.exe' }
        if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) { return $null }
        $sdks = @(& $candidate --list-sdks 2>$null)
        if ($LASTEXITCODE -ne 0 -or @($sdks | Where-Object { $_.ToString().Trim() }).Count -eq 0) { return $null }
        return (Resolve-Path -LiteralPath $candidate).Path
    } catch {
        $null = $_
        return $null
    }
}

$candidates = [Collections.Generic.List[string]]::new()
if ($env:XUANYU_DOTNET) { $candidates.Add($env:XUANYU_DOTNET) }
if (-not $NoFallback) {
    $candidates.Add((Join-Path $repo 'sdk-dotnet\dotnet.exe'))
    $candidates.Add((Join-Path $repo '.dotnet\dotnet.exe'))
    foreach ($drive in 65..90 | ForEach-Object { [char]$_ }) {
        $candidates.Add("${drive}:\MyApp\sdk-dotnet\dotnet.exe")
        $candidates.Add("${drive}:\DevTools\dotnet\dotnet.exe")
    }
    $pathDotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($pathDotnet) { $candidates.Add($pathDotnet.Source) }
}

foreach ($candidate in $candidates | Select-Object -Unique) {
    $resolved = Resolve-Candidate $candidate
    if ($resolved) { Write-Output $resolved; exit 0 }
}

[Console]::Error.WriteLine('[XuanYu] .NET SDK not found.')
exit 1
