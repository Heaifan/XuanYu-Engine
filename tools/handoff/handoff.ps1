[CmdletBinding()]
param(
    [Parameter(Position = 0)][string]$Command = 'help',
    [Parameter(Position = 1, ValueFromRemainingArguments = $true)][string[]]$Arguments = @()
)

$ErrorActionPreference = 'Stop'
$retired = @(
    'prepare', 'join', 'status', 'advance', 'close', 'lane-close',
    'migrate-active', 'lane-state', 'commit-lock', 'commit-unlock',
    'maintenance', 'repair'
)
$allowed = @('event', 'history', 'ack', 'context', 'help')
$name = $Command.ToLowerInvariant()
if ($retired -contains $name -or $allowed -notcontains $name) {
    Write-Error "HANDOFF_COMMAND_RETIRED: $Command"
    exit 1
}

$target = Join-Path $PSScriptRoot '..\handclap\handclap.ps1'
if (-not (Test-Path -LiteralPath $target -PathType Leaf)) {
    Write-Error 'HANDCLAP_ENTRY_MISSING'
    exit 1
}

$hostExe = if (Get-Command pwsh.exe -ErrorAction SilentlyContinue) { 'pwsh.exe' } else { 'powershell.exe' }
& $hostExe -NoLogo -NoProfile -ExecutionPolicy Bypass -File $target $name @Arguments
exit $LASTEXITCODE
