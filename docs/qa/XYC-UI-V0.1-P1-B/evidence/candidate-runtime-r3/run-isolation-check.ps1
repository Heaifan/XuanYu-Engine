param([string]$AppPath,[string]$CliPath,[string]$OutDir)
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Windows.Forms
Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
public static class XycNative {
 [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")] public static extern IntPtr GetWindow(IntPtr h,uint c);
 [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h,IntPtr after,int x,int y,int w,int hgt,uint f);
 [DllImport("user32.dll")] public static extern bool ShowWindowAsync(IntPtr h,int cmd);
 [DllImport("user32.dll")] public static extern IntPtr GetWindowLongPtr(IntPtr h,int index);
 [DllImport("user32.dll")] public static extern IntPtr SetWindowLongPtr(IntPtr h,int index,IntPtr value);
 [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr h);
 [DllImport("gdi32.dll")] public static extern int GetPixel(IntPtr dc,int x,int y);
 [DllImport("user32.dll")] public static extern int ReleaseDC(IntPtr h,IntPtr dc);
}
"@
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$session=[guid]::NewGuid().ToString('N');$si=[Diagnostics.ProcessStartInfo]::new($CliPath)
$si.Arguments="--app `"$AppPath`" --xyc-ui --session-id $session --timeout-seconds 180 --evidence-dir `"$OutDir`""
$si.UseShellExecute=$false;$si.RedirectStandardInput=$true;$si.RedirectStandardOutput=$true
$p=[Diagnostics.Process]::new();$p.StartInfo=$si;$p.Start()|Out-Null
function Send([string]$cmd,[object]$payload=@{}){$req=@{schemaVersion='XYC-API/1';requestId=[guid]::NewGuid().ToString('N');sessionId=$session;command=$cmd;args=$payload};$p.StandardInput.WriteLine(($req|ConvertTo-Json -Compress -Depth 16));$line=$p.StandardOutput.ReadLine();if(!$line){throw "No response: $cmd"};return $line|ConvertFrom-Json}
$report=[ordered]@{baseline='d2d011ac98314f172e0e3d729583e67832099f1f';cases=@{}}
try {
 $runtime=(Send 'runtime.info').observedState;$window=(Send 'ui.window.resize' @{widthDip=1360;heightDip=820}).observedState
 $theme=Send 'ui.theme.set' @{theme='Light'};$region=(Send 'ui.region.inspect').observedState
 $SendBefore=Send 'ui.capture' @{fileName='Light-1360-before.png'}
 $hwnd=[IntPtr]([Convert]::ToInt64($window.hwnd.Substring(2),16));$fg0=[XycNative]::GetForegroundWindow();$z0=[XycNative]::GetWindow($hwnd,3)
 $form=[System.Windows.Forms.Form]::new();$form.FormBorderStyle='None';$form.ShowInTaskbar=$false;$form.BackColor=[Drawing.Color]::Magenta;$form.StartPosition='Manual';$form.Bounds=[Drawing.Rectangle]::new([int]$region.xpx,[int]$region.ypx,[int]$region.widthPx,[int]$region.heightPx);$form.CreateControl();$overlay=[IntPtr]$form.Handle
 $ex=[XycNative]::GetWindowLongPtr($overlay,-20);[XycNative]::SetWindowLongPtr($overlay,-20,[IntPtr]($ex.ToInt64() -bor 0x08000000 -bor 0x00000080))|Out-Null
 $form.Show()
 [XycNative]::SetWindowPos($overlay,[IntPtr](-1),$form.Left,$form.Top,$form.Width,$form.Height,0x0010 -bor 0x0040)|Out-Null
 [System.Windows.Forms.Application]::DoEvents();Start-Sleep -Milliseconds 100
 $dc=[XycNative]::GetDC([IntPtr]::Zero);$overlayPixel=[XycNative]::GetPixel($dc,$form.Left+$form.Width/2,$form.Top+$form.Height/2);[XycNative]::ReleaseDC([IntPtr]::Zero,$dc)|Out-Null
 $fg1=[XycNative]::GetForegroundWindow();$z1=[XycNative]::GetWindow($hwnd,3)
 $occluded=Send 'ui.capture' @{fileName='Light-1360-occluded.png'}
 $fg2=[XycNative]::GetForegroundWindow();$z2=[XycNative]::GetWindow($hwnd,3)
 $report.cases.occlusion=@{success=$occluded.success;error=$occluded.errorCode;overlayPixel=('0x{0:X8}' -f $overlayPixel);foregroundStable=($fg1 -eq $fg2);zOrderStable=($z1 -eq $z2);foregroundUnchanged=$occluded.observedState.foregroundUnchanged;zOrderUnchanged=$occluded.observedState.zOrderUnchanged;overlayHwnd=('{0:X}' -f $overlay.ToInt64());pixelSha=$occluded.observedState.sha256}
 $form.Close();$form.Dispose()
 $duplicate=Send 'ui.capture' @{fileName='Light-1360-before.png'};$report.cases.duplicate=@{success=$duplicate.success;error=$duplicate.errorCode}
 [XycNative]::ShowWindowAsync($hwnd,6)|Out-Null;Start-Sleep -Milliseconds 100
 $minimized=Send 'ui.capture' @{fileName='Light-1360-minimized.png'};$report.cases.minimized=@{success=$minimized.success;error=$minimized.errorCode;visible=$minimized.observedState.window.visible;state=$minimized.observedState.window.windowState}
 [XycNative]::ShowWindowAsync($hwnd,9)|Out-Null;Start-Sleep -Milliseconds 200
 $restored=Send 'ui.window.inspect';$report.cases.restore=@{success=$restored.success;state=$restored.observedState.windowState;visible=$restored.observedState.visible}
 $null=Send 'session.close';$p.StandardInput.Close();$p.WaitForExit(10000)|Out-Null;$report.editorExit=$p.ExitCode
} finally {if(!$form.IsDisposed){$form.Close();$form.Dispose()};if(!$p.HasExited){$p.Kill($true);$p.WaitForExit()};$p.Dispose()}
$report|ConvertTo-Json -Depth 12|Set-Content -LiteralPath (Join-Path $OutDir 'isolation-report.json') -Encoding UTF8
