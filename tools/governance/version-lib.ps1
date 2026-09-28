Set-StrictMode -Version Latest

function ConvertTo-ProcessVersion {
    param([Parameter(Mandatory)][string]$Version)
    $m = [regex]::Match($Version.Trim(), '^v(?<a>\d+)\.(?<b>\d+)\.(?<c>\d+)\.(?<d>\d+)(?:-(?<suffix>[A-Za-z0-9][A-Za-z0-9.-]*))?$')
    if (-not $m.Success) { throw "Cannot parse process version: $Version" }
    [pscustomobject]@{
        Text = $Version.Trim(); A = [int]$m.Groups['a'].Value; B = [int]$m.Groups['b'].Value
        C = [int]$m.Groups['c'].Value; D = [int]$m.Groups['d'].Value
        Suffix = $m.Groups['suffix'].Value
    }
}

function Get-NextProcessVersion {
    param([Parameter(Mandatory)][string]$Current, [Parameter(Mandatory)][ValidateSet('FEATURE','FIX','STABILIZATION','GOVERNANCE')][string]$Type)
    $v = ConvertTo-ProcessVersion $Current
    if ($Type -eq 'GOVERNANCE') { throw 'GOVERNANCE does not advance product Process Version under the audited XYE historical rule.' }
    if ($Type -eq 'FIX' -and $v.Suffix -match '^fix(?<n>\d+)$') {
        return "v$($v.A).$($v.B).$($v.C).$($v.D)-fix$([int]$Matches['n'] + 1)"
    }
    $suffix = switch ($Type) { 'FEATURE' { 'r1' } 'FIX' { 'fix' } 'STABILIZATION' { 'stab' } }
    "v$($v.A).$($v.B).$($v.C).$($v.D + 1)-$suffix"
}
