$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'migrate-active.ps1'
$root = Join-Path ([IO.Path]::GetTempPath()) ('active-wave-migration-' + [guid]::NewGuid().ToString('N'))
function Invoke-Migration([string[]]$arguments) {
    $output = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath @arguments 2>&1)
    [pscustomobject]@{ Code = $LASTEXITCODE; Text = ($output -join "`n") }
}
try {
    New-Item -ItemType Directory -Path $root -Force | Out-Null
    git -C $root init -q
    git -C $root config user.email test@example.com
    git -C $root config user.name migration-test
    Set-Content -LiteralPath (Join-Path $root 'README.md') -Value 'base'
    git -C $root add README.md
    git -C $root commit -qm base
    $head = (git -C $root rev-parse HEAD).Trim()
    $stateDir = Join-Path $root '.git\xye-handoff'
    New-Item -ItemType Directory -Path $stateDir -Force | Out-Null
    [pscustomobject]@{ waveId = 'WAVE-1'; active = $true; mode = 'development'; coordinatorScope = $null; baselineHead = $head } |
        ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    Add-Content -LiteralPath (Join-Path $root 'README.md') -Value 'dirty'
    $before = @(git -C $root status --porcelain=v1 --untracked-files=all)
    $r = Invoke-Migration @('-CoordinatorScope','xye','-RepositoryRoot',$root,'-AllowTestWorkspace')
    if ($r.Code -ne 0 -or $r.Text -notmatch 'MIGRATION PASS') { throw "legacy migration failed: $($r.Text)" }
    $afterState = Get-Content -Raw (Join-Path $stateDir 'state.json') | ConvertFrom-Json
    if (!$afterState.active -or $afterState.coordinatorScope -ne 'xye') { throw 'scope was not established' }
    if ($afterState.schemaVersion -ne 'XYE-HANDOFF/3' -or $afterState.authorityModel -ne 'R3-LANE-COORDINATOR') { throw 'R2 to R3 authority migration missing' }
    if ((git -C $root rev-parse HEAD).Trim() -ne $head) { throw 'HEAD changed' }
    if (($before -join "`n") -ne (@(git -C $root status --porcelain=v1 --untracked-files=all) -join "`n")) { throw 'dirty set changed' }
    if (!(Test-Path (Join-Path $stateDir 'work-release.json')) -or !(Test-Path (Join-Path $stateDir 'ownership-locks.json'))) { throw 'initial control state missing' }
    $r = Invoke-Migration @('-CoordinatorScope','governance','-RepositoryRoot',$root,'-AllowTestWorkspace')
    if ($r.Code -eq 0 -or $r.Text -notmatch 'COORDINATOR_SCOPE_ALREADY_SET') { throw 'existing scope was not denied' }
    $state = $afterState; $state.active = $false; $state | ConvertTo-Json | Set-Content (Join-Path $stateDir 'state.json')
    $r = Invoke-Migration @('-CoordinatorScope','xye','-RepositoryRoot',$root,'-AllowTestWorkspace')
    if ($r.Code -eq 0 -or $r.Text -notmatch 'NON_ACTIVE_WAVE') { throw 'non-active wave was not denied' }
    'H1 LEGACY MIGRATION 1/1 PASS'
    'ACTIVE WAVE MIGRATION SELFTEST PASS'
} finally { if (Test-Path $root) { Remove-Item -LiteralPath $root -Recurse -Force } }
