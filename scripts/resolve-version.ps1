[CmdletBinding()]
param()

$propsPath = Join-Path $PSScriptRoot '..\Directory.Build.props'
$document = [xml](Get-Content -Raw $propsPath)
$version = $document.Project.PropertyGroup.InformationalVersion | Select-Object -First 1
if ($null -eq $version -or [string]::IsNullOrWhiteSpace([string]$version)) {
    throw 'InformationalVersion is missing from Directory.Build.props.'
}
[string]$version.Trim()
