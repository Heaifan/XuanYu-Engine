$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$script = Join-Path $root 'handclap.ps1'
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('handclap-cli-' + [guid]::NewGuid())

function Run([string]$EntryPoint, [string[]]$CliArgs) {
    $process = $null
    try {
        $start = [Diagnostics.ProcessStartInfo]::new()
        $start.FileName = (Get-Command pwsh.exe -ErrorAction Stop).Source
        $start.UseShellExecute = $false
        $start.RedirectStandardOutput = $true
        $start.RedirectStandardError = $true
        $start.Arguments = (@('-NoLogo', '-NoProfile', '-File', $EntryPoint) + $CliArgs |
            ForEach-Object { '"' + $_.Replace('"', '\"') + '"' }) -join ' '
        $process = [Diagnostics.Process]::new()
        $process.StartInfo = $start
        if (-not $process.Start()) { throw 'PowerShell child process did not start.' }
        $stdoutTask = $process.StandardOutput.ReadToEndAsync()
        $stderrTask = $process.StandardError.ReadToEndAsync()
        $process.WaitForExit()
        $stdout = $stdoutTask.GetAwaiter().GetResult()
        $stderr = $stderrTask.GetAwaiter().GetResult()
        [pscustomobject]@{
            Code = $process.ExitCode
            Stdout = $stdout
            Stderr = $stderr
            StderrDiagnostic = $stderr
            ExpectedRejection = ($process.ExitCode -eq 1 -and $stderr -match 'HANDOFF_COMMAND_RETIRED')
            ProcessCrashed = ($process.ExitCode -ne 0 -and $stderr -notmatch 'HANDOFF_COMMAND_RETIRED|UNKNOWN_COMMAND')
            HarnessException = $null
        }
    } catch {
        [pscustomobject]@{
            Code = $null
            Stdout = ''
            Stderr = ''
            StderrDiagnostic = ''
            ExpectedRejection = $false
            ProcessCrashed = $false
            HarnessException = $_.Exception.Message
        }
    } finally { if ($process) { $process.Dispose() } }
}
function Need([bool]$Ok, [string]$Message) { if (!$Ok) { throw "CLI SELFTEST FAILED: $Message" } }

try {
    New-Item -ItemType Directory -Force (Join-Path $tmp '.git/xye-handoff') | Out-Null
    $help = Run $script @('help')
    Need ($null -eq $help.HarnessException -and $help.Code -eq 0 -and $help.Stdout -match 'event.*history.*ack.*context') 'help contract'
    foreach ($name in @('event','history','ack','context')) {
        $args = @($name, '-RepositoryRoot', $tmp)
        if ($name -eq 'event') { $args += @('-Kind','COMMENTED','-Actor','test','-Scope','governance') }
        if ($name -eq 'ack') { $args += @('-Actor','test','-Scope','governance','-EventId','e1') }
        if ($name -eq 'context') { $args += @('-Actor','test','-Scope','governance','-Target','next') }
        $r = Run $script $args
        Need ($null -eq $r.HarnessException -and $r.Code -eq 0 -and !$r.ProcessCrashed) "$name command failed: $($r.Stderr)"
    }
    $history = Run $script @('history','-RepositoryRoot',$tmp)
    Need ($history.Code -eq 0 -and $history.Stdout -match 'COMMENTED|ACK|TRANSFERRED') 'ledger query'
    $handoff = Join-Path $root '..\handoff\handoff.ps1'
    $bad = Run $handoff @('prepare','-RepositoryRoot',$tmp)
    Need ($null -eq $bad.HarnessException -and $bad.Code -eq 1 -and $bad.ExpectedRejection -and
        !$bad.ProcessCrashed -and $bad.StderrDiagnostic -match 'HANDOFF_COMMAND_RETIRED: prepare') "retired command rejection code=$($bad.Code) stderr=$($bad.StderrDiagnostic)"
    'HANDCLAP CLI SELFTEST PASS 6/6'
} finally { if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force } }
