[CmdletBinding()]
param(
    [string]$OutputDirectory = (Join-Path (Get-Location) "terrain-smoke")
)

$ErrorActionPreference = "Stop"
$width = 401
$height = 801
$west = 120.0
$south = 21.5
$cellSize = 0.005
$noData = -9999
$culture = [Globalization.CultureInfo]::InvariantCulture
$ascPath = Join-Path $OutputDirectory "taiwan_synthetic_smoke_401x801.asc"
$prjPath = [IO.Path]::ChangeExtension($ascPath, ".prj")

New-Item -ItemType Directory -Force $OutputDirectory | Out-Null
$writer = [IO.StreamWriter]::new($ascPath, $false, [Text.UTF8Encoding]::new($false))
try {
    $writer.WriteLine("ncols $width")
    $writer.WriteLine("nrows $height")
    $writer.WriteLine(("xllcorner {0}" -f $west.ToString("0.0", $culture)))
    $writer.WriteLine(("yllcorner {0}" -f $south.ToString("0.0", $culture)))
    $writer.WriteLine(("cellsize {0}" -f $cellSize.ToString("0.000", $culture)))
    $writer.WriteLine("NODATA_value $noData")
    for ($row = 0; $row -lt $height; $row++) {
        $latitude = $south + ($row * $cellSize)
        $latitudeRatio = ($latitude - 23.5) / 2.0
        $centerLongitude = 121.0 + (0.12 * [Math]::Sin($latitudeRatio * [Math]::PI))
        $halfWidth = 0.16 + (0.12 * (1.0 - [Math]::Abs($latitudeRatio)))
        $values = [Collections.Generic.List[string]]::new($width)
        for ($column = 0; $column -lt $width; $column++) {
            $longitude = $west + ($column * $cellSize)
            $distance = ($longitude - $centerLongitude) / $halfWidth
            if ([Math]::Abs($latitudeRatio) -gt 1.0 -or [Math]::Abs($distance) -gt 1.0) {
                $values.Add($noData.ToString($culture))
                continue
            }
            $ridge = 2600.0 * [Math]::Pow(1.0 - [Math]::Abs($distance), 0.7)
            $northSouth = 0.65 + (0.35 * [Math]::Cos($latitudeRatio * [Math]::PI / 2.0))
            $variation = 140.0 * [Math]::Sin(($longitude - $west) * 17.0) * [Math]::Cos($latitudeRatio * 4.0)
            $elevation = [Math]::Max(0.0, ($ridge * $northSouth) + $variation)
            $values.Add($elevation.ToString("0.###", $culture))
        }
        $writer.WriteLine([String]::Join(" ", $values))
    }
}
finally { $writer.Dispose() }

$prj = 'GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563]],AUTHORITY["EPSG","4326"]]'
[IO.File]::WriteAllText($prjPath, $prj, [Text.UTF8Encoding]::new($false))
Write-Output "Generated synthetic Taiwan-shaped DEM (not NASA data; not for elevation accuracy acceptance)."
Write-Output "ASC: $ascPath"
Write-Output "PRJ: $prjPath"
