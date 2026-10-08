$ErrorActionPreference='Stop'
function Candidate-Items($value,[string]$name){if($null -eq $value){return @()};if($value.PSObject.Properties.Name -contains $name){return @($value.$name)};return @($value)}
function Candidate-Norm([string]$path){return $path.Replace('\','/').Trim().TrimStart('./').ToLowerInvariant()}
function Candidate-Hit([string]$path,[string]$scope){$p=Candidate-Norm $path;$s=(Candidate-Norm $scope).TrimEnd('/');if($s.EndsWith('/**')){return $p.StartsWith($s.Substring(0,$s.Length-2))};return $p -eq $s -or $p.StartsWith($s+'/')}
function Candidate-Closure($candidate){
    $names=@('CandidateFiles','BuildDependencies','TestDependencies','RuntimeDependencies','TruthFiles','RegistryFiles','HarnessDependencies')
    @($names|%{Candidate-Items $candidate $_}|%{Candidate-Norm ([string]$_)}|?{$_}|sort -Unique)
}
function Candidate-InClosure([string]$path,[string[]]$closure){@($closure|?{Candidate-Hit $path $_}).Count -gt 0}
function Candidate-TaskHits($task,[string[]]$closure){@($task.WriteScope|?{$scope=[string]$_;@($closure|?{(Candidate-Hit $_ $scope)-or(Candidate-Hit $scope $_)}).Count -gt 0}).Count -gt 0}
function Candidate-OpenDependencies($candidate){$items=@(Candidate-Items $candidate 'ExpectedDependencies')+@(Candidate-Items $candidate 'DependsOn');@($items|?{if($_ -is [string]){[string]$_ -match '(?i)\bOPEN\b'}else{[string]$_.Status -match '^(?i)OPEN$'}})}
function Candidate-LineViolations([string]$root,$candidate){
    $base=[IO.Path]::GetFullPath($root).TrimEnd('\','/')+[IO.Path]::DirectorySeparatorChar;$result=@()
    foreach($path in @(Candidate-Items $candidate 'CandidateFiles')){
        if(([IO.Path]::GetExtension([string]$path)) -notin @('.cs','.axaml','.js')){continue}
        $full=[IO.Path]::GetFullPath((Join-Path $root ([string]$path).TrimStart('/','\')))
        if(!$full.StartsWith($base,[StringComparison]::OrdinalIgnoreCase)){ $result+=,[pscustomobject]@{Path=$path;Lines=-1;Reason='OUTSIDE_REPOSITORY'};continue }
        if(Test-Path -LiteralPath $full -PathType Leaf){$count=[IO.File]::ReadAllLines($full).Count;if($count -gt 100){$result+=,[pscustomobject]@{Path=$path;Lines=$count;Reason='OVER_100'}}}
    }
    return $result
}
function Candidate-GlobalStatus($facts){$status=[string]$facts.GlobalArchitectureStatus;if($status -in @('PASS','FAIL')){$status}else{'UNKNOWN'}}
function Candidate-Outcome([bool]$blocked,[string]$globalStatus){
    $scope=if($blocked){'CANDIDATE_SCOPE_FAIL'}else{'CANDIDATE_SCOPE_PASS'}
    $overall=if($blocked){'CANDIDATE_SCOPE_FAIL'}elseif($globalStatus -eq 'FAIL'){'GLOBAL_KNOWN_BASELINE_FAIL'}elseif($globalStatus -eq 'PASS'){'PASS'}else{'GLOBAL_GATE_NOT_RUN'}
    [pscustomobject]@{CandidateScopeStatus=$scope;GlobalBaselineStatus=$globalStatus;OverallGateStatus=$overall;CommitEligibility=if($overall -eq 'PASS'){'YES'}elseif($overall -eq 'GLOBAL_GATE_NOT_RUN'){'NOT_ESTABLISHED'}else{'NO'}}
}
