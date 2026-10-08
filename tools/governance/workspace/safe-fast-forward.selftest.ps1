[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('xye-safe-sync-' + [guid]::NewGuid().ToString('N'))
$script = Join-Path $PSScriptRoot 'safe-fast-forward.ps1'
function Invoke-TestGit([string]$Repo,[string[]]$GitArgs) { $out=@(& git -C $Repo @GitArgs 2>&1); if($LASTEXITCODE){throw "GIT FAILED: $($GitArgs -join ' ') / $($out -join "`n")"} }
function NewFixture([string]$Name) {
    $dir=Join-Path $root $Name;$seed=Join-Path $dir 'seed';$remote=Join-Path $dir 'remote.git';$local=Join-Path $dir 'local';$writer=Join-Path $dir 'writer'
    New-Item -ItemType Directory -Force $seed|Out-Null;git -C $seed init -b main -q;git -C $seed config user.email test@example.invalid;git -C $seed config user.name test
    Set-Content (Join-Path $seed 'base.txt') 'base';Invoke-TestGit $seed @('add','base.txt');Invoke-TestGit $seed @('commit','-qm','base');Invoke-TestGit $seed @('clone','--bare',$seed,$remote);Invoke-TestGit $seed @('clone',$remote,$local);Invoke-TestGit $local @('config','user.email','test@example.invalid');Invoke-TestGit $local @('config','user.name','test');Invoke-TestGit $seed @('clone',$remote,$writer);Invoke-TestGit $writer @('config','user.email','test@example.invalid');Invoke-TestGit $writer @('config','user.name','test')
    Set-Content (Join-Path $writer 'remote.txt') 'remote';Invoke-TestGit $writer @('add','remote.txt');Invoke-TestGit $writer @('commit','-qm','remote update');Invoke-TestGit $writer @('push','origin','main');$local
}
function Sync($Repo) { $o=@(& pwsh -NoLogo -NoProfile -File $script -RepositoryRoot $Repo 2>&1);[pscustomobject]@{Code=$LASTEXITCODE;Text=$o -join "`n"} }
function DenyPreserved($Repo,[string]$Path,[string]$Needle,[string]$Reason) {
    $head=(git -C $Repo rev-parse HEAD).Trim();$result=Sync $Repo
    if($result.Code -eq 0 -or $result.Text -notmatch $Reason){throw "EXPECTED BLOCK $Reason / $($result.Text)"}
    if((git -C $Repo rev-parse HEAD).Trim() -cne $head){throw 'blocked sync moved HEAD'}
    if((Get-Content -Raw (Join-Path $Repo $Path)).TrimEnd() -cne $Needle){throw "blocked sync changed $Path"}
}
try {
    New-Item -ItemType Directory -Force $root|Out-Null
    $tracked=NewFixture 'tracked';Set-Content (Join-Path $tracked 'base.txt') 'tracked dirty';DenyPreserved $tracked 'base.txt' 'tracked dirty' 'SYNC_BLOCKED_DIRTY'
    $untracked=NewFixture 'untracked';Set-Content (Join-Path $untracked 'remote.txt') 'untracked collision';DenyPreserved $untracked 'remote.txt' 'untracked collision' 'SYNC_BLOCKED_DIRTY'
    $foreign=NewFixture 'foreign';Set-Content (Join-Path $foreign 'foreign-owner.txt') 'foreign dirty';DenyPreserved $foreign 'foreign-owner.txt' 'foreign dirty' 'SYNC_BLOCKED_DIRTY'
    $ahead=NewFixture 'ahead';Set-Content (Join-Path $ahead 'local.txt') 'local commit';Invoke-TestGit $ahead @('add','local.txt');Invoke-TestGit $ahead @('commit','-qm','local commit');$head=(git -C $ahead rev-parse HEAD).Trim();$result=Sync $ahead
    if($result.Code -eq 0 -or $result.Text -notmatch 'SYNC_BLOCKED_LOCAL_AHEAD' -or (git -C $ahead rev-parse HEAD).Trim() -cne $head -or (Get-Content -Raw (Join-Path $ahead 'local.txt')).TrimEnd() -cne 'local commit'){throw "ahead sync not blocked: $($result.Code) $($result.Text)"}
    $clean=NewFixture 'clean';$result=Sync $clean;if($result.Code -ne 0 -or $result.Text -notmatch 'SYNC_FAST_FORWARD_COMPLETE'){throw "clean fast-forward failed / $($result.Text)"}
    if((git -C $clean rev-parse HEAD).Trim() -cne (git -C $clean rev-parse '@{upstream}').Trim() -or (Get-Content -Raw (Join-Path $clean 'remote.txt')).TrimEnd() -cne 'remote'){throw 'clean fast-forward did not reach upstream content'}
    'SAFE FAST-FORWARD SELFTEST PASS 5/5'
} finally { $full=[IO.Path]::GetFullPath($root);$temp=[IO.Path]::GetFullPath([IO.Path]::GetTempPath());if($full.StartsWith($temp,[StringComparison]::OrdinalIgnoreCase)-and(Test-Path -LiteralPath $full)){Remove-Item -LiteralPath $full -Recurse -Force} }
