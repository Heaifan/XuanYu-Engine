$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$script = Join-Path $root 'handclap.ps1'
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('handclap-cli-' + [guid]::NewGuid())

function Run([string[]]$CliArgs) {
    $previous = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $out = @(& pwsh -NoLogo -NoProfile -File $script @CliArgs 2>&1)
        $code = $LASTEXITCODE
    } finally {
        $ErrorActionPreference = $previous
    }
    [pscustomobject]@{ Code = $code; Text = ($out -join "`n") }
}
function Need([bool]$Ok, [string]$Message) { if (!$Ok) { throw "CLI SELFTEST FAILED: $Message" } }

try {
    New-Item -ItemType Directory -Force (Join-Path $tmp '.git/xye-handoff') | Out-Null
    $help = Run @('help')
    Need ($help.Code -eq 0 -and $help.Text -match 'event.*history.*ack.*context') 'help contract'
    foreach ($name in @('event','history','ack','context')) {
        $args = @($name, '-RepositoryRoot', $tmp)
        if ($name -eq 'event') { $args += @('-Kind','COMMENTED','-Actor','test','-Scope','governance') }
        if ($name -eq 'ack') { $args += @('-Actor','test','-Scope','governance','-EventId','e1') }
        if ($name -eq 'context') { $args += @('-Actor','test','-Scope','governance','-Target','next') }
        $r = Run $args
        Need ($r.Code -eq 0) "$name command failed: $($r.Text)"
    }
    $history = Run @('history','-RepositoryRoot',$tmp)
    Need ($history.Code -eq 0 -and $history.Text -match 'COMMENTED|ACK|TRANSFERRED') 'ledger query'
    $bad = Run @('prepare','-RepositoryRoot',$tmp)
    Need ($bad.Code -ne 0 -and $bad.Text -match 'UNKNOWN_COMMAND|NOT_SUPPORTED') "forbidden command rejection code=$($bad.Code) text=$($bad.Text)"
    'HANDCLAP CLI SELFTEST PASS 6/6'
} finally { if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force } }
