[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$Mode = 'quick',
    [switch]$Help
)

$ErrorActionPreference = 'Stop'

function Resolve-XytMode([string]$Value) {
    switch ($Value.Trim().ToLowerInvariant()) {
        'quick' { return 'QUICK_VALIDATION' }
        'verify' { return 'QUICK_VALIDATION' }
        '快速验证' { return 'QUICK_VALIDATION' }
        'module' { return 'MODULE_CLOSEOUT' }
        'close-module' { return 'MODULE_CLOSEOUT' }
        '模块收口' { return 'MODULE_CLOSEOUT' }
        'global' { return 'GLOBAL_CLOSEOUT' }
        'close-global' { return 'GLOBAL_CLOSEOUT' }
        '全局收口' { return 'GLOBAL_CLOSEOUT' }
        default { return $null }
    }
}

function Write-XytReport([string]$ResolvedMode, [string]$Status) {
    Write-Output '================ XYT STARTUP REPORT ================'
    Write-Output 'Entry: xyt'
    Write-Output "Mode: $ResolvedMode"
    Write-Output "Status: $Status"
    Write-Output "Workspace: $PSScriptRoot"
    Write-Output "Timestamp: $([DateTime]::Now.ToString('o'))"
    Write-Output '======================================================'
}

if ($Help) {
    Write-Output 'Usage: xyt [quick|module|global]'
    Write-Output 'Modes: quick/快速验证, module/模块收口, global/全局收口'
    exit 0
}

$resolved = Resolve-XytMode $Mode
if ($null -eq $resolved) {
    Write-XytReport 'UNRECOGNIZED' 'INVALID_MODE'
    Write-Output "Error: Unknown XYT mode '$Mode'. Use quick, module, or global."
    exit 2
}

Write-XytReport $resolved 'EMPTY_RUN'
exit 0
