[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$BootstrapActiveBranch = 'feat/v0.3-world-authoring-r1'
$BootstrapRemote = 'origin'
$BootstrapWorkspaces = @(
    'D:\MyDoc\project-vsCode\XuanyuEngine',
    'E:\MyDoc\project-VSCode\XuanYuEngine'
)

function Stop-Handoff([string]$Code, [string]$Reason) {
    Write-Host ''
    Write-Host '================ HANDOFF ================' -ForegroundColor Red
    Write-Host 'HANDOFF BLOCKED' -ForegroundColor Red
    Write-Host "Code   : $Code"
    Write-Host "Reason : $Reason"
    Write-Host '========================================='
    exit 1
}

function Invoke-Git([Parameter(ValueFromRemainingArguments = $true)][string[]]$GitArgs) {
    $output = @(& git @GitArgs 2>&1)
    if ($LASTEXITCODE -ne 0) {
        throw "git $($GitArgs -join ' ') failed:`n$($output -join [Environment]::NewLine)"
    }
    return $output
}

try {
    $RepoRoot = ((Invoke-Git rev-parse --show-toplevel) -join '').Trim()
} catch {
    Stop-Handoff 'NO_REPOSITORY' '当前目录不在 Git 仓库中。'
}
Set-Location $RepoRoot

$ConfigPath = Join-Path $RepoRoot 'tools\handoff\HandoffConfig.psd1'
$ActiveBranch = $BootstrapActiveBranch
$Remote = $BootstrapRemote
$CanonicalWorkspaces = $BootstrapWorkspaces

if (Test-Path $ConfigPath -PathType Leaf) {
    $Config = Import-PowerShellDataFile $ConfigPath
    $ActiveBranch = $Config.ActiveBranch
    $Remote = $Config.Remote
    $CanonicalWorkspaces = $Config.CanonicalWorkspaces
}

$resolvedRepo = [IO.Path]::GetFullPath($RepoRoot).TrimEnd('\')
$workspaceOk = $false
foreach ($workspace in $CanonicalWorkspaces) {
    $candidate = [IO.Path]::GetFullPath($workspace).TrimEnd('\')
    if ($resolvedRepo.Equals($candidate, [StringComparison]::OrdinalIgnoreCase)) {
        $workspaceOk = $true
        break
    }
}
if (-not $workspaceOk) {
    Stop-Handoff 'NON_CANONICAL_WORKSPACE' "未登记的工作区：$resolvedRepo"
}

Invoke-Git fetch --prune $Remote | Out-Null
$RemoteRef = "$Remote/$ActiveBranch"
& git show-ref --verify --quiet "refs/remotes/$Remote/$ActiveBranch"
if ($LASTEXITCODE -ne 0) {
    Stop-Handoff 'REMOTE_BRANCH_NOT_FOUND' "找不到 $RemoteRef"
}

$counts = ((Invoke-Git rev-list --left-right --count "HEAD...$RemoteRef") -join '').Trim() -split '\s+'
$Ahead = [int]$counts[0]
$Behind = [int]$counts[1]
$dirtyBefore = @(& git status --porcelain=v1 --untracked-files=all)

if ($Ahead -gt 0) {
    Write-Host 'Local-only commits:' -ForegroundColor Yellow
    & git log --oneline "$RemoteRef..HEAD"
    Stop-Handoff 'LOCAL_COMMITS_EXIST' "本地存在 $Ahead 个 Remote 没有的正式 Commit。"
}

if ($Behind -gt 0) {
    Write-Host "REMOTE WINS: local is behind $Behind commit(s)." -ForegroundColor Yellow
    Invoke-Git reset --hard HEAD | Out-Null
    Invoke-Git clean -fd | Out-Null
} elseif ($dirtyBefore.Count -gt 0) {
    Stop-Handoff 'DIRTY_AT_REMOTE_TIP' 'Local 与 Remote 无 Commit 差异，但工作区 dirty；禁止静默删除。'
}

Invoke-Git switch --ignore-other-worktrees -C $ActiveBranch $RemoteRef | Out-Null
Invoke-Git reset --hard $RemoteRef | Out-Null
Invoke-Git clean -fd | Out-Null
Invoke-Git branch "--set-upstream-to=$RemoteRef" $ActiveBranch | Out-Null

# Reload the authoritative config after the remote branch is active.
$Config = Import-PowerShellDataFile $ConfigPath
$Resolver = Join-Path $RepoRoot $Config.ResolverScript
$Bootstrap = Join-Path $RepoRoot $Config.BootstrapScript

$drive = [IO.Path]::GetPathRoot($RepoRoot).Substring(0,1).ToUpperInvariant()
if ($Config.PreferredDotnetByDrive.ContainsKey($drive)) {
    $preferred = $Config.PreferredDotnetByDrive[$drive]
    if (Test-Path $preferred -PathType Leaf) {
        $env:XUANYU_DOTNET = $preferred
    }
}

$dotnet = (& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $Resolver).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) {
    Stop-Handoff 'SDK_NOT_FOUND' '正式 Resolver Chain 未能解析 .NET SDK。'
}

$env:DOTNET_ROOT = Split-Path $dotnet -Parent
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
$sdk = (& $dotnet --version).Trim()
if ($LASTEXITCODE -ne 0) {
    Stop-Handoff 'SDK_BROKEN' "解析到的 SDK 无法执行：$dotnet"
}

$bootstrapOutput = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $Bootstrap)
if ($LASTEXITCODE -ne 0) {
    Stop-Handoff 'BOOTSTRAP_FAILED' ($bootstrapOutput -join ' | ')
}

$finalLocal = ((Invoke-Git rev-parse HEAD) -join '').Trim()
$finalRemote = ((Invoke-Git rev-parse $RemoteRef) -join '').Trim()
$finalBranch = ((Invoke-Git branch --show-current) -join '').Trim()
$finalCounts = ((Invoke-Git rev-list --left-right --count "HEAD...$RemoteRef") -join '').Trim() -split '\s+'
$finalDirty = @(& git status --porcelain=v1 --untracked-files=all)

if ($finalBranch -ne $ActiveBranch) { Stop-Handoff 'WRONG_BRANCH' "当前分支：$finalBranch" }
if ($finalLocal -ne $finalRemote) { Stop-Handoff 'HEAD_MISMATCH' 'Local HEAD != Remote HEAD' }
if ([int]$finalCounts[0] -ne 0 -or [int]$finalCounts[1] -ne 0) {
    Stop-Handoff 'DIVERGED' "Ahead/Behind = $($finalCounts[0])/$($finalCounts[1])"
}
if ($finalDirty.Count -gt 0) { Stop-Handoff 'DIRTY_AFTER_SYNC' '同步后 working tree 仍 dirty。' }

$worktreeLines = @(Invoke-Git worktree list --porcelain | Where-Object { $_ -like 'worktree *' })

Write-Host ''
Write-Host '================ HANDOFF ================' -ForegroundColor Green
Write-Host 'HANDOFF PASS' -ForegroundColor Green
Write-Host '-----------------------------------------'
Write-Host "Workspace : $RepoRoot"
Write-Host "Branch    : $finalBranch"
Write-Host "HEAD      : $($finalLocal.Substring(0,8))"
Write-Host "Remote    : $($finalRemote.Substring(0,8))"
Write-Host 'Ahead     : 0'
Write-Host 'Behind    : 0'
Write-Host 'Status    : clean'
Write-Host "DOTNET    : $dotnet"
Write-Host "SDK       : $sdk"
Write-Host "DOTNET_ROOT: $env:DOTNET_ROOT"
Write-Host "Bootstrap : READY"
Write-Host "Worktrees : $($worktreeLines.Count) (informational only)"
Write-Host '========================================='
exit 0
