param(
    [string]$CliPath = (Resolve-Path 'XuanYu.Editor.Control.Cli/bin/Debug/net10.0/XuanYu.Editor.Control.Cli.exe').Path,
    [string]$AppPath = (Resolve-Path 'XuanYu.Editor.App/bin/Debug/net10.0/XuanYu.Editor.App.exe').Path,
    [string]$EvidenceDir = (Join-Path (Get-Location) 'docs/qa/XYC-P2-A-R1/evidence/runtime-001')
)
$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Force -Path $EvidenceDir | Out-Null
$session = [Guid]::NewGuid().ToString('N')
$start = [Diagnostics.ProcessStartInfo]::new($CliPath)
$start.UseShellExecute = $false
$start.RedirectStandardInput = $true
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $false
$start.ArgumentList.Add('--app'); $start.ArgumentList.Add($AppPath)
$start.ArgumentList.Add('--xyc-ui'); $start.ArgumentList.Add('--diagnostic-write')
$start.ArgumentList.Add('--session-id'); $start.ArgumentList.Add($session)
$start.ArgumentList.Add('--timeout-seconds'); $start.ArgumentList.Add('180')
$start.ArgumentList.Add('--evidence-dir'); $start.ArgumentList.Add($EvidenceDir)
$process = [Diagnostics.Process]::new(); $process.StartInfo = $start
$records = [Collections.Generic.List[object]]::new()
function Send([string]$Command, [hashtable]$Payload = @{}) {
    $request = @{ schemaVersion='XYC-API/1'; requestId=[Guid]::NewGuid().ToString('N'); sessionId=$session; command=$Command; args=$Payload }
    $process.StandardInput.WriteLine(($request | ConvertTo-Json -Depth 12 -Compress))
    $line = $process.StandardOutput.ReadLine()
    if ([string]::IsNullOrWhiteSpace($line)) { throw "No response for $Command; process exited=$($process.HasExited)" }
    $response = $line | ConvertFrom-Json -Depth 20
    $records.Add([pscustomobject]@{ request=$request; response=$response })
    if (-not $response.success) { throw "$Command failed: $($response.errorCode)" }
    return $response
}
function Probe([string]$Command, [hashtable]$Payload, [string]$Expected,
    [string]$RequestId, [string]$RequestSession) {
    if ([string]::IsNullOrWhiteSpace($RequestId)) { $RequestId = [Guid]::NewGuid().ToString('N') }
    if ([string]::IsNullOrWhiteSpace($RequestSession)) { $RequestSession = $session }
    $request = @{ schemaVersion='XYC-API/1'; requestId=$RequestId; sessionId=$RequestSession; command=$Command; args=$Payload }
    $process.StandardInput.WriteLine(($request | ConvertTo-Json -Depth 12 -Compress))
    $response = $process.StandardOutput.ReadLine() | ConvertFrom-Json -Depth 20
    $records.Add([pscustomobject]@{ request=$request; response=$response })
    if ($response.success -or $response.errorCode -ne $Expected) {
        throw "$Command expected $Expected, got $($response.errorCode)"
    }
    return $response
}
try {
    if (-not $process.Start()) { throw 'Editor CLI failed to start' }
    $null = Send 'runtime.info'
    $null = Send 'ui.window.resize' @{ widthDip=1360; heightDip=820 }
    $theme = Send 'ui.theme.inspect'
    if ($theme.observedState.theme -ne 'Light') { $theme = Send 'ui.theme.set' @{ theme='Light' } }
    $captureBefore = Send 'ui.capture' @{ fileName='Light-1360-p2a-before.png'; backend='avaloniaWindow'; timeoutMs=10000 }
    $scene = Send 'scene.inspect'
    $baseline = Send 'scene.test.inspect'
    if ($baseline.observedState.entityCount -ne 0 -or $baseline.observedState.scenePath -ne '') { throw 'Editor scene is not empty and untitled' }
    $sceneId = $baseline.observedState.sceneId
    $revision = [long]$baseline.observedState.sceneRevision
    $negative = [Collections.Generic.List[object]]::new()
    $negative.Add((Probe 'runtime.info' @{} 'UNAUTHORIZED' ([Guid]::NewGuid().ToString('N')) 'wrong-session'))
    $negative.Add((Probe 'scene.entity.inspect' @{ entityId='bad-id' } 'INVALID_ENTITY_ID'))
    $negative.Add((Probe 'scene.entity.inspect' @{ entityId='EntityId(999999)' } 'ENTITY_NOT_FOUND'))
    $negative.Add((Probe 'scene.entity.create-test-cube' @{ sceneId=$sceneId; expectedRevision=($revision - 1) } 'STALE_REVISION'))
    $negative.Add((Probe 'scene.history.undo' @{ sceneId=$sceneId; expectedRevision=$revision; entityId='EntityId(999999)' } 'INVALID_STAGE'))
    $null = Send 'runtime.info'
    $created = Send 'scene.entity.create-test-cube' @{ sceneId=$sceneId; expectedRevision=$revision }
    $entityId = $created.observedState.testEntityId
    if ($created.observedState.hierarchyEntityIds -notcontains $entityId) { throw 'Hierarchy did not contain the new EntityId' }
    if ($created.observedState.renderEntityIds -notcontains $entityId) { throw 'Render snapshot did not contain the new EntityId' }
    if (-not ($created.observedState.inspectorFields | Where-Object { $_.Label -eq '实体编号' -and $_.Value.Contains($entityId.Replace('EntityId(', '实体编号(')) })) { throw 'Inspector projection did not identify the new EntityId' }
    $captureCreated = Send 'ui.capture' @{ fileName='Light-1360-p2a-created.png'; backend='avaloniaWindow'; timeoutMs=10000 }
    $negative.Add((Probe 'scene.entity.create-test-cube' @{ sceneId=$sceneId; expectedRevision=$created.observedState.sceneRevision } 'UNSAFE_SCENE'))
    $negative.Add((Probe 'scene.entity.set-test-position' @{ sceneId=$sceneId; expectedRevision=$created.observedState.sceneRevision; entityId='EntityId(999999)'; position=@{x=0;y=0;z=0} } 'UNAUTHORIZED_ENTITY'))
    $negative.Add((Probe 'scene.entity.set-test-position' @{ sceneId=$sceneId; expectedRevision=$created.observedState.sceneRevision; entityId=$entityId; position=@{x=10001;y=0;z=0} } 'INVALID_INPUT'))
    $entity0 = Send 'scene.entity.inspect' @{ entityId=$entityId }
    $revision = [long]$created.observedState.sceneRevision
    $moved = Send 'scene.entity.set-test-position' @{ sceneId=$sceneId; expectedRevision=$revision; entityId=$entityId; position=@{x=3.5;y=-2.25;z=7.0} }
    $captureMoved = Send 'ui.capture' @{ fileName='Light-1360-p2a-moved.png'; backend='avaloniaWindow'; timeoutMs=10000 }
    $positionField = $moved.observedState.inspectorFields | Where-Object Label -eq '位置'
    if ($positionField.Value -notmatch '3\.5' -or $positionField.Value -notmatch '-2\.25' -or $positionField.Value -notmatch '7') { throw 'Inspector projection did not reflect T1' }
    $entity1 = Send 'scene.entity.inspect' @{ entityId=$entityId }
    $revision = [long]$moved.observedState.sceneRevision
    $undoMove = Send 'scene.history.undo' @{ sceneId=$sceneId; expectedRevision=$revision; entityId=$entityId }
    $afterUndo = Send 'scene.entity.inspect' @{ entityId=$entityId }
    $revision = [long]$undoMove.observedState.sceneRevision
    $redoMove = Send 'scene.history.redo' @{ sceneId=$sceneId; expectedRevision=$revision; entityId=$entityId }
    $afterRedo = Send 'scene.entity.inspect' @{ entityId=$entityId }
    $revision = [long]$redoMove.observedState.sceneRevision
    $undoAgain = Send 'scene.history.undo' @{ sceneId=$sceneId; expectedRevision=$revision; entityId=$entityId }
    $revision = [long]$undoAgain.observedState.sceneRevision
    $undoCreate = Send 'scene.history.undo' @{ sceneId=$sceneId; expectedRevision=$revision; entityId=$entityId }
    $final = Send 'scene.test.inspect'
    $captureRestored = Send 'ui.capture' @{ fileName='Light-1360-p2a-restored.png'; backend='avaloniaWindow'; timeoutMs=10000 }
    if ($final.observedState.entityCount -ne 0 -or $final.observedState.undoCount -ne 0 -or $final.observedState.hierarchyEntityIds.Count -ne 0 -or $final.observedState.renderEntityIds.Count -ne 0 -or $final.observedState.testEntityId) { throw 'Session did not restore its empty baseline' }
    $null = Send 'session.close'
    $process.StandardInput.Close()
    if (-not $process.WaitForExit(15000)) { throw 'CLI did not exit after session.close' }
    $buildFiles = @($AppPath, [IO.Path]::ChangeExtension($AppPath, '.dll'), (Join-Path (Split-Path $AppPath) 'XuanYu.Editor.UI.dll'), [IO.Path]::ChangeExtension($CliPath, '.dll')) | ForEach-Object { [pscustomobject]@{ path=$_; sha256=(Get-FileHash -Algorithm SHA256 -LiteralPath $_).Hash } }; $sourceFiles = @('XuanYu.Editor.UI/Control/XycSceneContracts.cs','XuanYu.Editor.UI/Control/XycSceneCommandProcessor.cs','XuanYu.Editor.UI/Vm/Control/UiVm.XycScene.cs','XuanYu.Editor.UI/Vm/Control/UiVm.XycScene.Guards.cs','XuanYu.World.Tests/Control/XycSceneCommandTests.cs') | ForEach-Object { [pscustomobject]@{ path=$_; sha256=(Get-FileHash -Algorithm SHA256 -LiteralPath $_).Hash } }; $head = (git rev-parse HEAD).Trim(); [pscustomobject]@{ sessionId=$session; pid=$process.Id; sourceHead=$head; sourceDirty=$true; buildFiles=$buildFiles; sourceFiles=$sourceFiles; baseline=$baseline.observedState; created=$created.observedState; entityBefore=$entity0.observedState; moved=$moved.observedState; entityMoved=$entity1.observedState; undoMove=$undoMove.observedState; entityUndo=$afterUndo.observedState; redoMove=$redoMove.observedState; entityRedo=$afterRedo.observedState; undoAgain=$undoAgain.observedState; undoCreate=$undoCreate.observedState; final=$final.observedState; captureBefore=$captureBefore.observedState; captureCreated=$captureCreated.observedState; captureMoved=$captureMoved.observedState; captureRestored=$captureRestored.observedState; exitCode=$process.ExitCode; negative=$negative; commands=$records } | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath (Join-Path $EvidenceDir 'runtime.json') -Encoding utf8
} catch {
    $records | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath (Join-Path $EvidenceDir 'runtime-partial.json') -Encoding utf8
    if (-not $process.HasExited) { try { $null = Send 'session.close'; $process.StandardInput.Close(); $process.WaitForExit(10000) | Out-Null } catch { $process.Kill($true); $process.WaitForExit() } }
    throw
} finally { $process.Dispose() }
