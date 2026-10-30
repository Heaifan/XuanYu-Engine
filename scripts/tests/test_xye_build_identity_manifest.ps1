$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$buildScript = Join-Path $repositoryRoot 'scripts\xye-build.ps1'
$fixtureRoot = Join-Path $repositoryRoot ('obj\xye-build-manifest-tests-' + [guid]::NewGuid().ToString('N'))
$outsideRoot = Join-Path $fixtureRoot 'outside-fixture'
$env:TEMP = Join-Path $repositoryRoot 'obj'
$env:TMP = $env:TEMP
$record = [pscustomobject]@{
    Identity = 'identity-test'
    SourceFingerprint = 'source-test'
    AssetFingerprint = 'assets-test'
    SDK = '10.0.400'
    Configuration = 'Release'
    Target = 'test.slnx'
    Outputs = @('bin/test.dll')
    OutputHashes = @{ 'bin/test.dll' = 'hash-test' }
}

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
}

function Assert-Throws([scriptblock]$Action, [string]$ExpectedText, [string]$Name) {
    $message = ''
    try { & $Action } catch { $message = $_.Exception.Message }
    if ([string]::IsNullOrWhiteSpace($message) -or $message -notmatch [regex]::Escape($ExpectedText)) {
        throw "$Name did not fail with expected text '$ExpectedText'; actual='$message'."
    }
    Write-Output "PASS: $Name"
}

try {
    New-Item -ItemType Directory -Path $fixtureRoot -Force | Out-Null
    New-Item -ItemType Directory -Path $outsideRoot -Force | Out-Null

    $tokens = $null
    $parseErrors = $null
    $ast = [System.Management.Automation.Language.Parser]::ParseFile($buildScript, [ref]$tokens, [ref]$parseErrors)
    if ($parseErrors.Count) { throw "Build script parse failed: $($parseErrors[0].Message)" }
    $requiredFunctions = @('Assert-RepositoryPath', 'Ensure-RepositoryDirectory', 'Write-BuildIdentityManifest')
    foreach ($name in $requiredFunctions) {
        $node = $ast.Find({ param($item) $item -is [System.Management.Automation.Language.FunctionDefinitionAst] -and $item.Name -eq $name }, $true)
        if (-not $node) { throw "Required testable build function is missing: $name" }
        . ([scriptblock]::Create($node.Extent.Text))
    }

    $missingRepo = Join-Path $fixtureRoot 'missing-parent-repo'
    New-Item -ItemType Directory -Path $missingRepo | Out-Null
    $missingManifest = Join-Path $missingRepo 'obj\.xye-build-identity.json'
    Write-BuildIdentityManifest -RepositoryRoot $missingRepo -ManifestPath $missingManifest -Record $record
    Assert-True (Test-Path -LiteralPath $missingManifest -PathType Leaf) 'Missing parent was not created and written.'
    $readBack = Get-Content -LiteralPath $missingManifest -Raw | ConvertFrom-Json
    Assert-True ($readBack.Identity -eq $record.Identity) 'Identity changed in serialized manifest.'
    Assert-True ($readBack.Target -eq $record.Target) 'Target changed in serialized manifest.'
    Assert-True ($readBack.OutputHashes.'bin/test.dll' -eq 'hash-test') 'Output hash contract changed.'
    Assert-True (@($readBack.PSObject.Properties.Name) -join ',' -eq 'Identity,SourceFingerprint,AssetFingerprint,SDK,Configuration,Target,Outputs,OutputHashes') 'Manifest property format changed.'
    $firstHash = (Get-FileHash -LiteralPath $missingManifest -Algorithm SHA256).Hash
    Write-BuildIdentityManifest -RepositoryRoot $missingRepo -ManifestPath $missingManifest -Record $record
    $secondHash = (Get-FileHash -LiteralPath $missingManifest -Algorithm SHA256).Hash
    Assert-True ($firstHash -eq $secondHash) 'A valid existing manifest changed under the same identity.'
    Write-Output 'PASS: missing parent creation and existing manifest identity/format'

    $existingRepo = Join-Path $fixtureRoot 'existing-parent-repo'
    $existingObj = Join-Path $existingRepo 'obj'
    New-Item -ItemType Directory -Path $existingObj -Force | Out-Null
    $existingManifest = Join-Path $existingObj '.xye-build-identity.json'
    Write-BuildIdentityManifest -RepositoryRoot $existingRepo -ManifestPath $existingManifest -Record $record
    Assert-True (Test-Path -LiteralPath $existingManifest -PathType Leaf) 'Existing parent path did not accept the manifest.'
    Write-Output 'PASS: existing parent directory'

    $escapePath = Join-Path $outsideRoot 'escaped.json'
    Assert-Throws { Write-BuildIdentityManifest -RepositoryRoot $missingRepo -ManifestPath $escapePath -Record $record } 'outside the repository' 'lexical path escape'
    Assert-True (-not (Test-Path -LiteralPath $escapePath)) 'Lexical escape wrote outside its fixture repository.'

    $linkedRepo = Join-Path $fixtureRoot 'linked-repo'
    New-Item -ItemType Directory -Path $linkedRepo -Force | Out-Null
    $linkedObj = Join-Path $linkedRepo 'obj'
    New-Item -ItemType Junction -Path $linkedObj -Target $outsideRoot | Out-Null
    $linkedManifest = Join-Path $linkedObj '.xye-build-identity.json'
    Assert-Throws { Write-BuildIdentityManifest -RepositoryRoot $linkedRepo -ManifestPath $linkedManifest -Record $record } 'reparse point' 'junction escape'
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $outsideRoot '.xye-build-identity.json'))) 'Junction escape wrote through the link.'

    $ioRepo = Join-Path $fixtureRoot 'io-failure-repo'
    New-Item -ItemType Directory -Path (Join-Path $ioRepo 'obj') -Force | Out-Null
    $ioManifest = Join-Path $ioRepo 'obj\.xye-build-identity.json'
    $failingWriter = { param($Path, $Content) throw [System.IO.IOException]::new('injected IO write failure') }
    Assert-Throws { Write-BuildIdentityManifest -RepositoryRoot $ioRepo -ManifestPath $ioManifest -Record $record -Writer $failingWriter } 'injected IO write failure' 'IO write failure reporting'
    Assert-True (-not (Test-Path -LiteralPath $ioManifest)) 'Injected writer unexpectedly created a manifest.'

    $functionText = @($requiredFunctions | ForEach-Object {
        $functionNode = $ast.Find({ param($item) $item -is [System.Management.Automation.Language.FunctionDefinitionAst] -and $item.Name -eq $_ }, $true)
        $functionNode.Extent.Text
    }) -join "`n`n"
    $childRepo = Join-Path $fixtureRoot 'child-io-repo'
    New-Item -ItemType Directory -Path $childRepo -Force | Out-Null
    $childManifest = Join-Path $childRepo 'obj\.xye-build-identity.json'
    $quotedRepo = $childRepo.Replace("'", "''")
    $quotedManifest = $childManifest.Replace("'", "''")
    $childScript = Join-Path $fixtureRoot 'io-failure-child.ps1'
    $childBody = @"
