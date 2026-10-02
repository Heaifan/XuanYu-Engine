[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$StatePath,
    [Parameter(Mandatory)][ValidateSet('xye','xyui','integration','governance')][string]$Scope,
    [string]$CoordinatorScope,
    [string]$Field,
    [string]$Value,
    [string]$Transition,
    [string]$ExceptionStatus
)

$ErrorActionPreference = 'Stop'
Write-Output 'HANDOFF_STATE_CONTROL_REMOVED'
Write-Output 'Handoff only records events; lifecycle state writes and transitions are retired.'
exit 1
