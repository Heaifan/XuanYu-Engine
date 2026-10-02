$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'candidate-scope.ps1')
function Get-CandidateFingerprint([string]$RepositoryRoot,[string[]]$Closure){
    $items=@();foreach($path in @($Closure|sort -Unique)){
        $full=Join-Path $RepositoryRoot $path;$exists=Test-Path -LiteralPath $full -PathType Leaf
        $hash=if($exists){(Get-FileHash -LiteralPath $full -Algorithm SHA256).Hash.ToLowerInvariant()}else{'MISSING'}
        $status=@(& git -C $RepositoryRoot status --porcelain=v1 --untracked-files=all -- $path 2>$null)-join "`n"
        $items+=[ordered]@{Path=$path;Exists=$exists;Hash=$hash;GitStatus=$status}
    }
    $json=$items|ConvertTo-Json -Depth 8 -Compress;$sha=[Security.Cryptography.SHA256]::Create();$bytes=[Text.Encoding]::UTF8.GetBytes($json)
    [pscustomobject]@{Digest=([BitConverter]::ToString($sha.ComputeHash($bytes))-replace '-','').ToLowerInvariant();Items=$items}
}
