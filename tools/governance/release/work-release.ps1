[CmdletBinding()]param([ValidateSet('issue','claim','assert-write','revoke')][string]$Mode='assert-write',[string]$RepositoryRoot='', [string]$Token='', [string]$LaneId='', [string]$TaskId='', [string[]]$OwnershipSet=@(), [string[]]$Files=@(), [string]$VersionEventId='',[string]$AuthorizationEvidenceUrl='', [string]$Reason='')
$d=$PSScriptRoot; . (Join-Path $d '..\coordinator\authority-auth.ps1'); . (Join-Path $d 'work-release.common.ps1'); . (Join-Path $d 'work-release.issue.ps1'); . (Join-Path $d 'work-release.assert.ps1'); $r=Root $RepositoryRoot
try {
    switch ($Mode) {
        'issue' { (Issue-Release $r ([pscustomobject]@{TaskId=$TaskId;LaneId=$LaneId;OwnershipSet=$OwnershipSet;VersionEventId=$VersionEventId}) $AuthorizationEvidenceUrl | ConvertTo-Json -Depth 8) }
        'claim' { (Claim-Ownership $r $Token $LaneId $TaskId $Files | ConvertTo-Json -Depth 8) }
        'assert-write' { (Validate-Release $r $Token $LaneId $TaskId $Files | ConvertTo-Json -Depth 8) }
        'revoke' { Revoke-WorkRelease $r $AuthorizationEvidenceUrl }
    }
    exit 0
} catch { Write-Error $_.Exception.Message; exit 1 }
