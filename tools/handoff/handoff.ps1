[CmdletBinding()]
param(
    [ValidateSet('prepare', 'join', 'status', 'close', 'advance', 'commit-lock', 'commit-unlock', 'maintenance', 'repair')][string]$Mode = 'join',
    [ValidateSet('xye', 'xyui', 'integration', 'governance')][string]$Scope = 'xye',
    [ValidateSet('development', 'convergence', 'WIP_RESUME')][string]$WaveMode = 'development',
    [AllowNull()][string]$CoordinatorScope = $null,
    [string]$Owner = $null,
    [string]$SourceBranch = $null,
    [string]$TargetBranch = $null,
    [string]$RepositoryRoot = $null,
    [switch]$AllowTestWorkspace
)

$ErrorActionPreference = 'Stop'
$RepoRoot = $null
$Config = $null
$StatePath = $null
$MutexPath = $null

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

function Normalize-CoordinatorScope($Value) {
    if ($null -eq $Value) { return $null }
    $text = ([string]$Value).Trim()
    if ([string]::IsNullOrWhiteSpace($text)) { return $null }
    if ($text -notin @('xye', 'integration', 'governance')) {
        Stop-Handoff 'STATE_INVALID' "非法 coordinatorScope：$text"
    }
    return $text
}

function Read-State {
    if (-not (Test-Path -LiteralPath $StatePath -PathType Leaf)) { return $null }
    try {
        $state = Get-Content -Raw -LiteralPath $StatePath | ConvertFrom-Json
        $scope = if ($null -ne $state.PSObject.Properties['coordinatorScope']) { $state.coordinatorScope } else { $null }
        if ($null -eq $state.PSObject.Properties['coordinatorScope']) {
            $state | Add-Member -MemberType NoteProperty -Name coordinatorScope -Value $null
        }
        $state.coordinatorScope = Normalize-CoordinatorScope $scope
        return $state
    }
    catch { Stop-Handoff 'STATE_INVALID' $_.Exception.Message }
}

