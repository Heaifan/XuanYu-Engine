$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'version-lib.ps1')
$checks = @()
function Check([string]$Name, [scriptblock]$Action) {
    try { & $Action; $script:checks += "PASS $Name" } catch { $script:checks += "FAIL $Name :: $($_.Exception.Message)" }
}
Check 'FEATURE advances' { if ((Get-NextProcessVersion 'v0.3.0.0-r1' FEATURE) -ne 'v0.3.0.1-r1') { throw 'wrong feature result' } }
Check 'FIX advances' { if ((Get-NextProcessVersion 'v0.3.0.0-r1' FIX) -ne 'v0.3.0.1-fix') { throw 'wrong fix result' } }
Check 'FIX chain advances ordinal' { if ((Get-NextProcessVersion 'v0.2.28.77-fix2' FIX) -ne 'v0.2.28.77-fix3') { throw 'wrong chain result' } }
Check 'No formal version reuse' { if ((Get-NextProcessVersion 'v0.3.0.0-r1' FEATURE) -eq 'v0.3.0.0-r1') { throw 'reused version' } }
Check 'Unknown type blocks' { try { Get-NextProcessVersion 'v0.3.0.0-r1' 'UNKNOWN'; throw 'did not block' } catch { if ($_.Exception.Message -eq 'did not block') { throw } } }
Check 'Unparseable version blocks' { try { ConvertTo-ProcessVersion 'v0.3'; throw 'did not block' } catch { if ($_.Exception.Message -eq 'did not block') { throw } } }
Check 'Governance non-product event blocks' { try { Get-NextProcessVersion 'v0.3.0.0-r1' GOVERNANCE; throw 'did not block' } catch { if ($_.Exception.Message -eq 'did not block') { throw } } }
$checks | ForEach-Object { Write-Output $_ }
if ($checks | Where-Object { $_ -like 'FAIL *' }) { exit 1 }
Write-Output 'VERSION SELFTEST PASS'
