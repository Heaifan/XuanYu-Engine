[CmdletBinding()]
param([string]$RepositoryRoot=(Get-Location).Path,[string]$RuntimeRoot='',[string]$ManifestPath='')
$ErrorActionPreference='Stop'
if(!$RuntimeRoot){$RuntimeRoot=Join-Path $RepositoryRoot '.xyt\runtime'}
if(!$ManifestPath){$ManifestPath=Join-Path $RepositoryRoot 'XYT\Runtime\xyt-runtime.manifest.json'}
$manifest=Get-Content -Raw -LiteralPath $ManifestPath|ConvertFrom-Json
if(!(Test-Path -LiteralPath $RuntimeRoot -PathType Container)){Write-Output 'RUNTIME ARTIFACT GATE: PASS';exit 0}
$unknown=[Collections.Generic.List[string]]::new();$known=[Collections.Generic.List[object]]::new()
foreach($file in Get-ChildItem -LiteralPath $RuntimeRoot -File -Recurse){$relative=$file.FullName.Substring((Resolve-Path $RuntimeRoot).Path.Length+1).Replace('\','/');$entry=@($manifest.artifacts|?{$relative -match $_.pattern}|select -First 1);if($entry){[void]$known.Add([pscustomobject]@{Path=$relative;Class=$entry.class})}else{[void]$unknown.Add($relative)}}
Write-Output "RUNTIME ARTIFACT GATE: $(if($unknown.Count){'BLOCKED'}else{'PASS'})";Write-Output "REGISTERED=$($known.Count) UNKNOWN=$($unknown.Count)";if($known.Count){$known|Sort-Object Path|Format-Table -AutoSize|Out-String|Write-Output};if($unknown.Count){Write-Output 'UNKNOWN FILES:';$unknown|Sort-Object|ForEach-Object{Write-Output "- $_"};exit 2};exit 0
