[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptPath = Join-Path $PSScriptRoot 'handoff.ps1'
$root = Join-Path $env:TEMP ('xye-handoff-selftest-' + [guid]::NewGuid().ToString('N'))

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "SELF TEST FAILED: $Message" }
}

function Invoke-Handoff([string[]]$Arguments) {
    $output = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $scriptPath @Arguments 2>&1)
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Text = ($output -join [Environment]::NewLine) }
}

function Assert-Output($Result, [int]$ExitCode, [string]$Text) {
    Assert-True ($Result.ExitCode -eq $ExitCode) "expected exit $ExitCode, got $($Result.ExitCode): $($Result.Text)"
    Assert-True $Result.Text.Contains($Text) "expected '$Text' in: $($Result.Text)"
}

try {
    New-Item -ItemType Directory -Path $root -Force | Out-Null
    git -C $root init -b main | Out-Null
    git -C $root config user.email 'handoff-selftest@example.invalid'
    git -C $root config user.name 'handoff-selftest'
    Set-Content -LiteralPath (Join-Path $root 'README.md') -Value 'fixture'
    git -C $root add README.md
    git -C $root commit -m fixture | Out-Null

    $dotnet = Join-Path $root 'dotnet.cmd'
    Set-Content -LiteralPath $dotnet -Value '@echo 9.9.9-selftest'
    Set-Content -LiteralPath (Join-Path $root 'resolve-dotnet.ps1') -Value "Write-Output '$dotnet'"
    Set-Content -LiteralPath (Join-Path $root 'bootstrap.ps1') -Value ''
    @"
@{
    ActiveBranch = 'main'
    Remote = 'origin'
    CanonicalWorkspaces = @('$root')
    PreferredDotnetByDrive = @{}
    ResolverScript = 'resolve-dotnet.ps1'
    BootstrapScript = 'bootstrap.ps1'
}
"@ | Set-Content -LiteralPath (Join-Path $root 'HandoffConfig.psd1')

    $head = (git -C $root rev-parse HEAD).Trim()
    $stateDir = Join-Path $root '.git\xye-handoff'
    New-Item -ItemType Directory -Path $stateDir -Force | Out-Null
    $state = [ordered]@{
        branch = 'main'; baselineHead = $head; workspace = $root; active = $true
        mode = 'development'; coordinatorScope = $null
    }
    $state | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $stateBefore = Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')

    New-Item -ItemType Directory -Path (Join-Path $root 'xyui') -Force | Out-Null
    New-Item -ItemType Directory -Path (Join-Path $root 'XuanYu.World') -Force | Out-Null
    Set-Content -LiteralPath (Join-Path $root 'xyui\dirty.txt') -Value 'xyui'
    Set-Content -LiteralPath (Join-Path $root 'XuanYu.World\dirty.txt') -Value 'xye'
    $statusBefore = @(git -C $root status --porcelain=v1 --untracked-files=all)

    $result = Invoke-Handoff @('-Mode', 'join', '-Scope', 'xyui', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF JOIN PASS'
    Assert-Output $result 0 'OwnDirty'
    Assert-Output $result 0 'ForeignDirty'
    Assert-Output $result 0 'Scope       : xyui'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -eq $stateBefore) 'join changed state.json'

    $result = Invoke-Handoff @('-Mode', 'join', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'Scope       : xye'
    Assert-True (@(git -C $root status --porcelain=v1 --untracked-files=all) -join "`n" -eq ($statusBefore -join "`n")) 'join changed dirty files'
    Assert-True ((git -C $root rev-parse HEAD).Trim() -eq $head) 'join changed HEAD'

    $result = Invoke-Handoff @('-Mode', 'status', '-Scope', 'xye', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 0 'HANDOFF STATUS PASS'
    Assert-True ((git -C $root rev-parse HEAD).Trim() -eq $head) 'status changed HEAD'
    Assert-True (@(git -C $root status --porcelain=v1 --untracked-files=all) -join "`n" -eq ($statusBefore -join "`n")) 'status changed dirty files'
    Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $stateDir 'state.json')) -eq $stateBefore) 'status changed state.json'

    $state.mode = 'convergence'; $state.coordinatorScope = 'xye'
    $state | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'join', '-Scope', 'xyui', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'CONVERGENCE_EXCLUSIVE'

    $state.mode = 'development'; $state.coordinatorScope = $null
    $state | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stateDir 'state.json')
    $result = Invoke-Handoff @('-Mode', 'prepare', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'ACTIVE_WAVE'
    $result = Invoke-Handoff @('-Mode', 'close', '-RepositoryRoot', $root, '-AllowTestWorkspace')
    Assert-Output $result 1 'DIRTY_ON_CLOSE'
    Write-Host 'HANDOFF SELF TEST PASS'
}
finally {
    if (Test-Path -LiteralPath $root) { Remove-Item -LiteralPath $root -Recurse -Force }
}
