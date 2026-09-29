[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('Fingerprint','Begin','Complete','Validate','Reuse','ManualInvalidate')][string]$Operation,
    [Parameter(Mandatory)][string]$RepositoryRoot,
    [Parameter(Mandatory)][string]$EvidencePath,
    [string]$EvidenceId, [string]$LaneId, [string]$SessionId, [string]$WorkspaceIdentity,
    [string[]]$OwnedFiles = @(), [string[]]$ConsumedDependencies = @(),
    [string]$TestCommand, [string]$TestScope, [ValidateSet('PASS','FAIL')][string]$TestResult,
    [string]$EvidenceClass, [string]$ProductAcceptanceState,
    [string]$OwnershipSnapshotPath = 'tools/handoff/ownership-manifest.json'
)
$ErrorActionPreference = 'Stop'
function Rel([string]$path) { ((Resolve-Path -LiteralPath $path).Path.Substring((Resolve-Path $RepositoryRoot).Path.Length)).TrimStart([char]92,[char]47).Replace([char]92,[char]47) }
function HashBytes([byte[]]$bytes) { $sha = [Security.Cryptography.SHA256]::Create(); try { ([BitConverter]::ToString($sha.ComputeHash($bytes))).Replace('-','').ToLowerInvariant() } finally { $sha.Dispose() } }
function HashFile([string]$path) { if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { return 'MISSING' }; HashBytes ([IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $path))) }
function Invoke-H2Git([string[]]$gitArgs) { $out = @(& git -C $RepositoryRoot @gitArgs 2>&1); if ($LASTEXITCODE -ne 0) { throw ($out -join "`n") }; $out }
function Scope([string]$path, [string[]]$roots) { foreach ($root in $roots) { $r = $root.Replace('\','/').TrimStart('./'); if ($path -eq $r -or $path.StartsWith($r.TrimEnd('/') + '/')) { return $true } }; return $false }
function Paths([string[]]$pathArgs) { @(Invoke-H2Git $pathArgs | Where-Object { $_ -and $_.Trim() } | ForEach-Object { $_.Trim().Trim('"').Replace('\','/') } | Sort-Object -Unique) }
function JsonHash($value) { HashBytes ([Text.Encoding]::UTF8.GetBytes(($value | ConvertTo-Json -Compress -Depth 20))) }
function NewFingerprint([string[]]$scopeOwned = $OwnedFiles, [string[]]$scopeDependencies = $ConsumedDependencies) {
    $allRoots = @($scopeOwned + $scopeDependencies | Where-Object { $_ } | ForEach-Object { $_.Replace('\','/').TrimStart('./') } | Sort-Object -Unique)
    $stagedAll = @(Paths @('diff','--cached','--name-only','--diff-filter=ACMRTUXB'))
    $unstagedAll = @(Paths @('diff','--name-only','--diff-filter=ACMRTUXB'))
    $untrackedAll = @(Paths @('ls-files','--others','--exclude-standard'))
    $staged = @($stagedAll | Where-Object { Scope $_ $allRoots })
    $trackedDirty = @($unstagedAll + $stagedAll | Sort-Object -Unique | Where-Object { Scope $_ $allRoots })
    $untracked = @($untrackedAll | Where-Object { Scope $_ $allRoots })
    $dirty = @($trackedDirty + $untracked | Sort-Object -Unique)
    $tracked = [ordered]@{}; foreach ($path in $trackedDirty) { $tracked[$path] = HashFile (Join-Path $RepositoryRoot $path) }
    $untrackedContent = [ordered]@{}; foreach ($path in $untracked) { $untrackedContent[$path] = HashFile (Join-Path $RepositoryRoot $path) }
    $dependencies = [ordered]@{}; foreach ($path in $scopeDependencies | Sort-Object -Unique) { $p = $path.Replace('\','/').TrimStart('./'); $dependencies[$p] = HashFile (Join-Path $RepositoryRoot $p) }
    $ownership = Join-Path $RepositoryRoot $OwnershipSnapshotPath
    $record = [ordered]@{
        HEAD = (Invoke-H2Git @('rev-parse','HEAD') | Select-Object -First 1).Trim(); StagedFiles = $staged; DirtyFiles = $dirty
        TrackedDirtyContentHash = (JsonHash $tracked); RelevantUntrackedFiles = $untrackedContent
        ConsumedDependencySnapshot = $dependencies; OwnershipSnapshot = HashFile $ownership
    }
    $record.DirtyDigest = JsonHash ([ordered]@{ Tracked = $tracked; Untracked = $untrackedContent })
    $record.FingerprintId = JsonHash $record
    [pscustomobject]$record
}
function ReadStore { if (-not (Test-Path -LiteralPath $EvidencePath)) { return @() }; @(Get-Content -Raw -LiteralPath $EvidencePath | ConvertFrom-Json) }
function WriteStore([object[]]$items) { $parent = Split-Path -Parent $EvidencePath; if ($parent) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }; $flat = [Collections.Generic.List[object]]::new(); foreach ($item in $items) { if ($item -is [Array]) { foreach ($nested in $item) { [void]$flat.Add($nested) } } else { [void]$flat.Add($item) } }; $json = ConvertTo-Json -InputObject $flat.ToArray() -Depth 30; Set-Content -LiteralPath $EvidencePath -Value $json -Encoding UTF8 }
function Require([string]$name, [string]$value) { if ([string]::IsNullOrWhiteSpace($value)) { throw "$name is required" } }
function NewPacket($before) {
    Require 'LaneId' $LaneId; Require 'SessionId' $SessionId; Require 'WorkspaceIdentity' $WorkspaceIdentity; Require 'TestCommand' $TestCommand; Require 'TestScope' $TestScope; Require 'EvidenceClass' $EvidenceClass; Require 'ProductAcceptanceState' $ProductAcceptanceState
    [pscustomobject][ordered]@{ EvidenceId = [guid]::NewGuid().ToString('N'); LaneId = $LaneId; SessionId = $SessionId; WorkspaceIdentity = $WorkspaceIdentity; BaselineHead = (Invoke-H2Git @('rev-parse','HEAD') | Select-Object -First 1).Trim(); CandidateHead = $before.HEAD; DirtyDigest = $before.DirtyDigest; OwnedFiles = @($OwnedFiles); ConsumedDependencies = @($ConsumedDependencies); TestCommand = $TestCommand; TestScope = $TestScope; TestResult = 'NOT_EXECUTED'; EvidenceClass = $EvidenceClass; Timestamp = [DateTime]::UtcNow.ToString('o'); ProductAcceptanceState = $ProductAcceptanceState; EvidenceStatus = 'VALID'; Reason = $null; FingerprintBefore = $before; FingerprintAfter = $null }
}
function Reason($a, $b) {
    if ($a.HEAD -ne $b.HEAD) { return 'HEAD_CHANGED' }; if ($a.OwnershipSnapshot -ne $b.OwnershipSnapshot) { return 'OWNERSHIP_CHANGED' }; if ((JsonHash $a.ConsumedDependencySnapshot) -ne (JsonHash $b.ConsumedDependencySnapshot)) { return 'DEPENDENCY_CHANGED' }; if ($a.TrackedDirtyContentHash -ne $b.TrackedDirtyContentHash -or (JsonHash $a.RelevantUntrackedFiles) -ne (JsonHash $b.RelevantUntrackedFiles)) { return 'DIRTY_CONTENT_CHANGED' }; return 'CANDIDATE_TREE_CHANGED'
}
function Find($items) { $x = @($items | Where-Object { $_.EvidenceId -eq $EvidenceId }); if ($x.Count -ne 1) { throw "EvidenceId not found or ambiguous: $EvidenceId" }; $x[0] }
$packet = $null; $store = @(ReadStore)
switch ($Operation) {
    'Fingerprint' { NewFingerprint | ConvertTo-Json -Depth 30; break }
    'Begin' { $packet = NewPacket (NewFingerprint); $store += $packet; WriteStore $store; $packet | ConvertTo-Json -Depth 30; break }
    'Complete' { $packet = Find $store; Require 'TestResult' $TestResult; $after = NewFingerprint $packet.OwnedFiles $packet.ConsumedDependencies; $packet | Add-Member -MemberType NoteProperty -Name TestResult -Value $TestResult -Force | Out-Null; $packet | Add-Member -MemberType NoteProperty -Name CandidateHead -Value $after.HEAD -Force | Out-Null; $packet | Add-Member -MemberType NoteProperty -Name FingerprintAfter -Value $after -Force | Out-Null; if ($packet.FingerprintBefore.FingerprintId -ne $after.FingerprintId) { $packet | Add-Member -MemberType NoteProperty -Name EvidenceStatus -Value 'INVALID' -Force | Out-Null; $packet | Add-Member -MemberType NoteProperty -Name Reason -Value 'CANDIDATE_TREE_CHANGED' -Force | Out-Null }; WriteStore $store; $packet | ConvertTo-Json -Depth 30; break }
    'Validate' { $packet = Find $store; $current = NewFingerprint $packet.OwnedFiles $packet.ConsumedDependencies; $baseline = if ($packet.FingerprintAfter) { $packet.FingerprintAfter } else { $packet.FingerprintBefore }; if ($baseline.FingerprintId -eq $current.FingerprintId -and $packet.EvidenceStatus -eq 'VALID') { $packet | Add-Member -MemberType NoteProperty -Name EvidenceStatus -Value 'VALID' -Force | Out-Null; $packet | Add-Member -MemberType NoteProperty -Name Reason -Value $null -Force | Out-Null } elseif ($packet.EvidenceStatus -ne 'INVALID') { $packet | Add-Member -MemberType NoteProperty -Name EvidenceStatus -Value 'STALE' -Force | Out-Null; $packet | Add-Member -MemberType NoteProperty -Name Reason -Value (Reason $baseline $current) -Force | Out-Null }; WriteStore $store; $packet | ConvertTo-Json -Depth 30; break }
    'Reuse' { $packet = Find $store; $current = NewFingerprint $packet.OwnedFiles $packet.ConsumedDependencies; $baseline = if ($packet.FingerprintAfter) { $packet.FingerprintAfter } else { $packet.FingerprintBefore }; if ($packet.EvidenceStatus -ne 'VALID' -or $baseline.FingerprintId -ne $current.FingerprintId) { throw 'EVIDENCE_REUSE_REJECTED' }; $packet | ConvertTo-Json -Depth 30; break }
    'ManualInvalidate' { $packet = Find $store; $packet | Add-Member -MemberType NoteProperty -Name EvidenceStatus -Value 'INVALID' -Force | Out-Null; $packet | Add-Member -MemberType NoteProperty -Name Reason -Value 'MANUAL_INVALIDATION' -Force | Out-Null; WriteStore $store; $packet | ConvertTo-Json -Depth 30; break }
}
