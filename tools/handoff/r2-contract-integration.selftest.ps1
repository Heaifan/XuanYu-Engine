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

$agents = Read-Source 'AGENTS.md'
$devRules = Read-Source 'docs/dev-rules.md'
$lanes = Read-Source 'docs/governance/development-lanes.md'
$experience = Read-Source 'docs/governance/agent-experience-rules.md'
$protocol = Read-Source 'tools/handoff/HANDOFF-PROTOCOL.md'
$config = Read-Source 'tools/governance/workspace/HandoffConfig.psd1'
$safeSync = Read-Source 'tools/governance/workspace/safe-fast-forward.ps1'
Assert-True ($agents -match '(?i)REMOTE WINS.*废止') 'current entry does not explicitly retire Remote Wins'
Assert-True ($agents -notmatch '(?im)^.*REMOTE WINS.*(清理并|自动覆盖|清理本地)') 'current entry still directs destructive Remote Wins cleanup'
Assert-True ($agents -match '未提交|uncommitted' -and $agents -match 'reset、clean、stash、checkout') 'current entry lacks a dirty-preservation stop rule'
Assert-True ($agents -notmatch '(?im)^.*Execution.*Handoff JOIN') 'AGENTS Execution still requires retired JOIN'
Assert-True ($devRules -notmatch '(?i)JOIN 可在') 'dev-rules still relies on JOIN'
Assert-True ($lanes -notmatch '(?i)JOIN 返回|JOIN 可') 'current lane contract still relies on JOIN'
Assert-True ($experience -notmatch '(?i)handoff JOIN \+ Task State') 'Execution Knowledge Preflight still depends on JOIN'
Assert-True ($protocol -match '(?i)RETIRED / DATA-SAFETY OVERRIDE' -and $protocol -match '(?i)REMOTE WINS') 'legacy Remote Wins section lacks a retirement warning'
Assert-True ($config -match 'FastForwardWhenBehind\s*=\s*\$false') 'workspace config still enables behind fast-forward'
$bootstrap = Read-Source 'scripts/xye-bootstrap.ps1'
Assert-True ($safeSync -match 'SYNC_BLOCKED_DIRTY' -and $safeSync -match 'SYNC_BLOCKED_LOCAL_AHEAD' -and $safeSync -match 'merge --ff-only') 'safe sync entry lacks dirty/ahead guard or ff-only'
foreach ($source in @($handoff,$bootstrap,$safeSync)) {
    Assert-True ($source -notmatch '(?i)git\s+(reset\s+--hard|clean\s+-[a-z]*f|checkout\s+--\s+\.|restore\s+--source|stash\s+drop)') 'active entry contains a destructive sync command'
}
foreach ($command in @('join','prepare')) {
    $output = @(& pwsh -NoLogo -NoProfile -File (Join-Path $PSScriptRoot 'handoff.ps1') $command 2>&1)
    if ($LASTEXITCODE -eq 0 -or ($output -join "`n") -notmatch 'HANDOFF_COMMAND_RETIRED') { throw "Legacy $command was not denied" }
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
