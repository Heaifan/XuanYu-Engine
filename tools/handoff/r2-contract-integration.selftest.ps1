$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "R2 FIREWALL FAILED: $Message" }
}

function Read-Source([string]$Path) { Get-Content -Raw -LiteralPath (Join-Path $repo $Path) }

$handoff = Read-Source 'tools/handoff/handoff.ps1'
$handclap = @(Get-ChildItem (Join-Path $repo 'tools/handclap') -Recurse -File |
    Where-Object { $_.FullName -notmatch '[\\/]tests[\\/]' })
$authorityCalls = '(?i)close-authority\.ps1|migrate-active\.ps1|task-flight-plan\.ps1|candidate-gate\.ps1|work-release(?:\.|-)|ownership-locks\.json|commit-mutex\.json|Write-State|Try-CreateMutex|Remove-Mutex|git\s+((fetch|reset|clean|checkout|switch|merge|rebase|stash|commit|push))'
$handoffAuthorities = @('prepare','join','advance','close','commit-lock','commit-unlock','maintenance','repair')
$legacy = @('PREPARE','JOIN','ADVANCE','CLOSE')

Assert-True ($handoff -match 'HANDOFF_COMMAND_RETIRED') 'legacy retired marker missing'
Assert-True ($handoff -notmatch $authorityCalls) 'Handoff still calls an Authority implementation'
foreach ($command in $handoffAuthorities) {
    Assert-True ($handoff -match "(?i)'$command'") "retired command missing: $command"
}
foreach ($command in $legacy) {
    Assert-True ($handoff -notmatch "(?i)HANDOFF $command PASS") "Handoff still claims $command PASS"
}

foreach ($file in $handclap) {
    $source = Get-Content -Raw $file.FullName
    Assert-True ($source -notmatch $authorityCalls) "Handclap authority leak: $($file.Name)"
    Assert-True ($source -notmatch '(?i)approved\s*=|released\s*=|authority\s*=|ownershipGranted\s*=') "Handclap authority field: $($file.Name)"
}

$runtime = @(Get-ChildItem $repo -Recurse -File -Include *.ps1,*.cmd,*.bat,*.cs,*.js |
    Where-Object { $_.FullName -notmatch '[\\/]tools[\\/]handoff[\\/]' -and $_.FullName -notmatch '[\\/]docs[\\/]' -and $_.FullName -notmatch '[\\/]tests[\\/]' })
$legacyCallers = @($runtime | Where-Object { (Get-Content -Raw $_.FullName) -match '(?i)handoff\.cmd\s+(prepare|join|advance|close)' })
Assert-True ($legacyCallers.Count -eq 0) "Runtime legacy callers: $($legacyCallers.FullName -join ', ')"
'R2 CONTRACT INTEGRATION SELFTEST PASS'
