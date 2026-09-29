[CmdletBinding()]
param(
    [ValidateSet('P3-01','P3-02','P3-03','P3-04')][string]$Capability='P3-01',
    [string]$AppCommand='', [string[]]$AppArgument=@(), [string]$AppScript='',
    [int]$TimeoutSeconds=30, [string]$EvidenceRoot='', [switch]$FakeHarnessFault, [switch]$ResizePulse
)
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$params=@{Capability=$Capability;AppCommand=$AppCommand;AppArgument=$AppArgument;AppScript=$AppScript;TimeoutSeconds=$TimeoutSeconds;EvidenceRoot=$EvidenceRoot;FakeHarnessFault=$FakeHarnessFault;ResizePulse=$ResizePulse}
& (Join-Path $root 'XYT\Runtime\xyt-runtime.ps1') @params
exit $LASTEXITCODE
