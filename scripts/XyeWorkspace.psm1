Set-StrictMode -Version Latest

function Get-XyeWorkspaceRoot {
    $value = [Environment]::GetEnvironmentVariable('XYE_WORKSPACE_ROOT')
    if ([string]::IsNullOrWhiteSpace($value)) {
        $value = [Environment]::GetEnvironmentVariable('XYE_WORKSPACE_ROOT', 'User')
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            [Environment]::SetEnvironmentVariable('XYE_WORKSPACE_ROOT', $value, 'Process')
        }
    }
    if ([string]::IsNullOrWhiteSpace($value)) {
        throw 'XYE_WORKSPACE_ROOT is required; configure a machine-local workspace path.'
    }
    return [IO.Path]::GetFullPath($value.Trim())
}

function Initialize-XyeWorkspace {
    $root = Get-XyeWorkspaceRoot
    foreach ($name in @('Sources', 'Builds', 'Diagnostics', 'Temp', 'Manifests')) {
        New-Item -ItemType Directory -Force -Path (Join-Path $root $name) | Out-Null
    }
    $probe = Join-Path $root ('.write-check-' + [Guid]::NewGuid().ToString('N'))
    try { [IO.File]::WriteAllText($probe, 'ok') }
    catch { throw "XYE workspace is not writable: $root. $($_.Exception.Message)" }
    finally { Remove-Item -LiteralPath $probe -Force -ErrorAction SilentlyContinue }
    $driveRoot = [IO.Path]::GetPathRoot($root)
    $drive = [IO.DriveInfo]::new($driveRoot)
    $free = [math]::Floor($drive.AvailableFreeSpace / 1GB)
    if ($free -lt 24) { Write-Warning "Workspace volume has $free GiB free; 24 GiB is the advisory threshold. No automatic task block is applied." }
    return $root
}

function Assert-XyeChildPath([string]$Root, [string]$Path) {
    $fullRoot = [IO.Path]::GetFullPath($Root).TrimEnd('\') + '\'
    $fullPath = [IO.Path]::GetFullPath($Path)
    if (-not $fullPath.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Path escapes XYE_WORKSPACE_ROOT: $fullPath"
    }
}

function Get-XyeFileHash([string]$Path) {
    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash
}

function Get-XyeTreeFingerprint([string]$Path) {
    $records = Get-ChildItem -LiteralPath $Path -File -Recurse -Force | Sort-Object FullName | ForEach-Object {
        $relative = $_.FullName.Substring($Path.Length).TrimStart('\').Replace('\', '/')
        "$relative|$($_.Length)|$(Get-XyeFileHash $_.FullName)"
    }
    $bytes = [Text.Encoding]::UTF8.GetBytes(($records -join "`n"))
    $sha = [Security.Cryptography.SHA256]::Create()
    try { return [BitConverter]::ToString($sha.ComputeHash($bytes)).Replace('-', '') }
    finally { $sha.Dispose() }
}

function Get-XyeDirectoryBytes([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path)) { return [long]0 }
    $sum = [long]0
    Get-ChildItem -LiteralPath $Path -File -Recurse -Force | ForEach-Object { $sum += $_.Length }
    return $sum
}

function Get-XyeOutputHashes([string[]]$Directories) {
    $extensions = @('.dll', '.exe', '.pdb', '.json', '.jsonl', '.png', '.log', '.csv', '.txt', '.md')
    $files = foreach ($directory in $Directories) {
        if (Test-Path -LiteralPath $directory) {
            Get-ChildItem -LiteralPath $directory -File -Recurse -Force |
                Where-Object { $extensions -contains $_.Extension.ToLowerInvariant() }
        }
    }
    return @($files | Sort-Object FullName -Unique | ForEach-Object {
        [pscustomobject]@{ Path=$_.FullName; Bytes=$_.Length; SHA256=(Get-XyeFileHash $_.FullName) }
    })
}

function Save-XyeManifest([string]$Path, [object]$Manifest) {
    $temporary = "$Path.$PID.tmp"
    $encoding = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($temporary, ($Manifest | ConvertTo-Json -Depth 24), $encoding)
    Move-Item -LiteralPath $temporary -Destination $Path -Force
}

function Get-XyeGitValue([string]$RepositoryRoot, [string[]]$Arguments) {
    $value = & git -C $RepositoryRoot @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) { throw "Git metadata query failed: git $($Arguments -join ' ')`n$value" }
    return ($value | Out-String).Trim()
}

function Get-XyeGitState([string]$RepositoryRoot, [string]$Commit) {
    $tree = Get-XyeGitValue $RepositoryRoot @('rev-parse', "$Commit^{tree}")
    $branch = Get-XyeGitValue $RepositoryRoot @('branch', '--show-current')
    $status = @(& git -C $RepositoryRoot status --porcelain=v1 --untracked-files=all)
    if ($LASTEXITCODE -ne 0) { throw 'Could not read Git dirty state.' }
    $changed = @(& git -C $RepositoryRoot diff --name-only $Commit --)
    $untracked = @(& git -C $RepositoryRoot ls-files --others --exclude-standard)
    $paths = @($changed + $untracked | Sort-Object -Unique)
    $records = foreach ($relative in $paths) {
        $full = Join-Path $RepositoryRoot $relative
        if (Test-Path -LiteralPath $full -PathType Leaf) {
            "$relative|$(Get-XyeFileHash $full)"
        } else { "$relative|DELETED" }
    }
    $sha = [Security.Cryptography.SHA256]::Create()
    try {
        $digest = [BitConverter]::ToString($sha.ComputeHash(
            [Text.Encoding]::UTF8.GetBytes(($records -join "`n")))).Replace('-', '')
    } finally { $sha.Dispose() }
    return [pscustomobject]@{
        Branch=$branch; BaseCommit=$Commit; BaseTree=$tree
        Dirty=($status.Count -gt 0); DirtyFiles=$paths; DirtyDigest=$digest
    }
}

