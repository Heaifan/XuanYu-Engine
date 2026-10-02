function Test-Deadlock($State, [string]$WaitText) {
    $edges = @()
    foreach ($item in ($WaitText -split ',' | Where-Object { $_ })) {
        $parts = $item -split '>'
        if ($parts.Count -ne 2) { Fail WAIT_EDGE_INVALID $item }
        $edges += [pscustomobject]@{ from = $parts[0].Trim(); to = $parts[1].Trim() }
    }
    foreach ($start in @($edges.from | Select-Object -Unique)) {
        $node = $start; $path = @()
        for ($i = 0; $i -le $edges.Count; $i++) {
            if ($path -contains $node) { Fail CIRCULAR_HANDOFF_DEPENDENCY (($path + $node) -join ' -> ') }
            $path += $node
            $next = @($edges | Where-Object from -eq $node | Select-Object -First 1)
            if (!$next) { break }
            $node = [string]$next.to
        }
    }
    'DEADLOCK CHECK PASS'
}
