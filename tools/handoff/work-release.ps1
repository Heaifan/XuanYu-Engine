[CmdletBinding()]param([ValidateSet('issue','claim','assert-write','revoke')][string]$Mode='assert-write',[string]$RepositoryRoot='', [string]$Token='', [string]$LaneId='', [string]$TaskId='', [string[]]$OwnershipSet=@(), [string[]]$Files=@(), [string]$VersionEventId='',[switch]$CentralIssuer)
$d=$PSScriptRoot; . (Join-Path $d 'work-release.common.ps1'); . (Join-Path $d 'work-release.issue.ps1'); . (Join-Path $d 'work-release.assert.ps1'); $r=Root $RepositoryRoot
try {
    switch ($Mode) {
        'issue' { if (!$CentralIssuer) { Fail WRITE_DENIED 'Only central Coordinator may issue.' }; (Issue-Release $r ([pscustomobject]@{TaskId=$TaskId;LaneId=$LaneId;OwnershipSet=$OwnershipSet;VersionEventId=$VersionEventId}) | ConvertTo-Json -Depth 8) }
        'claim' { (Claim-Ownership $r $Token $LaneId $TaskId $Files | ConvertTo-Json -Depth 8) }
        'assert-write' { (Validate-Release $r $Token $LaneId $TaskId $Files | ConvertTo-Json -Depth 8) }
        'revoke' { $x=Read-Json (JsonPath $r 'work-release.json'); if ($x) { $x.status='REVOKED'; $x.revokedReason='Central revocation'; Write-Json (JsonPath $r 'work-release.json') $x }; 'WORK RELEASE REVOKED' }
    }
    exit 0
} catch { Write-Error $_.Exception.Message; exit 1 }
