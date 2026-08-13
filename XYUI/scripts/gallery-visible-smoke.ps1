param([int]$WaitSeconds = 4)

$project = Join-Path $PSScriptRoot '..\src\XYUI.Avalonia.Gallery\XYUI.Avalonia.Gallery.csproj'
$project = [IO.Path]::GetFullPath($project)
$args = "run --project `"$project`" --no-build"
$launcher = Start-Process dotnet -ArgumentList $args -PassThru
try {
    Start-Sleep -Seconds $WaitSeconds
    $gallery = Get-Process -Name 'XYUI.Avalonia.Gallery' -ErrorAction SilentlyContinue |
        Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
    if ($null -eq $gallery) { throw 'Gallery window handle is zero.' }
    if (-not $gallery.MainWindowTitle.Contains('XYUI.Avalonia Gallery')) {
        throw "Unexpected Gallery title: $($gallery.MainWindowTitle)"
    }
    if (-not $gallery.Responding) { throw 'Gallery window is not responding.' }
    "PASS: PID=$($gallery.Id), Handle=$($gallery.MainWindowHandle), Title=$($gallery.MainWindowTitle)"
}
finally {
    Get-Process -Name 'XYUI.Avalonia.Gallery' -ErrorAction SilentlyContinue |
        Stop-Process -Force -ErrorAction SilentlyContinue
    if (-not $launcher.HasExited) { Stop-Process -Id $launcher.Id -Force -ErrorAction SilentlyContinue }
}
