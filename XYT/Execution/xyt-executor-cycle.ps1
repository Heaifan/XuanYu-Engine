function Find-XytCycle([string]$Node, [string[]]$Path, [hashtable]$Edges) {
    if ($Path -contains $Node) { return (($Path + $Node) -join ' -> ') }
    foreach ($next in @($Edges[$Node])) {
        if (!$Edges.ContainsKey([string]$next)) { continue }
        $cycle = Find-XytCycle ([string]$next) ($Path + $Node) $Edges
        if ($cycle) { return $cycle }
    }
}

function Find-XytDependencyCycle([pscustomobject]$Plan) {
    $edges = @{}
    foreach ($node in @($Plan.tests)) { $edges[[string]$node.testId] = @($node.dependsOn | Where-Object { $_ }) }
    foreach ($node in @($Plan.tests)) {
        $cycle = Find-XytCycle ([string]$node.testId) @() $edges
        if ($cycle) { return $cycle }
    }
}