`$ErrorActionPreference = 'Stop'
$functionText
`$record = [pscustomobject]@{ Identity='identity-test'; SourceFingerprint='source-test'; AssetFingerprint='assets-test'; SDK='10.0.400'; Configuration='Release'; Target='test.slnx'; Outputs=@(); OutputHashes=@{} }
`$writer = { param(`$Path, `$Content) throw [System.IO.IOException]::new('injected IO write failure') }
Write-BuildIdentityManifest -RepositoryRoot '$quotedRepo' -ManifestPath '$quotedManifest' -Record `$record -Writer `$writer
"@
    [IO.File]::WriteAllText($childScript, $childBody, [Text.UTF8Encoding]::new($false))
    $childStdout = Join-Path $fixtureRoot 'io-failure-child.stdout.log'
    $childStderr = Join-Path $fixtureRoot 'io-failure-child.stderr.log'
    $childProcess = Start-Process -FilePath powershell.exe -ArgumentList @('-NoProfile','-ExecutionPolicy','Bypass','-File',$childScript) -Wait -PassThru -NoNewWindow -RedirectStandardOutput $childStdout -RedirectStandardError $childStderr
    $childExit = $childProcess.ExitCode
    $childOutput = @((Get-Content -LiteralPath $childStdout -ErrorAction SilentlyContinue) + (Get-Content -LiteralPath $childStderr -ErrorAction SilentlyContinue))
    Assert-True ($childExit -ne 0) 'Unhandled manifest IO failure returned success from the script process.'
    Assert-True (($childOutput -join "`n") -match 'Failed to write build identity manifest') 'Child process omitted the manifest write error.'
    Assert-True (-not (Test-Path -LiteralPath $childManifest)) 'Failing child process created a manifest.'
    Write-Output 'PASS: IO failure is explicit and exits nonzero'
} finally {
    if (Test-Path -LiteralPath $fixtureRoot -PathType Container) {
        Remove-Item -LiteralPath $fixtureRoot -Recurse -Force
    }
}
