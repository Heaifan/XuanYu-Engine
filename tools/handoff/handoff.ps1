[CmdletBinding()]
param(
    [ValidateSet('prepare', 'join', 'status', 'close', 'advance')][string]$Mode = 'join',
    [ValidateSet('xye', 'xyui', 'integration', 'governance')][string]$Scope = 'xye',
    [ValidateSet('development', 'convergence')][string]$WaveMode = 'development',
    [ValidateSet('xye', 'integration', 'governance')][AllowNull()][string]$CoordinatorScope = $null,
    [string]$RepositoryRoot = $null,
    [switch]$AllowTestWorkspace
)

$ErrorActionPreference = 'Stop'
$RepoRoot = $null
$Config = $null
$StatePath = $null

function Stop-Handoff([string]$Code, [string]$Reason) {
    Write-Host ''
    Write-Host '================ HANDOFF ================' -ForegroundColor Red
    Write-Host 'HANDOFF BLOCKED' -ForegroundColor Red
    Write-Host "Code     : $Code"
    Write-Host "Reason   : $Reason"
    Write-Host '========================================='
    exit 1
}

function Invoke-Git([string[]]$GitArgs) {
    $old = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        $output = @(& git @GitArgs 2>&1)
        $exitCode = $LASTEXITCODE
    } finally { $ErrorActionPreference = $old }
    if ($exitCode -ne 0) {
        throw "git $($GitArgs -join ' ') failed with exit code ${exitCode}:`n$($output -join [Environment]::NewLine)"
    }
    return @($output | ForEach-Object { "$($_)" })
}

function Read-Config([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "Handoff config not found: $Path" }
    $import = Get-Command Import-PowerShellDataFile -ErrorAction SilentlyContinue
    if ($null -ne $import) { return Import-PowerShellDataFile -Path $Path }
    $value = & ([ScriptBlock]::Create([IO.File]::ReadAllText($Path)))
    if ($value -isnot [hashtable]) { throw "Handoff config did not return a Hashtable: $Path" }
    return $value
}

function Read-State {
    if (-not (Test-Path -LiteralPath $StatePath -PathType Leaf)) { return $null }
    try { return (Get-Content -Raw -LiteralPath $StatePath | ConvertFrom-Json) }
    catch { Stop-Handoff 'STATE_INVALID' $_.Exception.Message }
}