function Copy-XyeOverlay([string]$SourceRoot, [string]$DestinationRoot, [string[]]$OverlayPaths, [string[]]$DeletedPaths) {
    $records = [Collections.Generic.List[string]]::new()
    foreach ($relative in $OverlayPaths) {
        if ([IO.Path]::IsPathRooted($relative)) { throw "Overlay path must be relative: $relative" }
        $source = [IO.Path]::GetFullPath((Join-Path $SourceRoot $relative))
        $target = [IO.Path]::GetFullPath((Join-Path $DestinationRoot $relative))
        Assert-XyeChildPath $SourceRoot $source
        Assert-XyeChildPath $DestinationRoot $target
        if (-not (Test-Path -LiteralPath $source)) { throw "Overlay path does not exist: $relative" }
        if (Test-Path -LiteralPath $source -PathType Container) {
            Copy-Item -LiteralPath $source -Destination $target -Recurse -Force
            Get-ChildItem -LiteralPath $target -File -Recurse | ForEach-Object {
                $name = $_.FullName.Substring($DestinationRoot.Length).TrimStart('\').Replace('\', '/')
                $records.Add("$name|$($_.Length)|$(Get-XyeFileHash $_.FullName)")
            }
        } else {
            New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
            Copy-Item -LiteralPath $source -Destination $target -Force
            $records.Add("$relative|$((Get-Item $target).Length)|$(Get-XyeFileHash $target)")
        }
    }
    foreach ($relative in $DeletedPaths) {
        if ([IO.Path]::IsPathRooted($relative)) { throw "Deleted path must be relative: $relative" }
        $target = [IO.Path]::GetFullPath((Join-Path $DestinationRoot $relative))
        Assert-XyeChildPath $DestinationRoot $target
        if (Test-Path -LiteralPath $target) { Remove-Item -LiteralPath $target -Recurse -Force }
        $records.Add("$relative|DELETED")
    }
    return @($records | Sort-Object)
}

function Quote-XyeArgument([string]$Value) {
    if ($Value.Length -gt 0 -and $Value -notmatch '[\s"]') { return $Value }
    $builder = [Text.StringBuilder]::new()
    [void]$builder.Append('"')
    $slashes = 0
    foreach ($character in $Value.ToCharArray()) {
        if ($character -eq '\') { $slashes++; continue }
        if ($character -eq '"') {
            [void]$builder.Append(('\' * (2 * $slashes + 1))).Append('"')
        } else {
            [void]$builder.Append(('\' * $slashes)).Append($character)
        }
        $slashes = 0
    }
    [void]$builder.Append(('\' * (2 * $slashes))).Append('"')
    return $builder.ToString()
}

function Get-XyeManifestPreview([string]$Root) {
    $directory = Join-Path $Root 'Manifests'
    Get-ChildItem -LiteralPath $directory -Filter '*.json' -File | Sort-Object LastWriteTime -Descending | ForEach-Object {
        $item = Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json
        $observed = $item.Status
        if ($observed -eq 'ACTIVE') {
            try { Get-Process -Id ([int]$item.OwnerProcessId) -ErrorAction Stop | Out-Null }
            catch { $observed = 'INTERRUPTED (owner process absent; preview only)' }
        }
        $bytes = [long]0
        foreach ($name in @('SourceSnapshot','Build','Diagnostics','Temp')) {
            $entry = $item.DirectoryBytes.PSObject.Properties[$name]
            if ($null -ne $entry -and $null -ne $entry.Value) { $bytes += [long]$entry.Value }
        }
        [pscustomobject]@{
            RunId=$item.RunId; TaskType=$item.TaskType; Status=$item.Status; ObservedStatus=$observed
            UpdatedUtc=$item.UpdatedUtc; WorkspaceBytes=$bytes
            SourceDirectory=$item.SourceDirectory; BuildDirectory=$item.BuildDirectory
            DiagnosticsDirectory=$item.DiagnosticsDirectory; Manifest=$_.FullName
        }
    }
}

function Protect-XyeArguments([string[]]$Arguments) {
    $result = [Collections.Generic.List[string]]::new()
    $redactNext = $false
    foreach ($value in $Arguments) {
        if ($redactNext) { $result.Add('[REDACTED]'); $redactNext = $false; continue }
        if ($value -match '(?i)(password|token|secret|api[-_]?key|credential)') {
            if ($value -match '=') { $result.Add(($value -replace '=.*$','=[REDACTED]')) }
            else { $result.Add($value); $redactNext = $true }
        } else { $result.Add($value) }
    }
    return @($result)
}

Export-ModuleMember -Function Get-XyeWorkspaceRoot, Initialize-XyeWorkspace, Assert-XyeChildPath,
    Get-XyeFileHash, Get-XyeTreeFingerprint, Get-XyeDirectoryBytes, Get-XyeOutputHashes,
    Save-XyeManifest, Get-XyeGitValue, Get-XyeGitState, Copy-XyeOverlay, Quote-XyeArgument,
    Get-XyeManifestPreview, Protect-XyeArguments
