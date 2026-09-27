[CmdletBinding()]
param(
    [ValidateSet('prepare', 'join', 'status', 'close')][string]$Mode = 'join',
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
    $State | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $StatePath -Encoding UTF8
}

function Get-Facts {
    $branch = (Invoke-Git @('branch', '--show-current') -join '').Trim()
    $head = (Invoke-Git @('rev-parse', 'HEAD') -join '').Trim()
    $dirty = @(Invoke-Git @('status', '--porcelain=v1', '--untracked-files=all'))
    [pscustomobject]@{ Branch = $branch; Head = $head; Dirty = $dirty }
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
    if ($facts.Branch -ne $state.branch) { Stop-Handoff 'BASELINE_MOVED' "Branch 已从 $($state.branch) 变为 $($facts.Branch)。" }
    if ($facts.Head -ne $state.baselineHead) { Stop-Handoff 'BASELINE_MOVED' "HEAD 已从 $($state.baselineHead) 变为 $($facts.Head)。" }
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
    if ($null -ne $state -and [bool]$state.active) { Stop-Handoff 'ACTIVE_WAVE' '当前 Workspace 已有 Active Wave，禁止再次 PREPARE。' }
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
