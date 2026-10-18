$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$root=Join-Path $env:TEMP ('XYC-P2-A-R1-candidate-'+[Guid]::NewGuid().ToString('N'))
$zip=Join-Path $root 'base.zip';$snapshot=Join-Path $root 'source'
New-Item -ItemType Directory -Force -Path $root|Out-Null
& git -C $repo archive --format=zip --output=$zip HEAD
if($LASTEXITCODE -ne 0){throw 'git archive failed'}
Expand-Archive -LiteralPath $zip -DestinationPath $snapshot
$owned=@(
'XuanYu.Editor.UI/Control/XycCommandProcessor.cs',
'XuanYu.Editor.UI/Control/XycSceneContracts.cs',
'XuanYu.Editor.UI/Control/XycSceneCommandProcessor.cs',
'XuanYu.Editor.UI/Control/UiVmXycRuntime.Scene.cs',
'XuanYu.Editor.UI/Vm/Control/UiVm.XycScene.cs',
'XuanYu.Editor.UI/Vm/Control/UiVm.XycScene.Guards.cs',
'XuanYu.World.Tests/Control/FakeRuntime.cs',
'XuanYu.World.Tests/Control/XycSceneCommandTests.cs')
foreach($relative in $owned){$destination=Join-Path $snapshot $relative;New-Item -ItemType Directory -Force -Path (Split-Path $destination)|Out-Null;Copy-Item -LiteralPath (Join-Path $repo $relative) -Destination $destination}
Push-Location $snapshot
try{
 $builds=@();foreach($project in @('XuanYu.Editor.Control.Cli/XuanYu.Editor.Control.Cli.csproj','XuanYu.Editor.App/XuanYu.Editor.App.csproj')){& powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\scripts\xye-dotnet.ps1' build $project -c Release;if($LASTEXITCODE -ne 0){throw "BUILD_FAIL $project"};$builds+=$project}
 & powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\scripts\xye-dotnet.ps1' test 'XuanYu.World.Tests/XuanYu.World.Tests.csproj' -c Release --filter 'FullyQualifiedName~XycSceneCommandTests|FullyQualifiedName~XycCommandProcessorTests|FullyQualifiedName~XycDollyCommandTests|FullyQualifiedName~XycCaptureCommandTests|FullyQualifiedName~XycUiControlCommandTests|FullyQualifiedName~XycUiCommandProcessorTests|FullyQualifiedName~XycUiCaptureCommandProcessorTests';if($LASTEXITCODE -ne 0){throw 'XYC_TESTS_FAIL'}
}finally{Pop-Location}
$identity=[pscustomobject]@{baseHead=(git -C $repo rev-parse HEAD).Trim();snapshotPath=$snapshot;ownedFiles=$owned;sourceHashes=@($owned|%{[pscustomobject]@{path=$_;sha256=(Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path $snapshot $_)).Hash}});buildProjects=$builds;candidateDirty=$false}
$identity|ConvertTo-Json -Depth 8|Set-Content -LiteralPath (Join-Path $repo 'docs/qa/XYC-P2-A-R1/candidate-snapshot.json') -Encoding utf8
Write-Output "CANDIDATE_SNAPSHOT=$snapshot"
