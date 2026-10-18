param([string]$CliPath,[string]$AppPath,[string]$EvidencePath)
$ErrorActionPreference='Stop'
$session=[Guid]::NewGuid().ToString('N')
$start=[Diagnostics.ProcessStartInfo]::new($CliPath)
$start.UseShellExecute=$false;$start.RedirectStandardInput=$true;$start.RedirectStandardOutput=$true
$start.ArgumentList.Add('--app');$start.ArgumentList.Add($AppPath)
$start.ArgumentList.Add('--xyc-ui');$start.ArgumentList.Add('--session-id');$start.ArgumentList.Add($session)
$start.ArgumentList.Add('--timeout-seconds');$start.ArgumentList.Add('60')
$p=[Diagnostics.Process]::new();$p.StartInfo=$start;$rows=[Collections.Generic.List[object]]::new()
function Send([string]$command,[hashtable]$payload=@{}) {
 $q=@{schemaVersion='XYC-API/1';requestId=[Guid]::NewGuid().ToString('N');sessionId=$session;command=$command;args=$payload}
 $p.StandardInput.WriteLine(($q|ConvertTo-Json -Depth 8 -Compress));$line=$p.StandardOutput.ReadLine()
 if([string]::IsNullOrWhiteSpace($line)){throw "No response: $command"};$r=$line|ConvertFrom-Json -Depth 20
 $rows.Add([pscustomobject]@{request=$q;response=$r});return $r
}
try {
 if(!$p.Start()){throw 'CLI start failed'}
 $runtime=Send 'runtime.info';$before=Send 'scene.test.inspect';$s=$before.observedState
 $denied=Send 'scene.entity.create-test-cube' @{sceneId=$s.sceneId;expectedRevision=$s.sceneRevision}
 if($denied.success -or $denied.errorCode -ne 'UNAUTHORIZED'){throw "Expected UNAUTHORIZED, got $($denied.errorCode)"}
 $after=Send 'scene.test.inspect'
 if($after.observedState.entityCount -ne $s.entityCount -or $after.observedState.undoCount -ne $s.undoCount -or $after.observedState.redoCount -ne $s.redoCount -or $after.observedState.stage -ne $s.stage){throw 'World/History/Stage changed without write permission'}
 $null=Send 'session.close';$p.StandardInput.Close()
 if(!$p.WaitForExit(15000)){throw 'Editor session did not exit'}
 $hashes=@($AppPath,(Join-Path (Split-Path $AppPath) 'XuanYu.Editor.UI.dll'),$CliPath)|%{[pscustomobject]@{path=$_;sha256=(Get-FileHash $_ -Algorithm SHA256).Hash}}
 [pscustomobject]@{sessionId=$session;pid=$p.Id;runtime=$runtime.observedState;before=$s;denied=$denied;after=$after.observedState;exitCode=$p.ExitCode;buildHashes=$hashes;commands=$rows}|ConvertTo-Json -Depth 25|Set-Content -LiteralPath $EvidencePath -Encoding utf8
} catch {if(!$p.HasExited){try{$p.Kill($true);$p.WaitForExit()}catch{}};throw} finally {$p.Dispose()}
