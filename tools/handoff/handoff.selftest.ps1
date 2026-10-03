[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'handoff.ps1'

function Invoke-Handoff([string]$Command, [string[]]$Arguments = @()) {
    $shell = (Get-Command pwsh.exe -ErrorAction SilentlyContinue).Source
    if ([string]::IsNullOrWhiteSpace($shell)) { $shell = 'powershell.exe' }
    $old = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $output = @(& $shell -NoProfile -ExecutionPolicy Bypass -File $scriptPath $Command @Arguments 2>&1)
        [pscustomobject]@{ ExitCode = $LASTEXITCODE; Text = ($output -join "`n") }
    } finally { $ErrorActionPreference = $old }
}

function Assert-Result($Result, [int]$Code, [string]$Text) {
    if ($Result.ExitCode -ne $Code -or $Result.Text -notmatch [regex]::Escape($Text)) {
        throw "HANDOFF SELFTEST FAILED: expected $Code / $Text; got $($Result.ExitCode) / $($Result.Text)"
    }
}

Assert-Result (Invoke-Handoff 'help') 0 'Handclap commands: event, history, ack, context, help'

foreach ($command in @('prepare', 'join', 'status', 'advance', 'close', 'lane-close',
        'migrate-active', 'lane-state', 'commit-lock', 'commit-unlock', 'maintenance', 'repair')) {
    Assert-Result (Invoke-Handoff $command) 1 'HANDOFF_COMMAND_RETIRED'
}

'HANDOFF ZERO-AUTHORITY SELFTEST PASS'
