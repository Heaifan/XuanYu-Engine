[CmdletBinding()]
param(
    [ValidateSet('P3-01','P3-02','P3-03','P3-04')][string]$Capability='P3-01', [string]$CandidateId='',
    [string]$AppCommand='', [string[]]$AppArgument=@(), [string]$AppScript='',
    [int]$TimeoutSeconds=30, [int]$StdoutDrainTimeoutSeconds=5, [int]$StderrDrainTimeoutSeconds=5,
    [int]$EvidenceWriteTimeoutSeconds=5, [string]$EvidenceRoot='', [switch]$FakeHarnessFault,
    [switch]$FakeStdoutDrainTimeout, [switch]$FakeStderrDrainTimeout, [switch]$NoGui, [switch]$ResizePulse
)
$root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$params=@{Capability=$Capability;CandidateId=$CandidateId;AppCommand=$AppCommand;AppArgument=$AppArgument;AppScript=$AppScript;TimeoutSeconds=$TimeoutSeconds;StdoutDrainTimeoutSeconds=$StdoutDrainTimeoutSeconds;StderrDrainTimeoutSeconds=$StderrDrainTimeoutSeconds;EvidenceWriteTimeoutSeconds=$EvidenceWriteTimeoutSeconds;EvidenceRoot=$EvidenceRoot;FakeHarnessFault=$FakeHarnessFault;FakeStdoutDrainTimeout=$FakeStdoutDrainTimeout;FakeStderrDrainTimeout=$FakeStderrDrainTimeout;NoGui=$NoGui;ResizePulse=$ResizePulse}
& (Join-Path $root 'XYT\Runtime\xyt-runtime.ps1') @params
exit $LASTEXITCODE
