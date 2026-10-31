param([string]$EditorRoot = (Join-Path $PSScriptRoot '..\XuanYu.Editor.UI'))
$ErrorActionPreference = 'Stop'
$EditorRoot = (Resolve-Path -LiteralPath $EditorRoot).Path
$legal = @{
    'Panels/Navigation/Hierarchy/HierarchyWorkspace.axaml' = @('HierarchyScrollHost')
    'Panels/Navigation/ProjectTree/ProjectWorkspace.axaml' = @('ProjectScrollHost')
    'Panels/Status/LogDetails/LogDetailPanel.axaml' = @('')
    'Panels/Tools/WorkspaceModules/Top.axaml' = @('ContextToolScrollHost')
    'Panels/Tools/Tabs/TopTabStripTemplate.axaml' = @('TabScroller')
    'Panels/Inspector/InspectorPanels/InspectorPanel.axaml' = @('InspectorContentScrollHost')
    'Infrastructure/Diagnostics/DiagnosticDetailPanel.axaml' = @('')
    'ViewportUI/Diagnostics/Placement/DiagnosticFloatingCard.axaml' = @('')
}
$violations = [Collections.Generic.List[string]]::new()
foreach ($file in Get-ChildItem -LiteralPath $EditorRoot -Recurse -Filter '*.axaml') {
    $relative = $file.FullName.Substring($EditorRoot.TrimEnd('\').Length + 1).Replace('\','/')
    $text = Get-Content -Raw -LiteralPath $file.FullName
    $matches = [regex]::Matches($text, '(?is)<ScrollViewer\b(?<attrs>[^>]*)>')
    foreach ($match in $matches) {
        $name = [regex]::Match($match.Groups['attrs'].Value, 'x:Name="([^"]*)"').Groups[1].Value
        $allowed = $legal.ContainsKey($relative) -and $legal[$relative] -contains $name
        if (-not $allowed) { $violations.Add("$relative::$name") }
    }
}
if ($violations.Count -gt 0) {
    $violations | ForEach-Object { Write-Error "EDITOR-UI SCROLL VIOLATION: $_" }
    exit 1
}
Write-Output 'EDITOR-UI SCROLL AUDIT: PASS'
