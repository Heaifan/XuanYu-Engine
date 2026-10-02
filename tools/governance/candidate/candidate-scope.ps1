$ErrorActionPreference='Stop'
function Candidate-Items($value,[string]$name){if($null -eq $value){return @()};if($value.PSObject.Properties.Name -contains $name){return @($value.$name)};return @($value)}
function Candidate-Norm([string]$path){return $path.Replace('\','/').Trim().TrimStart('./').ToLowerInvariant()}
function Candidate-Hit([string]$path,[string]$scope){$p=Candidate-Norm $path;$s=(Candidate-Norm $scope).TrimEnd('/');if($s.EndsWith('/**')){return $p.StartsWith($s.Substring(0,$s.Length-2))};return $p -eq $s -or $p.StartsWith($s+'/')}
function Candidate-Closure($candidate){
    $names=@('CandidateFiles','BuildDependencies','TestDependencies','RuntimeDependencies','TruthFiles','RegistryFiles','HarnessDependencies')
    @($names|%{Candidate-Items $candidate $_}|%{Candidate-Norm ([string]$_)}|?{$_}|sort -Unique)
}
function Candidate-InClosure([string]$path,[string[]]$closure){@($closure|?{Candidate-Hit $path $_}).Count -gt 0}
function Candidate-TaskHits($task,[string[]]$closure){@($task.WriteScope|?{Candidate-InClosure $_ $closure}).Count -gt 0}
