function ConvertTo-HandoffProcessArgument([string]$Value) {
    if ($null -eq $Value -or $Value.Length -eq 0) { return '""' }
    if ($Value -notmatch '[\s"]') { return $Value }
    $escaped = $Value -replace '(\\*)"', '$1$1\"'
    $escaped = $escaped -replace '(\\+)$', '$1$1'
    return '"' + $escaped + '"'
}

function Invoke-HandoffProcess {
    param(
        [Parameter(Mandatory)][string]$FilePath,
        [string[]]$Arguments = @(),
        [string]$WorkingDirectory = $null,
        [int]$TimeoutMilliseconds = 0
    )
    $psi = New-Object Diagnostics.ProcessStartInfo
    $psi.FileName = $FilePath
    $psi.Arguments = (($Arguments | ForEach-Object { ConvertTo-HandoffProcessArgument $_ }) -join ' ')
    if ($WorkingDirectory) { $psi.WorkingDirectory = $WorkingDirectory }
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $process = New-Object Diagnostics.Process
    $process.StartInfo = $psi
    $watch = [Diagnostics.Stopwatch]::StartNew()
    try {
        if (!$process.Start()) { throw "Could not start process: $FilePath" }
        $stdoutTask = $process.StandardOutput.ReadToEndAsync()
        $stderrTask = $process.StandardError.ReadToEndAsync()
        $completed = if ($TimeoutMilliseconds -gt 0) { $process.WaitForExit($TimeoutMilliseconds) } else { $process.WaitForExit(); $true }
        if (!$completed) {
            $process.Kill()
            $process.WaitForExit()
        }
        $stdout = $stdoutTask.Result
        $stderr = $stderrTask.Result
        [pscustomobject]@{
            ExitCode = if ($completed) { $process.ExitCode } else { 124 }
            Stdout = $stdout
            Stderr = $stderr
            Command = $FilePath
            Arguments = $psi.Arguments
            Duration = $watch.Elapsed
        }
    } finally {
        $watch.Stop()
        $process.Dispose()
    }
}
