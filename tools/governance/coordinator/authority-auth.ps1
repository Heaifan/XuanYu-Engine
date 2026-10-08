$ErrorActionPreference='Stop'
function Auth-Fail([string]$Code,[string]$Message){throw "${Code}: $Message"}
function Repository-Identity([string]$Root){
 $remote=(git -C $Root remote get-url origin 2>$null);if($LASTEXITCODE){Auth-Fail AUTH_REPOSITORY_UNKNOWN 'origin is required.'}
 if($remote -notmatch '^(?:https://github\.com/|git@github\.com:)Heaifan/XuanYu-Engine(?:\.git)?$'){Auth-Fail AUTH_REPOSITORY_MISMATCH 'origin is not the authorized repository.'}
 'Heaifan/XuanYu-Engine'
}
function Get-AuthorityComment([string]$Root,[string]$Url){
 $repo=Repository-Identity $Root
 if($Url -notmatch '^https://github\.com/Heaifan/XuanYu-Engine/issues/(\d+)#issuecomment-(\d+)$'){Auth-Fail AUTH_EVIDENCE_INVALID 'Use a canonical issue comment URL.'}
 $issue=$Matches[1];$id=$Matches[2];$api="https://api.github.com/repos/$repo/issues/comments/$id"
 try{$c=Invoke-RestMethod -Uri $api -Method Get -Headers @{Accept='application/vnd.github+json';'User-Agent'='XuanYuEngine-Governance'} -TimeoutSec 15}catch{Auth-Fail AUTH_EVIDENCE_UNAVAILABLE $_.Exception.Message}
 if([string]$c.id -ne $id -or [string]$c.issue_url -ne "https://api.github.com/repos/$repo/issues/$issue"){Auth-Fail AUTH_EVIDENCE_MISMATCH 'Comment identity does not match its URL.'}
 $c
}
function Assert-OwnerAuthorization([string]$Root,[string]$EvidenceUrl,[string]$Action,[string]$Target,[string]$Detail,[string]$Baseline){
 if($Action -cnotin @('work-release-issue','work-release-revoke','task-close','task-reap','task-transfer','wave-init','wave-close','lane-close')){Auth-Fail AUTH_ACTION_INVALID 'Unsupported lifecycle action.'}
 if([string]::IsNullOrWhiteSpace($EvidenceUrl)){Auth-Fail AUTHORIZATION_REQUIRED 'Owner authorization evidence is required.'}
 $c=Get-AuthorityComment $Root $EvidenceUrl
 if([string]$c.user.login -ne 'Heaifan' -or [string]$c.user.id -ne '86356120' -or [string]$c.author_association -ne 'OWNER'){Auth-Fail AUTH_OWNER_REQUIRED 'Comment author is not the repository owner.'}
 if([string]$c.updated_at -ne [string]$c.created_at){Auth-Fail AUTH_EDITED_EVIDENCE 'Edited authorization comments are not accepted.'}
 $lines=@(([string]$c.body -split "`r?`n")|%{$_.TrimEnd()});if($lines.Count -ne 6 -or $lines[0] -ne 'XYE AUTHORIZATION V1'){Auth-Fail AUTH_EVIDENCE_INVALID 'Authorization comment schema is invalid.'}
 $fields=@{};foreach($line in $lines[1..5]){if($line -notmatch '^([A-Za-z]+): (.+)$' -or $Matches[1] -cnotin @('Action','Target','Detail','Baseline','ExpiresAt') -or $fields.ContainsKey($Matches[1])){Auth-Fail AUTH_EVIDENCE_INVALID 'Authorization fields are malformed.'};$fields[$Matches[1]]=$Matches[2]}
 foreach($name in @('Action','Target','Detail','Baseline','ExpiresAt')){if(!$fields.ContainsKey($name)){Auth-Fail AUTH_EVIDENCE_INVALID "Missing $name."}}
 if($fields.Action -cne $Action -or $fields.Target -cne $Target -or $fields.Detail -cne $Detail -or $fields.Baseline -cne $Baseline){Auth-Fail AUTH_SCOPE_MISMATCH 'Owner authorization does not match this operation.'}
 try{$created=[datetimeoffset]::Parse([string]$c.created_at);$expires=[datetimeoffset]::Parse($fields.ExpiresAt)}catch{Auth-Fail AUTH_EVIDENCE_INVALID 'Authorization time is invalid.'}
 $now=[datetimeoffset]::UtcNow;if($expires -le $now -or $expires -gt $created.AddHours(48) -or $created -gt $now.AddMinutes(2)){Auth-Fail AUTH_EXPIRED 'Authorization is expired or exceeds the 48-hour limit.'}
 [pscustomobject]@{CommentId=[string]$c.id;Author=[string]$c.user.login;CreatedAt=$created.ToString('o');ExpiresAt=$expires.ToString('o');Action=$Action;Target=$Target;Detail=$Detail;Baseline=$Baseline;Url=$EvidenceUrl}
}
function Consume-OwnerAuthorization([string]$Root,$Grant){
 $dir=Join-Path $Root '.git/xye-handoff';New-Item -ItemType Directory -Force $dir|Out-Null;$lock=$null
 try{$lock=[IO.File]::Open((Join-Path $dir 'authorization-events.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)}catch{Auth-Fail AUTHORIZATION_BUSY 'Another authorization is being consumed.'}
 try{$path=Join-Path $dir 'authorization-events.jsonl';$events=@();foreach($line in Get-Content $path -ErrorAction SilentlyContinue){try{$entry=ConvertFrom-Json $line;$events+=,$entry}catch{Auth-Fail AUTH_AUDIT_CORRUPT 'Authorization audit history is malformed.'}};if(@($events|?{[string]$_.CommentId -ceq [string]$Grant.CommentId}).Count){Auth-Fail AUTH_EVIDENCE_REPLAYED 'Owner authorization comment was already consumed.'};$event=[ordered]@{At=[datetime]::UtcNow.ToString('o');Event='AUTHORIZATION_CONSUMED';CommentId=$Grant.CommentId;Author=$Grant.Author;Action=$Grant.Action;Target=$Grant.Target;Detail=$Grant.Detail;Baseline=$Grant.Baseline;EvidenceUrl=$Grant.Url}|ConvertTo-Json -Compress;[IO.File]::AppendAllText($path,$event+"`n",(New-Object Text.UTF8Encoding($false)))}finally{$lock.Dispose()}
}
