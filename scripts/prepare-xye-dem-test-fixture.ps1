[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ArchivePath)

$ErrorActionPreference = 'Stop'
$expectedZipSha = '4FAD3EC397874E3F08C6FB22C53789B742FB237DBA9317D7F9B041AA6ACCD23E'
$expectedDemSha = 'BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1'
$expectedLength = 25934402L
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$sourceRoot = $repo
if (-not (Test-Path (Join-Path $repo 'XuanYu.Engine.slnx'))) { throw 'Run this script from the canonical XYE project tree.' }
$temp = Join-Path $repo 'artifacts/diagnostics/wp2-p4-r3'
$zip = (Resolve-Path -LiteralPath $ArchivePath).Path
if ((Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash -ne $expectedZipSha) { throw 'NASADEM ZIP SHA-256 does not match the registered source.' }
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [IO.Compression.ZipFile]::OpenRead($zip)
try {
    $entries = @($archive.Entries | Where-Object { $_.FullName -ceq 'n24e121.hgt' })
    if ($entries.Count -ne 1 -or $entries[0].Length -ne $expectedLength) { throw 'ZIP must contain exactly one registered n24e121.hgt member with the expected size.' }
    $entry = $entries[0]
    $dem = Join-Path $repo 'artifacts/diagnostics/wp2-p4-r3/n24e121.hgt'
    $directory = Split-Path -Parent $dem
    $relative = 'artifacts/diagnostics/wp2-p4-r3'.Split('/')
    $cursor = $repo
    foreach ($part in $relative) {
        $cursor = Join-Path $cursor $part
        if (Test-Path -LiteralPath $cursor) {
            if ((Get-Item -LiteralPath $cursor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Fixture path contains a reparse point: $cursor" }
        } else { New-Item -ItemType Directory -Path $cursor | Out-Null }
    }
    if (Test-Path -LiteralPath $dem) {
        $existing = Get-Item -LiteralPath $dem -Force
        if (($existing.Attributes -band [IO.FileAttributes]::ReparsePoint) -or $existing.Length -ne $expectedLength -or (Get-FileHash -LiteralPath $dem -Algorithm SHA256).Hash -ne $expectedDemSha) { throw "Existing fixture differs from the registered data; refusing to overwrite: $dem" }
        Write-Output "Verified existing fixture (no write): $dem"
        exit 0
    }
    New-Item -ItemType Directory -Force -Path $temp | Out-Null
    $stage = Join-Path $temp ("n24e121-$([guid]::NewGuid().ToString('N')).hgt")
    $inputStream = $entry.Open()
    $output = [IO.File]::Open($stage, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
    $sha = [Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        $buffer = New-Object byte[] (1024 * 1024); $length = 0L
        while (($read = $inputStream.Read($buffer, 0, $buffer.Length)) -gt 0) { $output.Write($buffer, 0, $read); $sha.AppendData($buffer, 0, $read); $length += $read }
        $output.Flush($true)
        $actualSha = [Convert]::ToHexString($sha.GetHashAndReset())
    } finally { $inputStream.Dispose(); $output.Dispose(); $sha.Dispose() }
    if ($length -ne $expectedLength -or $actualSha -ne $expectedDemSha) { Remove-Item -LiteralPath $stage -Force; throw 'Extracted HGT does not match registered size and SHA-256.' }
    $created = $false
    $inputStream = $null
    $output = $null
    try {
        $inputStream = [IO.File]::OpenRead($stage)
        $output = [IO.File]::Open($dem, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None); $created = $true
        $inputStream.CopyTo($output); $output.Flush($true)
    } catch {
        if ($inputStream) { try { $inputStream.Dispose() } catch {}; $inputStream = $null }
        if ($output) { try { $output.Dispose() } catch {}; $output = $null }
        if ($created -and (Test-Path -LiteralPath $dem)) { Remove-Item -LiteralPath $dem -Force }
        throw
    } finally { if ($inputStream) { $inputStream.Dispose() }; if ($output) { $output.Dispose() }; Remove-Item -LiteralPath $stage -Force -ErrorAction SilentlyContinue }
    if ((Get-Item -LiteralPath $dem).Length -ne $expectedLength -or (Get-FileHash -LiteralPath $dem -Algorithm SHA256).Hash -ne $expectedDemSha) { throw 'Installed fixture failed post-write verification.' }
    Write-Output "Restored and verified fixture: $dem"
} finally { $archive.Dispose() }
