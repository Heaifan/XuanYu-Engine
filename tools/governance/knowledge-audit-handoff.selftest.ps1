$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$required = @(
    'AGENTS.md',
    'docs\governance\agent-completion-report-standard.md',
    'docs\dev-rules.md',
    'docs\codex-fast-execution-profile.md',
    'docs\governance\sync-handoff-sop.md',
    'docs\knowledge\README.md'
)
$marker = 'KNOWLEDGE / EXPERIENCE AUDIT HANDOFF'
$requiredText = 'CHATGPT KNOWLEDGE AUDIT REQUIRED'
foreach ($relative in $required) {
    $path = Join-Path $root $relative
    if (-not (Test-Path -LiteralPath $path)) { throw "Missing report norm: $relative" }
    $content = Get-Content -Raw -LiteralPath $path
    if ($content -notmatch [regex]::Escape($marker)) { throw "Missing marker: $relative" }
    if ($content -notmatch [regex]::Escape($requiredText)) { throw "Missing required audit text: $relative" }
}
$canonical = Get-Content -Raw (Join-Path $root 'docs\governance\knowledge-audit-handoff.md')
foreach ($term in @('Root Cause', 'Changed Files', 'Candidate Lessons', 'SEARCH EXISTING', 'UPDATE / STRENGTHEN', 'CREATE', 'RETIRE', 'NO DEPOSIT')) {
    if ($canonical -notmatch [regex]::Escape($term)) { throw "Canonical handoff missing: $term" }
}
if ($canonical -notmatch 'Knowledge is|Knowledge 是') { throw 'Knowledge definition missing' }
if ($canonical -notmatch 'Experience is|Experience 是') { throw 'Experience definition missing' }
Write-Output 'KNOWLEDGE AUDIT HANDOFF SELFTEST: PASS'
