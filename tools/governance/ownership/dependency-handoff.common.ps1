$ErrorActionPreference = 'Stop'
function Fail([string]$Code, [string]$Message) { throw "${Code}: $Message" }
function StatePath([string]$Root) { Join-Path $Root '.git\xye-handoff\dependency-state.json' }
function Read-State([string]$Root) {
    $path = StatePath $Root
    if (!(Test-Path -LiteralPath $path)) {
        return [pscustomobject]@{ schema = 'XYE-H3/1'; lanes = @(); edges = @(); evidence = @(); files = @(); transfers = @(); waits = @() }
    }
    Get-Content -Raw -LiteralPath $path | ConvertFrom-Json
}
function Write-State([string]$Root, $State) {
    $path = StatePath $Root
    New-Item -ItemType Directory -Force -Path (Split-Path $path) | Out-Null
    $tmp = "$path.$([guid]::NewGuid().ToString('N')).tmp"
    [IO.File]::WriteAllText($tmp, ($State | ConvertTo-Json -Depth 16), (New-Object Text.UTF8Encoding($false)))
    Move-Item -LiteralPath $tmp -Destination $path -Force
}
function Relative-File([string]$Root, [string]$File) {
    $fullRoot = [IO.Path]::GetFullPath($Root).TrimEnd('\') + '\'
    $full = [IO.Path]::GetFullPath($File)
    if (!$full.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) { Fail FILE_OUTSIDE_WORKSPACE $File }
    $full.Substring($fullRoot.Length).Replace('\', '/')
}
function File-Fingerprint([string]$Root, [string]$File) {
    $path = Join-Path $Root ($File.Replace('/', '\'))
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) { Fail FILE_MISSING $File }
    (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()
}
function Find-One($Items, [string]$Property, [string]$Value) {
    @($Items | Where-Object { [string]$_.$Property -eq $Value }) | Select-Object -First 1
}
function Add-Item($Items, $Value) { @($Items) + @($Value) }
