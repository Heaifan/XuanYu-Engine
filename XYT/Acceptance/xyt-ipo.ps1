[CmdletBinding()]
param(
    [Parameter(Mandatory)][string[]]$Capability,
    [string[]]$ChangeSet = @(), [string[]]$RequiredP4Scope = @(),
    [Parameter(Mandatory)][string]$Version, [Parameter(Mandatory)][string]$Commit,
    [Parameter(Mandatory)][string]$Branch,
    [ValidateSet('P4 PENDING','PASS','FAIL')][string]$UserVerdict = 'P4 PENDING',
    [string]$MappingPath = '', [string]$OutputPath = ''
)
$ErrorActionPreference = 'Stop'
$mapping = if ($MappingPath) { $MappingPath } else { Join-Path $PSScriptRoot 'xyt-ipo.mapping.ps1' }
$templates = & $mapping
$keys = @($Capability | ForEach-Object { $_ -split '[,;]' } | ForEach-Object { $_.Trim().ToLowerInvariant() } | Where-Object { $_ } | Select-Object -Unique)
if (!$keys.Count) { throw 'At least one Capability is required.' }
$unknown = @($keys | Where-Object { !$templates.ContainsKey($_) })
if ($unknown.Count) { throw "Unknown Capability: $($unknown -join ', ')" }
$items = foreach ($key in $keys) {
    $template = $templates[$key]
    [ordered]@{ '序号'=$template.Id; '路径'=$template.Path; '输入 I'=$template.Input; '过程 P'=$template.Process; '输出 O'=$template.Output; '判定'=if ($UserVerdict -eq 'P4 PENDING') {'P4 PENDING'} else {$UserVerdict}; Capability=$key }
}
$document = [ordered]@{ Schema='XYT-I/P4-IPO-v1'; Capability=$keys; 'Change Set'=@($ChangeSet | ForEach-Object { $_ -split '[,;]' } | ForEach-Object { $_.Trim() } | Where-Object { $_ }); 'Required P4 Scope'=@($RequiredP4Scope | ForEach-Object { $_ -split '[,;]' } | ForEach-Object { $_.Trim() } | Where-Object { $_ }); Version=$Version; Commit=$Commit; Branch=$Branch; 'P4 Rule'='最终判定只能由用户确认；生成器不得自动填写 P4 PASS'; Items=@($items) }
$json = $document | ConvertTo-Json -Depth 10
if ($OutputPath) { $json | Set-Content -LiteralPath $OutputPath -Encoding UTF8 } else { $json }
Write-Output "XYT IPO ITEMS = $(@($items).Count)"
Write-Output "P4 STATUS = $(if ($UserVerdict -eq 'P4 PENDING') {'P4 PENDING'} else {$UserVerdict})"