function Write-State($State) {
    $dir = Split-Path -Parent $StatePath
    if (-not (Test-Path -LiteralPath $dir -PathType Container)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $temp = Join-Path $dir ('state.' + [guid]::NewGuid().ToString('N') + '.tmp')
    try {
        $json = $State | ConvertTo-Json -Depth 6
        [IO.File]::WriteAllText($temp, $json, [Text.UTF8Encoding]::new($false))
        Move-Item -LiteralPath $temp -Destination $StatePath -Force
    } finally {
        if (Test-Path -LiteralPath $temp) { Remove-Item -LiteralPath $temp -Force }
    }
}

function Get-Facts {
    $branch = (Invoke-Git @('branch', '--show-current') -join '').Trim()
    $head = (Invoke-Git @('rev-parse', 'HEAD') -join '').Trim()
    $dirty = @(Invoke-Git @('status', '--porcelain=v1', '--untracked-files=all'))
    [pscustomobject]@{ Branch = $branch; Head = $head; Dirty = $dirty }
}

function Get-RemoteFacts {
    $remote = $Config.Remote
    $remoteRef = "refs/remotes/$remote/$($Config.ActiveBranch)"
    try { Invoke-Git @('show-ref', '--verify', '--quiet', $remoteRef) | Out-Null }
    catch { return [pscustomobject]@{ Exists = $false; Head = $null; Ahead = $null; Behind = $null } }
    $remoteHead = (Invoke-Git @('rev-parse', $remoteRef) -join '').Trim()
    $counts = ((Invoke-Git @('rev-list', '--left-right', '--count', "HEAD...$remoteRef")) -join '').Trim() -split '\s+'
    [pscustomobject]@{ Exists = $true; Head = $remoteHead; Ahead = [int]$counts[0]; Behind = [int]$counts[1] }
}

function Test-Ancestor([string]$Old, [string]$New) {
    & git merge-base --is-ancestor $Old $New 2>$null
    return $LASTEXITCODE -eq 0
}

function Test-AdvanceReady($State, $Facts, $RemoteFacts) {
    if ($null -eq $State -or -not [bool]$State.active) { return $false }
    if ($Facts.Branch -ne $State.branch -or -not $RemoteFacts.Exists) { return $false }
    if ($Facts.Head -ne $RemoteFacts.Head -or $RemoteFacts.Ahead -ne 0 -or $RemoteFacts.Behind -ne 0) { return $false }
    if ($Facts.Head -eq $State.baselineHead) { return $true }
    return Test-Ancestor $State.baselineHead $Facts.Head
}

function Stop-Advance([string]$Code, [string]$Reason) {
    Write-Host ''
    Write-Host '================ HANDOFF ================' -ForegroundColor Red
    Write-Host "ADVANCE FAIL: $Code" -ForegroundColor Red
    Write-Host "Reason: $Reason"
    Write-Host '========================================='
    exit 1
}

function Show-BaselineMoved($Facts, $State, $RemoteFacts) {
    Write-Host ''
    Write-Host 'HANDOFF JOIN BLOCKED' -ForegroundColor Red
    Write-Host 'Reason: BASELINE_MOVED'
    Write-Host "Baseline: $($State.baselineHead)"
    Write-Host "Current HEAD: $($Facts.Head)"
    Write-Host "Branch: $($Facts.Branch)"
    Write-Host "Remote HEAD: $(if ($RemoteFacts.Exists) { $RemoteFacts.Head } else { 'NOT_FOUND' })"
    Write-Host "Ahead/Behind: $(if ($RemoteFacts.Exists) { "$($RemoteFacts.Ahead)/$($RemoteFacts.Behind)" } else { 'UNKNOWN' })"
    if (Test-AdvanceReady $State $Facts $RemoteFacts) {
        Write-Host 'Resolution:'
        Write-Host 'Coordinator must run:'
        Write-Host 'handoff.cmd advance --scope xye'
        Write-Host 'Then retry join.'
    } else {
        Write-Host 'Resolution: Resolve branch or remote convergence before advance.'
    }
    Write-Host 'IMPORTANT: Do NOT run prepare.'
    Write-Host 'Do NOT manually edit state.json.'
    Write-Host 'Do NOT reset/stash/clean the workspace.'
    exit 1
}

function Get-DirtyPath([string]$Line) {
    $path = if ($Line.Length -gt 3) { $Line.Substring(3).Trim() } else { $Line.Trim() }
    if ($path.Contains(' -> ')) { $path = ($path -split ' -> ')[-1] }
    return $path.Trim('"') -replace '\\', '/'
}

function Test-OwnedPath([string]$Path, [string]$Lane) {
    $normalized = $Path.TrimStart('/')
    if ($Lane -eq 'xyui') { return $normalized.StartsWith('xyui/') }
    if ($Lane -eq 'governance') {
        return $normalized -eq 'AGENTS.md' -or $normalized.StartsWith('docs/') -or $normalized.StartsWith('tools/handoff/')
    }
    if ($Lane -eq 'xye') {
        return -not ($normalized.StartsWith('xyui/') -or $normalized.StartsWith('docs/') -or $normalized.StartsWith('tools/handoff/') -or $normalized -eq 'AGENTS.md')
    }
    return $false
}

function Get-DirtyReport($Facts, [string]$Lane) {
    $paths = @($Facts.Dirty | ForEach-Object { Get-DirtyPath $_ })
    if ($Lane -eq 'integration') {
        return [pscustomobject]@{ Own = 'UNKNOWN'; Foreign = 'INFORMATIONAL'; Paths = $paths }
    }
    $own = @($paths | Where-Object { Test-OwnedPath $_ $Lane })
    $foreign = @($paths | Where-Object { -not (Test-OwnedPath $_ $Lane) })
    [pscustomobject]@{ Own = [string]$own.Count; Foreign = [string]$foreign.Count; Paths = $paths }
}

function Assert-CanonicalWorkspace {
    if ($AllowTestWorkspace) { return }
    $resolved = [IO.Path]::GetFullPath($RepoRoot).TrimEnd('\')
    foreach ($workspace in $Config.CanonicalWorkspaces) {
        if ($resolved.Equals([IO.Path]::GetFullPath($workspace).TrimEnd('\'), [StringComparison]::OrdinalIgnoreCase)) { return }
    }
    Stop-Handoff 'NON_CANONICAL_WORKSPACE' "未登记的工作区：$resolved"
}

function Resolve-Dotnet {
    $drive = [IO.Path]::GetPathRoot($RepoRoot).Substring(0, 1).ToUpperInvariant()
    if ($Config.PreferredDotnetByDrive.ContainsKey($drive)) {
        $preferred = $Config.PreferredDotnetByDrive[$drive]
        if (Test-Path -LiteralPath $preferred -PathType Leaf) { $env:XUANYU_DOTNET = $preferred }
    }
    $resolver = Join-Path $RepoRoot $Config.ResolverScript
    $lines = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $resolver)
    if ($LASTEXITCODE -ne 0 -or $lines.Count -eq 0) { Stop-Handoff 'SDK_NOT_FOUND' '正式 Resolver Chain 未能解析 .NET SDK。' }
    $dotnet = ($lines | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -Last 1).Trim()
    $sdk = (& $dotnet --version 2>&1 | Out-String).Trim()
    if ($LASTEXITCODE -ne 0) { Stop-Handoff 'SDK_BROKEN' "解析到的 SDK 无法执行：$dotnet" }
    [pscustomobject]@{ Path = $dotnet; Version = $sdk }
}

function Show-Header([string]$Name, [string]$Result, $Facts, $State, $Toolchain, $Report, [string]$Bootstrap = 'NOT_RUN') {
    Write-Host ''
    Write-Host '================ HANDOFF ================' -ForegroundColor $(if ($Result -like '*PASS') { 'Green' } else { 'Red' })
    Write-Host "HANDOFF $Name $Result"
    Write-Host '-----------------------------------------'
    Write-Host "Mode       : $Name"
    Write-Host "Workspace  : $RepoRoot"
    Write-Host "Branch     : $($Facts.Branch)"
    Write-Host "Baseline   : $(if ($null -eq $State) { 'NONE' } else { $State.baselineHead })"
    Write-Host "HEAD       : $($Facts.Head)"
    Write-Host "Active     : $(if ($null -eq $State) { 'false' } else { $State.active })"
    Write-Host "WaveMode   : $(if ($null -eq $State.mode) { 'development' } else { $State.mode })"
    Write-Host "Coordinator: $(if ($null -eq $State.coordinatorScope) { 'null' } else { $State.coordinatorScope })"
    Write-Host "Scope       : $Scope"
    Write-Host "Dirty      : $(if ($Facts.Dirty.Count -gt 0) { 'YES' } else { 'NO' })"
    Write-Host "DirtyFiles : $($Facts.Dirty.Count)"
    Write-Host "OwnDirty   : $($Report.Own)"
    Write-Host "ForeignDirty: $($Report.Foreign)"
    Write-Host 'GitMutation: NONE'
    if ($null -ne $Toolchain) { Write-Host "DOTNET     : $($Toolchain.Path)"; Write-Host "SDK        : $($Toolchain.Version)" }
    Write-Host "Bootstrap  : $Bootstrap"
    Write-Host '========================================='
}

try {
    if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) { $RepoRoot = ((Invoke-Git @('rev-parse', '--show-toplevel')) -join '').Trim() }
    else { $RepoRoot = [IO.Path]::GetFullPath($RepositoryRoot) }
} catch { Stop-Handoff 'NO_REPOSITORY' '当前目录不在 Git 仓库中。' }
Set-Location $RepoRoot
$configPath = Join-Path $RepoRoot 'tools\handoff\HandoffConfig.psd1'
if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) { $configPath = Join-Path $RepoRoot 'HandoffConfig.psd1' }
$Config = Read-Config $configPath
$StatePath = Join-Path $RepoRoot '.git\xye-handoff\state.json'
Assert-CanonicalWorkspace

try { $facts = Get-Facts } catch { Stop-Handoff 'GIT_STATE_FAILED' $_.Exception.Message }
$state = Read-State
$report = Get-DirtyReport $facts $Scope

if ($Mode -eq 'status') {
    $toolchain = Resolve-Dotnet
    Show-Header 'STATUS' 'PASS' $facts $state $toolchain $report
    exit 0
}

if ($Mode -eq 'join') {
    if ($null -eq $state) { Stop-Handoff 'STATE_MISSING' 'JOIN 只读且需要 Coordinator 先建立 Workspace Baseline。' }
    if (-not [bool]$state.active) { Stop-Handoff 'INACTIVE_WAVE' '当前 Workspace Wave 已 CLOSE，请由 Coordinator 执行 PREPARE。' }
    if ($facts.Branch -ne $state.branch -or $facts.Head -ne $state.baselineHead) {
        Show-BaselineMoved $facts $state (Get-RemoteFacts)
    }
    if ($state.mode -eq 'convergence' -and $state.coordinatorScope -eq 'xye' -and $Scope -eq 'xyui') {
        Stop-Handoff 'CONVERGENCE_EXCLUSIVE' 'XYE Convergence 期间 Workspace 由 XYE Coordinator 独占，XYUI JOIN 暂停。'
    }
    $toolchain = Resolve-Dotnet
    $bootstrap = Join-Path $RepoRoot $Config.BootstrapScript
    $output = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $bootstrap)
    if ($LASTEXITCODE -ne 0) { Stop-Handoff 'BOOTSTRAP_FAILED' ($output -join ' | ') }
    Show-Header 'JOIN' 'PASS' $facts $state $toolchain $report 'READY'
    exit 0
}

if ($Mode -eq 'advance') {
    if ($null -eq $state) { Stop-Advance 'STATE_NOT_FOUND' 'handoff state.json 不存在。' }
    if (-not [bool]$state.active) { Stop-Advance 'NO_ACTIVE_WAVE' '不存在 Active Wave。' }
    if ($null -ne $state.coordinatorScope -and $Scope -ne $state.coordinatorScope) {
        Stop-Advance 'COORDINATOR_MISMATCH' "当前 scope $Scope 不是 Coordinator scope $($state.coordinatorScope)。"
    }
    if ($facts.Branch -ne $state.branch) { Stop-Advance 'BRANCH_MISMATCH' "Expected $($state.branch)，Current $($facts.Branch)。" }
    $remote = $Config.Remote
    try { Invoke-Git @('fetch', '--prune', $remote) | Out-Null } catch { Stop-Advance 'REMOTE_NOT_FOUND' "无法读取 $remote。" }
    $remoteFacts = Get-RemoteFacts
    if (-not $remoteFacts.Exists) { Stop-Advance 'REMOTE_NOT_FOUND' "找不到 $remote/$($Config.ActiveBranch)。" }
    if ($facts.Head -ne $remoteFacts.Head) { Stop-Advance 'REMOTE_DIVERGED' "Current HEAD $($facts.Head) != Remote HEAD $($remoteFacts.Head)。" }
    if ($remoteFacts.Ahead -ne 0 -or $remoteFacts.Behind -ne 0) {
        Stop-Advance 'REMOTE_NOT_CONVERGED' "Ahead/Behind = $($remoteFacts.Ahead)/$($remoteFacts.Behind)。"
    }
    $oldBaseline = $state.baselineHead
    if ($facts.Head -eq $oldBaseline) {
        Write-Host 'ADVANCE NOOP: BASELINE_ALREADY_CURRENT'
        exit 0
    }
    if (-not (Test-Ancestor $oldBaseline $facts.Head)) {
        Stop-Advance 'NON_FAST_FORWARD_BASELINE' 'Old baseline 不是当前 HEAD 的祖先。'
    }
    $dirtyBefore = @($facts.Dirty)
    $state.baselineHead = $facts.Head
    $audit = [pscustomobject]@{
        from = $oldBaseline
        to = $facts.Head
        at = (Get-Date).ToUniversalTime().ToString('o')
    }
    $state | Add-Member -MemberType NoteProperty -Name baselineAdvance -Value $audit -Force
    Write-State $state
    $verifiedState = Read-State
    $verifiedFacts = Get-Facts
    $verifiedRemote = Get-RemoteFacts
    if ($verifiedState.baselineHead -ne $facts.Head -or $verifiedFacts.Head -ne $facts.Head) {
        Stop-Advance 'STATE_VERIFY_FAILED' 'state 或 HEAD 在 advance 后未保持预期。'
    }
    if ($verifiedRemote.Head -ne $facts.Head -or $verifiedRemote.Ahead -ne 0 -or $verifiedRemote.Behind -ne 0) {
        Stop-Advance 'REMOTE_VERIFY_FAILED' '远端在 advance 后不再与 HEAD 收敛。'
    }
    $dirtyAfter = @($verifiedFacts.Dirty)
    if (($dirtyBefore -join "`n") -ne ($dirtyAfter -join "`n")) {
        Stop-Advance 'FOREIGN_DIRTY_CHANGED' 'ForeignDirty 在 advance 前后发生变化。'
    }
    Write-Host ''
    Write-Host 'HANDOFF ADVANCE PASS' -ForegroundColor Green
    Write-Host "Scope: $Scope"
    Write-Host "Branch: $($verifiedFacts.Branch)"
    Write-Host "Old Baseline: $oldBaseline"
    Write-Host "New Baseline: $($verifiedFacts.Head)"
    Write-Host "HEAD: $($verifiedFacts.Head)"
    Write-Host "Remote HEAD: $($verifiedRemote.Head)"
    Write-Host 'Ahead/Behind: 0/0'
    Write-Host "ForeignDirty: PRESERVED ($($dirtyAfter.Count))"
    Write-Host 'Active Wave: PRESERVED'
    exit 0
}

if ($Mode -eq 'close') {
    if ($null -eq $state -or -not [bool]$state.active) { Stop-Handoff 'NO_ACTIVE_WAVE' '没有可关闭的 Active Wave。' }
    if ($facts.Dirty.Count -gt 0) { Stop-Handoff 'DIRTY_ON_CLOSE' 'Working tree dirty，禁止关闭 Active Wave。' }
    $state.active = $false
    $state.closedAt = (Get-Date).ToUniversalTime().ToString('o')
    Write-State $state
    Show-Header 'CLOSE' 'PASS' $facts $state $null $report
    exit 0
}

if ($Mode -eq 'prepare') {
    if ($null -ne $state -and [bool]$state.active) {
        Write-Host 'HANDOFF PREPARE BLOCKED' -ForegroundColor Red
        Write-Host 'Reason: ACTIVE_WAVE_EXISTS'
        $remoteFacts = Get-RemoteFacts
        if ($facts.Head -ne $state.baselineHead -and (Test-AdvanceReady $state $facts $remoteFacts)) {
            Write-Host 'Current wave does not require a new prepare.'
            Write-Host 'Coordinator action:'
            Write-Host 'handoff.cmd advance --scope xye'
        }
        Write-Host 'Do NOT manually edit state.json, reset HEAD, rebuild workspace, or stash ForeignDirty.'
        exit 1
    }
    $remote = $Config.Remote
    $remoteRef = "$remote/$($Config.ActiveBranch)"
    try { Invoke-Git @('fetch', '--prune', $remote) | Out-Null } catch { Stop-Handoff 'GIT_FETCH_FAILED' $_.Exception.Message }
    try { Invoke-Git @('show-ref', '--verify', '--quiet', "refs/remotes/$remote/$($Config.ActiveBranch)") | Out-Null } catch { Stop-Handoff 'REMOTE_BRANCH_NOT_FOUND' "找不到 $remoteRef" }
    try { $counts = ((Invoke-Git @('rev-list', '--left-right', '--count', "HEAD...$remoteRef")) -join '').Trim() -split '\s+' } catch { Stop-Handoff 'GIT_STATE_FAILED' $_.Exception.Message }
    if ([int]$counts[0] -gt 0) { Stop-Handoff 'LOCAL_COMMITS_EXIST' "本地存在 $($counts[0]) 个 Remote 没有的正式 Commit。" }
    if ([int]$counts[1] -gt 0) {
        Invoke-Git @('reset', '--hard', $remoteRef) | Out-Null
        Invoke-Git @('clean', '-fd') | Out-Null
        Invoke-Git @('switch', '--ignore-other-worktrees', '-C', $Config.ActiveBranch, $remoteRef) | Out-Null
    } elseif ($facts.Dirty.Count -gt 0) { Stop-Handoff 'DIRTY_AT_REMOTE_TIP' 'Local 与 Remote 无 Commit 差异但工作区 dirty。' }
    $newFacts = Get-Facts
    $state = [pscustomobject]@{ branch = $newFacts.Branch; baselineHead = $newFacts.Head; workspace = $RepoRoot; active = $true; mode = $WaveMode; coordinatorScope = $CoordinatorScope; preparedAt = (Get-Date).ToUniversalTime().ToString('o') }
    Write-State $state
    $toolchain = Resolve-Dotnet
    Show-Header 'PREPARE' 'PASS' $newFacts $state $toolchain (Get-DirtyReport $newFacts $Scope)
    exit 0
}