function Write-State($State) {
    $dir = Split-Path -Parent $StatePath
    if (-not (Test-Path -LiteralPath $dir -PathType Container)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $temp = Join-Path $dir ('state.' + [guid]::NewGuid().ToString('N') + '.tmp')
    try {
        $State.coordinatorScope = Normalize-CoordinatorScope $State.coordinatorScope
        $json = $State | ConvertTo-Json -Depth 6
        [IO.File]::WriteAllText($temp, $json, (New-Object System.Text.UTF8Encoding -ArgumentList $false))
        Move-Item -LiteralPath $temp -Destination $StatePath -Force
    } finally {
        if (Test-Path -LiteralPath $temp) { Remove-Item -LiteralPath $temp -Force }
    }
}

function Write-JsonAtomic([string]$Path, $Value) {
    $dir = Split-Path -Parent $Path
    if (-not (Test-Path -LiteralPath $dir -PathType Container)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $temp = Join-Path $dir ([IO.Path]::GetFileName($Path) + '.' + [guid]::NewGuid().ToString('N') + '.tmp')
    try {
        [IO.File]::WriteAllText($temp, ($Value | ConvertTo-Json -Depth 6), (New-Object System.Text.UTF8Encoding -ArgumentList $false))
        Move-Item -LiteralPath $temp -Destination $Path -Force
    } finally {
        if (Test-Path -LiteralPath $temp) { Remove-Item -LiteralPath $temp -Force }
    }
}

function Read-Mutex {
    if (-not (Test-Path -LiteralPath $MutexPath -PathType Leaf)) { return $null }
    try { return Get-Content -Raw -LiteralPath $MutexPath | ConvertFrom-Json }
    catch { Stop-Handoff 'COMMIT_MUTEX_INVALID' $_.Exception.Message }
}

function Try-CreateMutex($Mutex) {
    $dir = Split-Path -Parent $MutexPath
    if (-not (Test-Path -LiteralPath $dir -PathType Container)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $json = $Mutex | ConvertTo-Json -Depth 6
    $bytes = (New-Object System.Text.UTF8Encoding -ArgumentList $false).GetBytes($json)
    $stream = $null
    try {
        $stream = [IO.File]::Open($MutexPath, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
        $stream.Write($bytes, 0, $bytes.Length)
        return $true
    } catch [IO.IOException] {
        return $false
    } finally {
        if ($null -ne $stream) { $stream.Dispose() }
    }
}

function Remove-Mutex {
    if (Test-Path -LiteralPath $MutexPath -PathType Leaf) { Remove-Item -LiteralPath $MutexPath -Force }
}

function Get-EffectiveOwner([string]$Value) {
    if (-not [string]::IsNullOrWhiteSpace($Value)) { return $Value.Trim() }
    return "$env:USERNAME@$env:COMPUTERNAME"
}

function Stop-Mutex([string]$Code, [string]$Reason) {
    Write-Host ''
    Write-Host "COMMIT MUTEX BLOCKED: $Code" -ForegroundColor Red
    Write-Host "Reason: $Reason"
    exit 1
}

function Get-UpstreamRef([string]$Branch) {
    $upstream = @(& git rev-parse --abbrev-ref --symbolic-full-name '@{u}' 2>$null)
    if ($LASTEXITCODE -eq 0 -and $upstream.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace([string]$upstream[0])) {
        return ([string]$upstream[0]).Trim()
    }
    $fallback = "origin/$Branch"
    & git show-ref --verify --quiet "refs/remotes/$fallback" 2>$null
    if ($LASTEXITCODE -eq 0) { return $fallback }
    return $null
}

function Get-Facts {
    $branch = (Invoke-Git @('branch', '--show-current') -join '').Trim()
    $head = (Invoke-Git @('rev-parse', 'HEAD') -join '').Trim()
    $dirty = @(Invoke-Git @('status', '--porcelain=v1', '--untracked-files=all'))
    $upstream = Get-UpstreamRef $branch
    [pscustomobject]@{ Branch = $branch; Head = $head; Dirty = $dirty; Upstream = $upstream }
}

function Get-RemoteFacts($Facts = $null) {
    $remote = $Config.Remote
    if ($null -eq $Facts) { $Facts = Get-Facts }
    $upstream = [string]$Facts.Upstream
    if ([string]::IsNullOrWhiteSpace($upstream)) {
        return [pscustomobject]@{ Exists = $false; Head = $null; Ahead = $null; Behind = $null; Ref = $null; Branch = $null }
    }
    $remoteRef = "refs/remotes/$upstream"
    try { Invoke-Git @('show-ref', '--verify', '--quiet', $remoteRef) | Out-Null }
    catch { return [pscustomobject]@{ Exists = $false; Head = $null; Ahead = $null; Behind = $null; Ref = $upstream; Branch = $upstream.Substring($upstream.IndexOf('/') + 1) } }
    $remoteHead = (Invoke-Git @('rev-parse', $remoteRef) -join '').Trim()
    $counts = ((Invoke-Git @('rev-list', '--left-right', '--count', "HEAD...$upstream")) -join '').Trim() -split '\s+'
    [pscustomobject]@{ Exists = $true; Head = $remoteHead; Ahead = [int]$counts[0]; Behind = [int]$counts[1]; Ref = $upstream; Branch = $upstream.Substring($upstream.IndexOf('/') + 1) }
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

function Test-WipExpired($State, $Facts, $RemoteFacts) {
    if ($null -eq $State -or $State.mode -ne 'WIP_RESUME') { return $false }
    $target = if ($null -ne $State.PSObject.Properties['targetBranch']) { [string]$State.targetBranch } else { '' }
    if ([string]::IsNullOrWhiteSpace($target) -or $Facts.Branch -ne $target) { return $false }
    return $RemoteFacts.Exists -and $RemoteFacts.Branch -eq $target -and $Facts.Head -eq $RemoteFacts.Head -and $RemoteFacts.Ahead -eq 0 -and $RemoteFacts.Behind -eq 0
}

function Test-StatelessReady($Facts, $RemoteFacts) {
    return $RemoteFacts.Exists -and $Facts.Head -eq $RemoteFacts.Head -and $RemoteFacts.Ahead -eq 0 -and $RemoteFacts.Behind -eq 0 -and $Facts.Dirty.Count -eq 0
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
        Write-Host 'handoff.cmd commit-lock --scope xye --owner <session>'
        Write-Host 'handoff.cmd advance --scope xye --owner <session>'
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

function Get-DirtyRecords($Facts) {
    @($Facts.Dirty | ForEach-Object {
        $line = [string]$_
        $status = if ($line.Length -ge 2) { $line.Substring(0, 2) } else { '??' }
        [pscustomobject]@{
            Status = $status
            Path = Get-DirtyPath $line
            Staged = $status[0] -ne ' ' -and $status -ne '??'
            ChangeKind = if ($status -eq '??') { 'Untracked' } elseif ($status[0] -eq 'R') { 'Renamed' } elseif ($status[0] -eq 'D' -or $status[1] -eq 'D') { 'Deleted' } else { 'TrackedModified' }
        }
    })
}

function Get-ConvergenceOwner([string]$Path, $State = $null) {
    $manifestPath = if ($null -ne $State -and $null -ne $State.PSObject.Properties['ownershipManifest']) { [string]$State.ownershipManifest } else { [string]$Config.OwnershipManifest }
    if ([string]::IsNullOrWhiteSpace($manifestPath)) { return $null }
    $full = Join-Path $RepoRoot $manifestPath
    if (-not (Test-Path -LiteralPath $full -PathType Leaf)) { Stop-Handoff 'OWNERSHIP_MANIFEST_MISSING' "找不到 Ownership Manifest：$manifestPath" }
    try { $entries = @(Get-Content -Raw -LiteralPath $full | ConvertFrom-Json).entries }
    catch { Stop-Handoff 'OWNERSHIP_MANIFEST_INVALID' $_.Exception.Message }
    $entry = $entries | Where-Object { [string]$_.path -eq $Path } | Select-Object -First 1
    if ($null -ne $entry) { return [string]$entry.owner }
    return $null
}

function Get-ContentFingerprint([string]$Path) {
    $full = Join-Path $RepoRoot $Path
    if (-not (Test-Path -LiteralPath $full -PathType Leaf)) { return $null }
    return (Get-FileHash -LiteralPath $full -Algorithm SHA256).Hash.ToLowerInvariant()
}

function Get-ConvergenceBaseline($Facts, $RemoteFacts, [string]$RequestedMode, $State = $null) {
    $records = @(Get-DirtyRecords $Facts)
    $staged = @($records | Where-Object { $_.Staged })
    $invalid = @($records | Where-Object { $_.ChangeKind -in @('Deleted', 'Renamed') })
    $baseline = @()
    $governance = @()
    $foreign = @()
    $unknown = @()
    foreach ($record in $records) {
        $owner = Get-ConvergenceOwner $record.Path $state
        if ($null -eq $owner) {
            if (Test-OwnedPath $record.Path 'xye') { $unknown += $record } else { $foreign += $record }
            continue
        }
        $fingerprint = Get-ContentFingerprint $record.Path
        if ([string]::IsNullOrWhiteSpace($fingerprint)) { $unknown += $record; continue }
        $baseline += [pscustomobject]@{
            Path = $record.Path
            Owner = $owner
            ChangeKind = $record.ChangeKind
            BaselineHead = $Facts.Head
            Fingerprint = $fingerprint
            FingerprintKind = if ($record.ChangeKind -eq 'Untracked') { 'ContentHash' } else { 'ContentHash' }
        }
        if ($owner -eq 'GOVERNANCE') { $governance += $baseline[-1] }
    }
    [pscustomobject]@{
        ValidMode = $RequestedMode -eq 'convergence'
        ValidRemote = $null -ne $RemoteFacts -and $RemoteFacts.Exists -and $Facts.Head -eq $RemoteFacts.Head -and $RemoteFacts.Ahead -eq 0 -and $RemoteFacts.Behind -eq 0
        Baseline = $baseline
        Governance = $governance
        Staged = $staged
        Foreign = $foreign
        Unknown = $unknown
        Invalid = $invalid
    }
}

function Stop-DirtyConvergence([string]$Code, [string]$Reason) {
    Stop-Handoff $Code $Reason
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
    Write-Host "Upstream   : $(if ([string]::IsNullOrWhiteSpace([string]$Facts.Upstream)) { 'NONE' } else { $Facts.Upstream })"
    Write-Host "Baseline   : $(if ($null -eq $State) { 'NONE' } else { $State.baselineHead })"
    Write-Host "HEAD       : $($Facts.Head)"
    Write-Host "Active     : $(if ($null -eq $State) { 'false' } else { $State.active })"
    Write-Host "HandoffMode: $(if ($null -eq $State.mode) { 'development' } else { $State.mode })"
    $wave = if ($null -eq $State -or -not [bool]$State.active) { 'STATELESS / READY' } else { 'ACTIVE' }
    Write-Host "Wave       : $wave"
    Write-Host "Coordinator: $(if ($null -eq $State.coordinatorScope) { 'null' } else { $State.coordinatorScope })"
    Write-Host "Scope       : $Scope"
    Write-Host "Dirty      : $(if ($Facts.Dirty.Count -gt 0) { 'YES' } else { 'NO' })"
    Write-Host "DirtyFiles : $($Facts.Dirty.Count)"
    $ownDirty = $Report.Own
    $foreignDirty = $Report.Foreign
    if ($null -ne $State -and $null -ne $State.dirtyBaseline) {
        $ownDirty = @($State.dirtyBaseline | Where-Object { $_.Owner -in @('A', 'B', 'C') }).Count
        $foreignDirty = $State.foreignDirtyCount
    }
    Write-Host "OwnDirty   : $ownDirty"
    Write-Host "ForeignDirty: $foreignDirty"
    if ($null -ne $State -and $null -ne $State.dirtyBaseline) {
        Write-Host "UnknownDirty: $($State.unknownDirtyCount)"
        Write-Host "Staged     : $($State.stagedCount)"
        Write-Host "BaselineFingerprint: $($State.dirtyBaseline.Count) files"
        Write-Host "CandidateTreeMatch: $(if ($State.candidateTreeMatch) { 'YES' } else { 'NO' })"
        Write-Host 'CommitEligibility: NO'
    }
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
$MutexPath = Join-Path $RepoRoot '.git\xye-handoff\commit-mutex.json'
Assert-CanonicalWorkspace

try { $facts = Get-Facts } catch { Stop-Handoff 'GIT_STATE_FAILED' $_.Exception.Message }
$state = Read-State
$report = Get-DirtyReport $facts $Scope
$remoteFacts = Get-RemoteFacts $facts

function Invoke-Bootstrap {
    $bootstrap = Join-Path $RepoRoot $Config.BootstrapScript
    $output = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $bootstrap)
    if ($LASTEXITCODE -ne 0) { Stop-Handoff 'BOOTSTRAP_FAILED' ($output -join ' | ') }
    return 'READY'
}

if ($Mode -eq 'status') {
    $toolchain = Resolve-Dotnet
    Show-Header 'STATUS' 'PASS' $facts $state $toolchain $report
    exit 0
}

if ($Mode -in @('maintenance', 'repair')) {
    if ($facts.Dirty.Count -gt 0) { Stop-Handoff 'MAINTENANCE_DIRTY' 'Control Plane Maintenance 要求 working tree clean。' }
    if (-not $remoteFacts.Exists) { Stop-Handoff 'MAINTENANCE_UPSTREAM_MISSING' '当前 branch 没有可验证的 upstream。' }
    if ($facts.Head -ne $remoteFacts.Head -or $remoteFacts.Ahead -ne 0 -or $remoteFacts.Behind -ne 0) {
        Stop-Handoff 'MAINTENANCE_NOT_CONVERGED' "当前 branch 未与 upstream 收敛：$($remoteFacts.Ahead)/$($remoteFacts.Behind)。"
    }
    $toolchain = Resolve-Dotnet
    $bootstrapStatus = Invoke-Bootstrap
    Show-Header 'MAINTENANCE' 'PASS' $facts $state $toolchain $report $bootstrapStatus
    exit 0
}

if ($Mode -eq 'commit-lock') {
    if ($null -eq $state -or -not [bool]$state.active) { Stop-Mutex 'NO_ACTIVE_WAVE' '没有可获取提交锁的 Active Wave。' }
    $owner = Get-EffectiveOwner $Owner
    $mutex = Read-Mutex
    if ($null -ne $mutex) {
        if ($mutex.owner -eq $owner) {
            Write-Host "HANDOFF COMMIT-LOCK PASS: ALREADY_HELD ($owner)"
            exit 0
        }
        Stop-Mutex 'COMMIT_MUTEX_HELD' "当前持有者：$($mutex.owner)。"
    }
    $created = Try-CreateMutex ([pscustomobject]@{
            owner = $owner
            scope = $Scope
            branch = $facts.Branch
            baselineHead = $state.baselineHead
            acquiredAt = (Get-Date).ToUniversalTime().ToString('o')
        })
    if (-not $created) {
        $mutex = Read-Mutex
        Stop-Mutex 'COMMIT_MUTEX_HELD' "当前持有者：$($mutex.owner)。"
    }
    Write-Host "HANDOFF COMMIT-LOCK PASS: $owner"
    Write-Host 'GitCommitPushAdvance: SERIALIZED'
    Write-Host 'StagingRule: precise paths only; never git add .'
    exit 0
}

if ($Mode -eq 'commit-unlock') {
    $owner = Get-EffectiveOwner $Owner
    $mutex = Read-Mutex
    if ($null -eq $mutex) { Stop-Mutex 'COMMIT_MUTEX_NOT_HELD' '当前没有提交锁。' }
    if ($mutex.owner -ne $owner) { Stop-Mutex 'COMMIT_MUTEX_OWNER_MISMATCH' "当前持有者：$($mutex.owner)。" }
    if ($null -ne $state -and $facts.Head -ne $state.baselineHead) {
        Stop-Mutex 'COMMIT_MUTEX_UNADVANCED' 'HEAD 已前进，必须完成 push + advance 后才能释放提交锁。'
    }
    Remove-Mutex
    Write-Host "HANDOFF COMMIT-UNLOCK PASS: $owner"
    exit 0
}

if ($Mode -eq 'join') {
    if ($null -eq $state -or -not [bool]$state.active -or (Test-WipExpired $state $facts $remoteFacts)) {
        if (-not (Test-StatelessReady $facts $remoteFacts)) {
            Stop-Handoff 'STATELESS_NOT_READY' '无 Active Wave 时要求 canonical workspace、upstream、clean 与 HEAD/Remote 0/0。'
        }
        $toolchain = Resolve-Dotnet
        $bootstrapStatus = Invoke-Bootstrap
        Show-Header 'JOIN' 'PASS' $facts $null $toolchain $report $bootstrapStatus
        exit 0
    }
    if ($facts.Branch -ne $state.branch) {
        Show-BaselineMoved $facts $state $remoteFacts
    }
    if ($facts.Head -ne $state.baselineHead -and -not (Test-Ancestor $state.baselineHead $facts.Head)) {
        Show-BaselineMoved $facts $state $remoteFacts
    }
    if ($state.mode -eq 'convergence' -and $state.coordinatorScope -eq 'xye' -and $Scope -eq 'xyui') {
        Stop-Handoff 'CONVERGENCE_EXCLUSIVE' 'XYE Convergence 期间 Workspace 由 XYE Coordinator 独占，XYUI JOIN 暂停。'
    }
    $toolchain = Resolve-Dotnet
    $bootstrapStatus = Invoke-Bootstrap
    Show-Header 'JOIN' 'PASS' $facts $state $toolchain $report $bootstrapStatus
    exit 0
}

if ($Mode -eq 'advance') {
    if ($null -eq $state) { Stop-Advance 'STATE_NOT_FOUND' 'handoff state.json 不存在。' }
    if (-not [bool]$state.active) { Stop-Advance 'NO_ACTIVE_WAVE' '不存在 Active Wave。' }
    $coordinatorScope = Normalize-CoordinatorScope $state.coordinatorScope
    $advanceScopeException = $state.mode -eq 'convergence' -and $coordinatorScope -eq 'xye' -and $Scope -eq 'governance'
    if (-not [string]::IsNullOrWhiteSpace([string]$coordinatorScope) -and $Scope -ne $coordinatorScope -and -not $advanceScopeException) {
        Stop-Advance 'COORDINATOR_MISMATCH' "当前 scope $Scope 不是 Coordinator scope $coordinatorScope。"
    }
    if ($facts.Branch -ne $state.branch) { Stop-Advance 'BRANCH_MISMATCH' "Expected $($state.branch)，Current $($facts.Branch)。" }
    $remote = $Config.Remote
    try { Invoke-Git @('fetch', '--prune', $remote) | Out-Null } catch { Stop-Advance 'REMOTE_NOT_FOUND' "无法读取 $remote。" }
    $facts = Get-Facts
    $remoteFacts = Get-RemoteFacts $facts
    if (-not $remoteFacts.Exists) { Stop-Advance 'REMOTE_NOT_FOUND' "找不到当前 branch 的 upstream：$($facts.Upstream)。" }
    if ($facts.Head -ne $remoteFacts.Head) { Stop-Advance 'REMOTE_DIVERGED' "Current HEAD $($facts.Head) != Remote HEAD $($remoteFacts.Head)。" }
    if ($remoteFacts.Ahead -ne 0 -or $remoteFacts.Behind -ne 0) {
        Stop-Advance 'REMOTE_NOT_CONVERGED' "Ahead/Behind = $($remoteFacts.Ahead)/$($remoteFacts.Behind)。"
    }
    $oldBaseline = $state.baselineHead
    if ($facts.Head -eq $oldBaseline) {
        Write-Host 'ADVANCE NOOP: BASELINE_ALREADY_CURRENT'
        exit 0
    }
    $mutex = Read-Mutex
    if ($null -eq $mutex) { Stop-Advance 'COMMIT_MUTEX_REQUIRED' 'Baseline-changing advance 必须先获取 commit-lock。' }
    if ([string]::IsNullOrWhiteSpace($Owner)) { Stop-Advance 'COMMIT_MUTEX_REQUIRED' 'Baseline-changing advance 必须显式提供 --owner。' }
    $owner = $Owner.Trim()
    if ($mutex.owner -ne $owner) { Stop-Advance 'COMMIT_MUTEX_OWNER_MISMATCH' "当前持有者：$($mutex.owner)。" }
    if ($mutex.scope -ne $Scope) { Stop-Advance 'COMMIT_MUTEX_SCOPE_MISMATCH' "锁 scope 为 $($mutex.scope)，当前 scope 为 $Scope。" }
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
    $verifiedRemote = Get-RemoteFacts $verifiedFacts
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
    Remove-Mutex
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
    Write-Host 'Commit Mutex: RELEASED'
    exit 0
}

if ($Mode -eq 'close') {
    if ($null -eq $state -or -not [bool]$state.active) { Stop-Handoff 'NO_ACTIVE_WAVE' '没有可关闭的 Active Wave。' }
    if ($facts.Dirty.Count -gt 0) { Stop-Handoff 'DIRTY_ON_CLOSE' 'Working tree dirty，禁止关闭 Active Wave。' }
    $state.active = $false
    if ($null -eq $state.PSObject.Properties['closedAt']) {
        $state | Add-Member -NotePropertyName closedAt -NotePropertyValue $null
    }
    $state.closedAt = (Get-Date).ToUniversalTime().ToString('o')
    Write-State $state
    Show-Header 'CLOSE' 'PASS' $facts $state $null $report
    exit 0
}

if ($Mode -eq 'prepare') {
    try { Invoke-Git @('fetch', '--prune', $Config.Remote) | Out-Null } catch { Stop-Handoff 'GIT_FETCH_FAILED' $_.Exception.Message }
    $facts = Get-Facts
    $remoteFacts = Get-RemoteFacts $facts
    if ($null -ne $state -and [bool]$state.active -and -not (Test-WipExpired $state $facts $remoteFacts)) {
        Write-Host 'HANDOFF PREPARE BLOCKED' -ForegroundColor Red
        Write-Host 'Reason: ACTIVE_WAVE_EXISTS'
        if ($facts.Head -ne $state.baselineHead -and (Test-AdvanceReady $state $facts $remoteFacts)) {
            Write-Host 'Current wave does not require a new prepare.'
            Write-Host 'Coordinator action:'
            Write-Host 'handoff.cmd commit-lock --scope xye --owner <session>'
            Write-Host 'handoff.cmd advance --scope xye --owner <session>'
        }
        Write-Host 'Do NOT manually edit state.json, reset HEAD, rebuild workspace, or stash ForeignDirty.'
        exit 1
    }
    if (-not $remoteFacts.Exists) { Stop-Handoff 'REMOTE_BRANCH_NOT_FOUND' "当前 branch 没有可验证的 upstream：$($facts.Upstream)" }
    if ($remoteFacts.Ahead -gt 0) { Stop-Handoff 'LOCAL_COMMITS_EXIST' "本地存在 $($remoteFacts.Ahead) 个 upstream 没有的正式 Commit。" }
    if ($remoteFacts.Behind -gt 0) {
        if ($Config.RemoteWinsWhenBehind -ne $true) { Stop-Handoff 'REMOTE_BEHIND' '本地落后 upstream，但 RemoteWinsWhenBehind 未启用。' }
        Invoke-Git @('reset', '--hard', $remoteFacts.Ref) | Out-Null
        Invoke-Git @('clean', '-fd') | Out-Null
        $facts = Get-Facts
        $remoteFacts = Get-RemoteFacts $facts
    } elseif ($facts.Dirty.Count -gt 0 -and $WaveMode -ne 'convergence') { Stop-Handoff 'DIRTY_AT_REMOTE_TIP' 'Local 与 Remote 无 Commit 差异但工作区 dirty。' }
    $newFacts = Get-Facts
    $requestedConvergence = $WaveMode -eq 'convergence'
    if ($requestedConvergence) {
        $coordinator = Normalize-CoordinatorScope $CoordinatorScope
        if ($coordinator -ne 'xye') {
            Stop-Handoff 'CONVERGENCE_COORDINATOR_REQUIRED' 'Dirty Convergence Baseline 必须由 XYE Coordinator 显式建立。'
        }
        $remoteFacts = Get-RemoteFacts $newFacts
        $dirtyBaseline = Get-ConvergenceBaseline $newFacts $remoteFacts $WaveMode $state
        if (-not $dirtyBaseline.ValidRemote) {
            Stop-DirtyConvergence 'REMOTE_NOT_CONVERGED' 'Dirty Convergence Baseline 要求 Local/Remote HEAD 与 Ahead/Behind 完全一致。'
        }
        if ($dirtyBaseline.Staged.Count -gt 0) {
            Stop-DirtyConvergence 'STAGED_DIRTY' 'Dirty Convergence Baseline 禁止包含 Staged 修改。'
        }
        if ($dirtyBaseline.Invalid.Count -gt 0) {
            Stop-DirtyConvergence 'UNOWNED_DELETE_OR_RENAME' 'Deleted/Renamed 文件不能作为 Dirty Convergence Baseline。'
        }
        if ($dirtyBaseline.Foreign.Count -gt 0) {
            Stop-DirtyConvergence 'FOREIGN_DIRTY' 'Dirty Convergence Baseline 不允许 ForeignDirty。'
        }
        if ($dirtyBaseline.Unknown.Count -gt 0 -or $dirtyBaseline.Baseline.Count -ne $newFacts.Dirty.Count) {
            Stop-DirtyConvergence 'UNKNOWN_DIRTY' 'Dirty Convergence Baseline 要求所有 Dirty/Untracked 文件都有明确 Owner。'
        }
        $state = [pscustomobject]@{
            waveId = [guid]::NewGuid().ToString('N')
            branch = $newFacts.Branch
            baselineHead = $newFacts.Head
            remoteHead = $remoteFacts.Head
            workspace = $RepoRoot
            active = $true
            mode = 'convergence'
            waveMode = 'convergence'
            ownershipManifest = [string]$Config.OwnershipManifest
            coordinatorScope = $coordinator
            dirtyBaseline = @($dirtyBaseline.Baseline)
            ownDirtyCount = @($dirtyBaseline.Baseline | Where-Object { $_.Owner -in @('A', 'B', 'C') }).Count
            governanceDirtyCount = $dirtyBaseline.Governance.Count
            foreignDirtyCount = $dirtyBaseline.Foreign.Count
            unknownDirtyCount = $dirtyBaseline.Unknown.Count
            stagedCount = $dirtyBaseline.Staged.Count
            candidateTreeMatch = $false
            commitEligibility = $false
            preparedAt = (Get-Date).ToUniversalTime().ToString('o')
        }
    } else {
        if ($newFacts.Dirty.Count -gt 0) {
            Stop-Handoff 'DIRTY_AT_REMOTE_TIP' 'Local 与 Remote 无 Commit 差异但工作区 dirty。'
        }
        if ($WaveMode -eq 'WIP_RESUME') {
            if ([string]::IsNullOrWhiteSpace($TargetBranch)) { Stop-Handoff 'WIP_TARGET_REQUIRED' 'WIP_RESUME 必须显式提供 TargetBranch。' }
            $source = if ([string]::IsNullOrWhiteSpace($SourceBranch)) { $newFacts.Branch } else { $SourceBranch.Trim() }
            $state = [pscustomobject]@{ waveId = [guid]::NewGuid().ToString('N'); branch = $newFacts.Branch; baselineHead = $newFacts.Head; remoteHead = $newFacts.Head; workspace = $RepoRoot; active = $true; mode = 'WIP_RESUME'; waveMode = 'WIP_RESUME'; sourceBranch = $source; targetBranch = $TargetBranch.Trim(); createdAt = (Get-Date).ToUniversalTime().ToString('o'); expiryCondition = 'CurrentBranch==targetBranch && HEAD==origin/targetBranch && Ahead/Behind==0/0'; ownershipManifest = [string]$Config.OwnershipManifest; coordinatorScope = (Normalize-CoordinatorScope $CoordinatorScope) }
        } else {
            $state = [pscustomobject]@{ waveId = [guid]::NewGuid().ToString('N'); branch = $newFacts.Branch; baselineHead = $newFacts.Head; remoteHead = $newFacts.Head; workspace = $RepoRoot; active = $true; mode = 'development'; waveMode = 'development'; ownershipManifest = [string]$Config.OwnershipManifest; coordinatorScope = (Normalize-CoordinatorScope $CoordinatorScope); preparedAt = (Get-Date).ToUniversalTime().ToString('o') }
        }
    }
    Write-State $state
    $toolchain = Resolve-Dotnet
    Show-Header 'PREPARE' 'PASS' $newFacts $state $toolchain (Get-DirtyReport $newFacts $Scope)
    exit 0
}
