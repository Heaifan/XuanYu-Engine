$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'xye-workspace.ps1'
$originalRoot = $env:XYE_WORKSPACE_ROOT
Import-Module (Join-Path $PSScriptRoot 'XyeWorkspace.psm1') -Force
$configuredRoot = Get-XyeWorkspaceRoot
$testRoot = Join-Path $configuredRoot (Join-Path 'Temp' ('selftest-' + [Guid]::NewGuid().ToString('N')))

try {
    $env:XYE_WORKSPACE_ROOT = $testRoot
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath -Action Initialize
    if ($LASTEXITCODE -ne 0) { throw 'Workspace initialization failed.' }
    foreach ($name in @('Sources','Builds','Diagnostics','Temp','Manifests')) {
        if (-not (Test-Path (Join-Path $testRoot $name) -PathType Container)) { throw "Missing workspace folder: $name" }
    }

    $blockedRoot = Join-Path $testRoot 'Temp\blocked-root-file'
    Set-Content -LiteralPath $blockedRoot -Value 'file blocks directory creation'
    $env:XYE_WORKSPACE_ROOT = $blockedRoot
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath -Action Initialize 2>&1 | Out-String | Out-Null
    if ($LASTEXITCODE -eq 0) { throw 'Unwritable workspace root unexpectedly passed.' }
    $env:XYE_WORKSPACE_ROOT = $testRoot
    Remove-Item -LiteralPath $blockedRoot -Force

    $requestPath = Join-Path $testRoot 'Temp\invoke.json'
    [pscustomobject]@{
        TaskType='WorkspaceSelfTest'; ExecutablePath='cmd.exe'
        ExecutableArguments=@('/c','echo %XYE_WORKSPACE_ROOT% & echo %TEMP%')
        RepositoryRoot=(Get-Location).Path; SourceRoot=(Get-Location).Path
    } | ConvertTo-Json | Set-Content -LiteralPath $requestPath -Encoding utf8
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath -Action Invoke -RequestFile $requestPath
    if ($LASTEXITCODE -ne 0) { throw 'Controlled child invocation failed.' }

    $manifest = Get-ChildItem (Join-Path $testRoot 'Manifests') -Filter '*.json' |
        Where-Object { (Get-Content $_.FullName -Raw | ConvertFrom-Json).TaskType -eq 'WorkspaceSelfTest' } |
        Select-Object -First 1
    if ($null -eq $manifest) { throw 'Task manifest was not created.' }
    $record = Get-Content $manifest.FullName -Raw | ConvertFrom-Json
    if ($record.Status -ne 'COMPLETED' -or $record.ExitCode -ne 0) { throw 'Completion state was not recorded.' }
    if (-not $record.BuildDirectory.StartsWith($testRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Build directory escaped the workspace root.'
    }
    if (-not $record.DiagnosticsDirectory.StartsWith($testRoot, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Diagnostics directory escaped the workspace root.'
    }
    $stdout = Get-Content (Join-Path $record.DiagnosticsDirectory 'stdout.log') -Raw
    if ($stdout -notmatch [regex]::Escape($testRoot) -or $stdout -notmatch '\\Temp\\') {
        throw 'Controlled child did not inherit its scoped workspace and TEMP paths.'
    }

    [pscustomobject]@{
        TaskType='WorkspaceFailureSelfTest'; ExecutablePath='cmd.exe'
        ExecutableArguments=@('/c','exit 7')
        RepositoryRoot=(Get-Location).Path; SourceRoot=(Get-Location).Path
    } | ConvertTo-Json | Set-Content -LiteralPath $requestPath -Encoding utf8
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath -Action Invoke -RequestFile $requestPath
    if ($LASTEXITCODE -ne 7) { throw 'Nonzero child exit code was not propagated.' }
    $failed = Get-ChildItem (Join-Path $testRoot 'Manifests') -Filter '*.json' |
        Where-Object { (Get-Content $_.FullName -Raw | ConvertFrom-Json).TaskType -eq 'WorkspaceFailureSelfTest' } |
        Select-Object -First 1
    $failedRecord = Get-Content $failed.FullName -Raw | ConvertFrom-Json
    if ($failedRecord.Status -ne 'FAILED' -or $failedRecord.ExitCode -ne 7) {
        throw 'Failed task state was not recorded.'
    }

    [pscustomobject]@{
        RunId='stale-run'; TaskType='WorkspaceInterruptSelfTest'; Status='ACTIVE'
        OwnerProcessId=2147483647; UpdatedUtc=[DateTimeOffset]::UtcNow.ToString('O')
        DirectoryBytes=@{}
        SourceDirectory=$testRoot; BuildDirectory=$testRoot; DiagnosticsDirectory=$testRoot
    } | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $testRoot 'Manifests\stale-run.json') -Encoding utf8
    $preview = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath -Action Preview -AsJson
    $previewItems = @($preview | ConvertFrom-Json)
    $stalePreview = $previewItems | Where-Object RunId -eq 'stale-run' | Select-Object -First 1
    if ($LASTEXITCODE -ne 0 -or $null -eq $stalePreview -or
        $stalePreview.ObservedStatus -notmatch 'INTERRUPTED' -or [long]$stalePreview.WorkspaceBytes -ne 0) {
        throw 'Read-only preview did not safely classify an abandoned ACTIVE task with incomplete size data.'
    }
    $global:LASTEXITCODE = 0
    Write-Output 'XYE workspace lifecycle self-test passed.'
}
finally {
    if ($null -eq $originalRoot) { Remove-Item Env:XYE_WORKSPACE_ROOT -ErrorAction SilentlyContinue }
    else { $env:XYE_WORKSPACE_ROOT = $originalRoot }
    if (Test-Path -LiteralPath $testRoot) { Remove-Item -LiteralPath $testRoot -Recurse -Force }
}
