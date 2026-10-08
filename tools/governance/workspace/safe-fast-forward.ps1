[CmdletBinding()]
param([string]$RepositoryRoot = (Get-Location).Path)
$ErrorActionPreference = 'Stop'

function Stop-Sync([string]$Code, [string]$Message) {
    Write-Output "$Code $Message"
    exit 2
}
function Read-Git([string[]]$Arguments) {
    $result = @(& git -C $RepositoryRoot @Arguments 2>&1)
    if ($LASTEXITCODE -ne 0) { Stop-Sync 'SYNC_BLOCKED_GIT' ($result -join "`n") }
    ($result -join "`n").Trim()
}

try {
    $RepositoryRoot = [IO.Path]::GetFullPath($RepositoryRoot)
    if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot '.git'))) {
        Stop-Sync 'SYNC_BLOCKED_NOT_REPOSITORY' $RepositoryRoot
    }
    $branch = Read-Git @('branch','--show-current')
    if (-not $branch) { Stop-Sync 'SYNC_BLOCKED_DETACHED_HEAD' 'A named branch is required.' }
    $upstream = Read-Git @('rev-parse','--abbrev-ref','--symbolic-full-name','@{upstream}')
    $beforeHead = Read-Git @('rev-parse','HEAD')
    $status = @(& git -C $RepositoryRoot status --porcelain=v1 --untracked-files=all 2>&1)
    if ($LASTEXITCODE -ne 0) { Stop-Sync 'SYNC_BLOCKED_GIT' ($status -join "`n") }
    if ($status.Count -gt 0) {
        Stop-Sync 'SYNC_BLOCKED_DIRTY' ('Local changes preserved; classify before sync: ' + ($status -join '; '))
    }
    $fetch = @(& git -C $RepositoryRoot fetch 2>&1)
    if ($LASTEXITCODE -ne 0) { Stop-Sync 'SYNC_BLOCKED_FETCH' ($fetch -join "`n") }
    if ((Read-Git @('branch','--show-current')) -cne $branch -or (Read-Git @('rev-parse','HEAD')) -cne $beforeHead) {
        Stop-Sync 'SYNC_BLOCKED_STATE_CHANGED' 'Branch or HEAD changed during fetch.'
    }
    $afterFetchStatus = @(& git -C $RepositoryRoot status --porcelain=v1 --untracked-files=all 2>&1)
    if ($LASTEXITCODE -ne 0) { Stop-Sync 'SYNC_BLOCKED_GIT' ($afterFetchStatus -join "`n") }
    if ($afterFetchStatus.Count -gt 0) {
        Stop-Sync 'SYNC_BLOCKED_DIRTY' ('Local changes appeared during fetch; preserved: ' + ($afterFetchStatus -join '; '))
    }
    $target = Read-Git @('rev-parse',$upstream)
    $counts = Read-Git @('rev-list','--left-right','--count',"$beforeHead...$target")
    $parts = $counts -split '\s+'
    $ahead = [int]$parts[0]; $behind = [int]$parts[1]
    if ($ahead -gt 0) { Stop-Sync 'SYNC_BLOCKED_LOCAL_AHEAD' "Ahead=$ahead Behind=$behind; local commits are preserved." }
    if ($behind -eq 0) { Write-Output 'SYNC_ALREADY_CURRENT'; exit 0 }
    $merge = @(& git -C $RepositoryRoot merge --ff-only $target 2>&1)
    if ($LASTEXITCODE -ne 0) { Stop-Sync 'SYNC_BLOCKED_FAST_FORWARD' ($merge -join "`n") }
    $finalHead = Read-Git @('rev-parse','HEAD')
    $finalStatus = @(& git -C $RepositoryRoot status --porcelain=v1 --untracked-files=all 2>&1)
    if ($finalHead -cne $target -or $finalStatus.Count -gt 0) {
        Stop-Sync 'SYNC_BLOCKED_POSTCHECK' 'Fast-forward postcondition failed; inspect without cleanup.'
    }
    Write-Output "SYNC_FAST_FORWARD_COMPLETE $finalHead"
    exit 0
} catch {
    Stop-Sync 'SYNC_BLOCKED_ERROR' $_.Exception.Message
}
