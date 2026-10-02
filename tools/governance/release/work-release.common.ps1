$ErrorActionPreference = 'Stop'
function Fail([string]$Code,[string]$Message){ throw "${Code}: $Message" }
function Root([string]$Path){ if($Path){return (Resolve-Path -LiteralPath $Path).Path}; (git rev-parse --show-toplevel).Trim() }
function HandoffDir([string]$R){ Join-Path $R '.git\xye-handoff' }
function JsonPath([string]$R,[string]$N){ Join-Path (HandoffDir $R) $N }
function Read-Json([string]$P){ if(!(Test-Path -LiteralPath $P)){return $null}; Get-Content -Raw -LiteralPath $P | ConvertFrom-Json }
function Write-Json([string]$Path,$Value){ $d=Split-Path $Path; New-Item -ItemType Directory -Force $d|Out-Null; $t="$Path.$([guid]::NewGuid().ToString('N')).tmp"; [IO.File]::WriteAllText($t,($Value|ConvertTo-Json -Depth 12),(New-Object Text.UTF8Encoding($false))); Move-Item $t $Path -Force|Out-Null }
function Invoke-Git([string]$R,[string[]]$GitArgs){ $o=@(& git -C $R @GitArgs 2>&1); if($LASTEXITCODE){Fail GIT_FAILED ($o -join ' | ')}; $o }
function Current([string]$R){ $s=@(Invoke-Git $R @('status','--porcelain=v1','--untracked-files=all')); [pscustomobject]@{Head=(Invoke-Git $R @('rev-parse','HEAD')).Trim(); Branch=(Invoke-Git $R @('branch','--show-current')).Trim(); Dirty=$s} }
function CandidatePath([string]$Path){ $Path.Trim('"').Replace('\','/').TrimStart('./') }
function InCandidateScope([string]$Path,[string[]]$Scope){ foreach($root in $Scope){$r=CandidatePath $root;if($Path -eq $r -or $Path.StartsWith($r.TrimEnd('/')+'/')){return $true}};return $false }
function ContentHash([string]$R,[string]$Path){$full=Join-Path $R ($Path.Replace('/','\'));if(!(Test-Path -LiteralPath $full -PathType Leaf)){return 'MISSING'};$sha=[Security.Cryptography.SHA256]::Create();try{return (([BitConverter]::ToString($sha.ComputeHash([IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $full)))))-replace '-','').ToLowerInvariant()}finally{$sha.Dispose()}}
function Fingerprint([string]$R,[string[]]$Scope=@()){
    $c=Current $R;$scope=@($Scope|%{CandidatePath $_}|sort -Unique);$files=[ordered]@{}
    foreach($line in @($c.Dirty)){$path=CandidatePath $line.Substring(3);if(InCandidateScope $path $scope){$files[$path]=[ordered]@{Status=$line.Substring(0,2);ContentHash=ContentHash $R $path}}}
    $record=[ordered]@{HEAD=$c.Head;Branch=$c.Branch;OwnedScope=$scope;DirtyFiles=$files};$raw=$record|ConvertTo-Json -Compress -Depth 12;$sha=[Security.Cryptography.SHA256]::Create();try{return (([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($raw))))-replace '-','').ToLowerInvariant()}finally{$sha.Dispose()}
}
function RegisteredPath([string]$R,[string]$Path){
    if($Path -match '^(tools/handoff/|tools/governance/|scripts/governance/|docs/|XYT/|xyt\.)'){return $true}
    if($Path -match '^xyui/'){return $true}
    $m=Read-Json (Join-Path $R 'tools/governance/ownership/ownership-manifest.json')
    @($m.entries)|?{[string]$_.path -eq $Path}|Select-Object -First 1 -ExpandProperty path
}
function UnknownDirty([string]$R){ @((Current $R).Dirty|%{ if($_.Length -gt 3){$p=$_.Substring(3).Trim('"').Replace('\','/'); if(!(RegisteredPath $R $p)){$_} } }) }
function State([string]$R){ Read-Json (JsonPath $R 'state.json') }
function VersionEvent([string]$R,[string]$Id){ $p=Join-Path $R 'docs/governance/version-events.tsv'; if(!(Test-Path $p)){return $false}; @(Get-Content $p|?{$_ -match [regex]::Escape($Id)}).Count -gt 0 }
function Lock([string]$P){ $d=Split-Path $P; New-Item -ItemType Directory -Force $d|Out-Null; try{return [IO.File]::Open($P,[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)}catch{Fail OWNERSHIP_MUTEX_BUSY 'Ownership registry is locked.'} }
