param([string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path)

$ErrorActionPreference = 'Stop'
$tree = (& git -C $Root rev-parse 'HEAD^{tree}').Trim()
$paths = @(& git -C $Root diff --name-only HEAD) +
    @(& git -C $Root ls-files --others --exclude-standard -- '*.cs' '*.csproj' '*.slnx' '*.props' '*.targets' '*.ps1')
$extensions = @('.cs', '.csproj', '.slnx', '.props', '.targets', '.ps1')
$material = [Text.StringBuilder]::new()
foreach ($relative in ($paths | Sort-Object -Unique)) {
    if ([IO.Path]::GetExtension($relative) -notin $extensions) { continue }
    $path = Join-Path $Root $relative
    $hash = if (Test-Path -LiteralPath $path -PathType Leaf) {
        (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
    } else { 'DELETED' }
    [void]$material.AppendLine("$relative|$hash")
}
$content = [Text.Encoding]::UTF8.GetBytes("$tree`n$material")
$fingerprint = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($content))
Write-Output "git-tree-sha1:$tree;worktree-source-sha256:$fingerprint"
