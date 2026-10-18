param([string]$CliPath,[string]$AppPath,[string]$OutputRoot)
$ErrorActionPreference='Stop';Add-Type -AssemblyName System.Drawing
$sessionCases=@(@{name='Light-1360';theme='Light';width=1360},@{name='Dark-1360';theme='Dark';width=1360},@{name='Light-1920';theme='Light';width=1920},@{name='Dark-1920';theme='Dark';width=1920})
$results=[Collections.Generic.List[object]]::new()
foreach($case in $sessionCases){
 $dir=Join-Path $OutputRoot $case.name;New-Item -ItemType Directory -Force $dir|Out-Null
 $session=[Guid]::NewGuid().ToString('N');$si=[Diagnostics.ProcessStartInfo]::new($CliPath)
 $si.UseShellExecute=$false;$si.RedirectStandardInput=$true;$si.RedirectStandardOutput=$true;$si.ArgumentList.Add('--app');$si.ArgumentList.Add($AppPath)
 foreach($arg in @('--xyc-ui','--diagnostic-write','--session-id',$session,'--timeout-seconds','60','--evidence-dir',$dir)){$si.ArgumentList.Add($arg)}
 $p=[Diagnostics.Process]::new();$p.StartInfo=$si
 function Send([string]$command,[hashtable]$payload=@{}){$q=@{schemaVersion='XYC-API/1';requestId=[Guid]::NewGuid().ToString('N');sessionId=$session;command=$command;args=$payload};$p.StandardInput.WriteLine(($q|ConvertTo-Json -Depth 8 -Compress));$line=$p.StandardOutput.ReadLine();if([string]::IsNullOrWhiteSpace($line)){throw "No response: $command"};$line|ConvertFrom-Json -Depth 25}
 try{
  if(!$p.Start()){throw "Start failed $($case.name)"};$null=Send 'runtime.info';$null=Send 'ui.window.resize' @{widthDip=$case.width;heightDip=820};$set=Send 'ui.theme.set' @{theme=$case.theme};$capture=Send 'ui.capture' @{fileName="$($case.name).png";backend='avaloniaWindow';timeoutMs=10000};$null=Send 'session.close';$p.StandardInput.Close();if(!$p.WaitForExit(15000)){throw 'Session exit timeout'}
  $side=Get-Content (Join-Path $dir "$($case.name).json")|ConvertFrom-Json;$refPath=Join-Path (Join-Path (Resolve-Path 'docs/qa/XYC-UI-V0.1-P1-A/evidence/candidate-final').Path $case.name) "$($case.name).png"
  $a=[Drawing.Bitmap]::new($refPath);$b=[Drawing.Bitmap]::new($side.imagePath)
  try{$pixels=$a.Width*$a.Height;if($a.Width -ne $b.Width -or $a.Height -ne $b.Height){throw "Dimension mismatch $($case.name)"};$same=0;$over16=0;$sum=0.0;$sq=0.0;$opaque=$true
   for($y=0;$y -lt $a.Height;$y++){for($x=0;$x -lt $a.Width;$x++){$pa=$a.GetPixel($x,$y);$pb=$b.GetPixel($x,$y);if($pa.A -ne 255 -or $pb.A -ne 255){$opaque=$false};$dr=[Math]::Abs($pa.R-$pb.R);$dg=[Math]::Abs($pa.G-$pb.G);$db=[Math]::Abs($pa.B-$pb.B);if($dr -eq 0 -and $dg -eq 0 -and $db -eq 0){$same++};if([Math]::Max($dr,[Math]::Max($dg,$db)) -gt 16){$over16++};foreach($d in @($dr,$dg,$db)){$sum+=$d;$sq+=$d*$d}}}
   $mae=$sum/($pixels*3);$mse=$sq/($pixels*3);$psnr=if($mse -eq 0){[double]::PositiveInfinity}else{10*[Math]::Log10(255*255/$mse)};$exact=100*$same/$pixels;$over=100*$over16/$pixels;$pass=($exact -ge 94.5 -and $mae -le 0.5 -and $psnr -ge 35 -and $over -le 0.9 -and $opaque -and $side.theme.actual -eq $case.theme)
   $results.Add([pscustomobject]@{case=$case.name;sessionId=$session;exitCode=$p.ExitCode;theme=$side.theme.actual;dimensions=@($a.Width,$a.Height);sha256=$side.sha256;buildIdentity=$side.buildIdentity;pid=$side.window.processId;hwnd=$side.window.hwnd;exactPct=$exact;mae=$mae;psnrDb=$psnr;over16Pct=$over;alphaOpaque=$opaque;pass=$pass;reference=$refPath;capture=$side.imagePath;sidecar=$capture.observedState.sidecarPath})
  }finally{$a.Dispose();$b.Dispose()}
 }catch{if(!$p.HasExited){try{$p.Kill($true);$p.WaitForExit()}catch{}};throw}finally{$p.Dispose()}
}
$manifest=[pscustomobject]@{status=if(@($results|Where-Object{-not $_.pass}).Count -eq 0){'PASS'}else{'FAIL'};thresholds=[pscustomobject]@{exactPctMin=94.5;maeMax=0.5;psnrDbMin=35;over16PctMax=0.9};cases=$results};$manifest|ConvertTo-Json -Depth 20|Set-Content (Join-Path $OutputRoot 'manifest.json') -Encoding utf8;$results|Format-Table case,theme,exactPct,mae,psnrDb,over16Pct,pass -AutoSize;if($manifest.status -ne 'PASS'){throw 'P1B fidelity recheck failed'}
