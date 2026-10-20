<#
.SYNOPSIS
    PreToolUse hook on Edit|Write: denies a path the agent's role may not write.

.DESCRIPTION
    Item 1 of "Enforcement" in .claude/design/2026-09-30-agent-workflow.md.
    Reads the hook's JSON on stdin; exit 0 allows, exit 2 denies with the
    reason on stderr. The role comes from the agent's frontmatter, as -Role.

      coder       everything inside the repository except the denylist of
                  "What the coder may not write"
      tester      core/Tests/ only
      record      every session, from .claude/settings.json (point 4 of the
                  workflow review plan): decides DECISIONS-OUTSIDE-SPEC.md and
                  PLAN.md only and allows every other path, inside the
                  repository or not. An Edit of either passes when its
                  new_string begins with a non-empty old_string; in the
                  register, also when it replaces one "**Ruling:**" line with
                  another, the line the decider updates under an entry (the
                  user's choice when point 4's audit found the decider doing
                  so); a Write only when the file does not exist.

    A path outside the repository is denied to every role but record. Writes
    made through a shell are not seen by this hook: it guards the Edit and
    Write tools only.
#>
param(
    [string]$Role,
    [switch]$SelfTest
)

$CoderDenied = @(
    'docs/', 'core/Tests/', '.claude/agents/', '.claude/hooks/', '.claude/skills/',
    'PLAN.md', 'DECISIONS-OUTSIDE-SPEC.md', 'SPEC-QUESTIONS.md', 'REVIEW-REQUEST.md',
    'PROMOTIONS.md', 'RULINGS.md', 'core/BannedSymbols.txt',
    '.claude/settings.json', '.claude/settings.local.json'
)

# An entry ending in '/' is a folder and matches itself and everything under it.
function Test-Listed([string]$Rel, [string[]]$List) {
    foreach ($e in $List) {
        if ($e.EndsWith('/')) {
            if ($Rel -ieq $e.TrimEnd('/') -or $Rel.StartsWith($e, [StringComparison]::OrdinalIgnoreCase)) { return $true }
        } elseif ($Rel -ieq $e) { return $true }
    }
    return $false
}

$RecordFiles = @('DECISIONS-OUTSIDE-SPEC.md', 'PLAN.md')

# The record role: the register and the plan are written by insertion, in every
# session; every other path is another role's business, or none.
function Get-RecordDenial([string]$Tool, $In, [string]$Full, [string]$RootFull) {
    if (-not $Full.StartsWith($RootFull, [StringComparison]::OrdinalIgnoreCase)) { return $null }
    $rel = $Full.Substring($RootFull.Length).Replace('\', '/')
    if (-not (Test-Listed $rel $RecordFiles)) { return $null }
    if ($Tool -eq 'Write') {
        if (Test-Path -LiteralPath $Full) { return "path-guard: $rel exists; the record is written by insertion, not by Write" }
        return $null
    }
    $old = [string]$In.tool_input.old_string
    $new = [string]$In.tool_input.new_string
    # The exemption is one ruling line of the register, replaced by one ruling
    # line: nothing before it, nothing after it, not PLAN.md.
    $oneRuling = { param($s) $s.StartsWith('**Ruling:**', [StringComparison]::Ordinal) -and $s -notmatch "[`r`n]" }
    if ($rel -ieq 'DECISIONS-OUTSIDE-SPEC.md' -and (& $oneRuling $old) -and (& $oneRuling $new)) { return $null }
    if ($old -eq '' -or -not $new.StartsWith($old, [StringComparison]::Ordinal)) {
        return "path-guard: $rel is written by insertion; new_string must begin with a non-empty old_string"
    }
    return $null
}

# Returns $null to allow, or the reason for denying.
function Get-Denial([string]$Role, [string]$Json, [string]$Root) {
    try { $in = $Json | ConvertFrom-Json } catch { return 'path-guard: input is not JSON' }
    $tool = $in.tool_name
    if ($tool -ne 'Edit' -and $tool -ne 'Write') { return $null }
    $path = $in.tool_input.file_path
    if (-not $path) { return "path-guard: $tool without file_path" }

    $rootFull = [IO.Path]::GetFullPath($Root).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    $full = [IO.Path]::GetFullPath($path, $rootFull)
    if ($Role -eq 'record') { return (Get-RecordDenial $tool $in $full $rootFull) }
    if (-not $full.StartsWith($rootFull, [StringComparison]::OrdinalIgnoreCase)) {
        return "path-guard: $path is outside the repository"
    }
    $rel = $full.Substring($rootFull.Length).Replace('\', '/')

    switch ($Role) {
        'coder' {
            if (Test-Listed $rel $CoderDenied) { return "path-guard: the coder may not write $rel" }
        }
        'tester' {
            if (-not (Test-Listed $rel @('core/Tests/'))) { return "path-guard: the tester writes core/Tests/ only, not $rel" }
        }
        'record' { return $null }
        default { return "path-guard: unknown role '$Role'" }
    }
    return $null
}

if ($SelfTest) {
    $root = 'F:\repo'
    function J($tool, $path, $old = $null, $new = $null) {
        $ti = @{ file_path = $path }
        if ($null -ne $old) { $ti.old_string = $old; $ti.new_string = $new }
        @{ tool_name = $tool; tool_input = $ti } | ConvertTo-Json -Compress
    }
    $cases = @(
        @('coder', (J 'Write' 'F:\repo\docs\x'), 'deny'),
        @('coder', (J 'Write' 'docs/x'), 'deny'),
        @('coder', (J 'Edit' 'F:\repo\DOCS\SIM-REQ.md' 'a' 'b'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\core\Runtime\..\..\docs\x'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\core\Tests\A.cs'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\PLAN.md'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\RULINGS.md'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\core\BannedSymbols.txt'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\.claude\hooks\path-guard.ps1'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\.claude\agents\coder.md'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\.claude\skills\plan-next\SKILL.md'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\.claude\settings.json'), 'deny'),
        @('coder', (J 'Edit' 'F:\repo\.claude\settings.local.json' 'a' 'b'), 'deny'),
        @('coder', (J 'Write' 'F:\elsewhere\x.cs'), 'deny'),
        @('coder', (J 'Write' 'F:\repo-other\x.cs'), 'deny'),
        @('coder', (J 'Write' 'F:\repo\core\Runtime\Systems\TickLoop.cs'), 'allow'),
        @('coder', (J 'Write' 'F:\repo\docsx\a.md'), 'allow'),
        @('coder', (J 'Edit' 'F:\repo\AGENTS.md' 'a' 'b'), 'allow'),
        @('tester', (J 'Write' 'F:\repo\core\Tests\AC02.cs'), 'allow'),
        @('tester', (J 'Write' 'F:\repo\core\Runtime\X.cs'), 'deny'),
        @('tester', (J 'Write' 'F:\repo\core\TestsX\A.cs'), 'deny'),
        @('reviewer', (J 'Write' 'F:\repo\core\Runtime\X.cs'), 'deny'),
        @('coder', '{not json', 'deny'),
        @('coder', '{"tool_name":"Read","tool_input":{"file_path":"F:\\repo\\docs\\x"}}', 'allow')
    )
    $failed = 0
    foreach ($c in $cases) {
        $got = if (Get-Denial $c[0] $c[1] $root) { 'deny' } else { 'allow' }
        if ($got -ne $c[2]) { $failed++; Write-Host "FAIL $($c[0]) expected $($c[2]), got ${got}: $($c[1])" }
    }
    # The record role, on a real directory: Write is judged by whether the file
    # exists, and a denial carries record's reason, not another.
    $r = Join-Path ([IO.Path]::GetTempPath()) "path-guard-record-$PID"
    $null = New-Item -ItemType Directory -Force $r
    Set-Content (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') 'x'
    Set-Content (Join-Path $r 'PLAN.md') 'x'
    $recordCases = @(
        @((J 'Edit' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') 'anchor' "anchor`nmore"), 'allow'),
        @((J 'Edit' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') 'a word' 'another word'), 'deny'),
        @((J 'Edit' (Join-Path $r 'decisions-outside-spec.md') 'a word' 'another word'), 'deny'),
        @((J 'Edit' (Join-Path $r 'PLAN.md') '' 'new'), 'deny'),
        @((J 'Edit' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') '**Ruling:** none yet.' '**Ruling:** R-200 — ratified.'), 'allow'),
        @((J 'Edit' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') 'text **Ruling:** none' 'text changed'), 'deny'),
        @((J 'Edit' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') "**Ruling:** none yet.`n`n### D-200 · next entry" '**Ruling:** R-200 — ratified.'), 'deny'),
        @((J 'Edit' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md') '**Ruling:** none yet.' 'anything else'), 'deny'),
        @((J 'Edit' (Join-Path $r 'PLAN.md') '**Ruling:** none yet.' '**Ruling:** R-200 — ratified.'), 'deny'),
        @((J 'Write' (Join-Path $r 'PLAN.md')), 'deny'),
        @((J 'Write' (Join-Path $r 'DECISIONS-OUTSIDE-SPEC.md')), 'deny'),
        @((J 'Write' (Join-Path $r 'sub\PLAN.md')), 'allow'),
        @((J 'Edit' (Join-Path $r 'README.md') 'a' 'b'), 'allow'),
        @((J 'Edit' (Join-Path $r 'SPEC-QUESTIONS.md') 'a' 'b'), 'allow'),
        @((J 'Write' 'C:\elsewhere\scratch\msg.txt'), 'allow')
    )
    foreach ($c in $recordCases) {
        $reason = Get-Denial 'record' $c[0] $r
        $got = if ($reason) { 'deny' } else { 'allow' }
        if ($got -ne $c[1]) { $failed++; Write-Host "FAIL record expected $($c[1]), got ${got}: $($c[0])" }
        if ($reason -and $reason -notmatch 'insertion') { $failed++; Write-Host "FAIL record denied for another reason: $reason" }
    }
    $fresh = Join-Path $r 'new-plan'
    $null = New-Item -ItemType Directory -Force $fresh
    if (Get-Denial 'record' (J 'Write' (Join-Path $fresh 'PLAN.md')) $fresh) { $failed++; Write-Host 'FAIL record: a Write of a PLAN.md that does not exist was denied' }
    Remove-Item -Recurse -Force $r
    $cases += $recordCases
    $cases += , 'fresh'
    # The entry point itself: stdin, -Role, exit code.
    $p = Join-Path ([IO.Path]::GetTempPath()) "path-guard-$PID"
    $null = New-Item -ItemType Directory -Force $p
    $env:CLAUDE_PROJECT_DIR = $p
    foreach ($e in @(@((J 'Write' (Join-Path $p 'docs\x')), 2), @((J 'Write' (Join-Path $p 'core\x.cs')), 0))) {
        $e[0] | pwsh -NoProfile -File $PSCommandPath -Role coder 2>$null
        if ($LASTEXITCODE -ne $e[1]) { $failed++; Write-Host "FAIL entry point: exit $LASTEXITCODE, expected $($e[1])" }
    }
    Remove-Item -Recurse -Force $p
    Write-Host "path-guard self-test: $($cases.Count + 2) cases, $failed failed"
    exit ([int]($failed -ne 0))
}

$json = [Console]::In.ReadToEnd()
$root = if ($env:CLAUDE_PROJECT_DIR) { $env:CLAUDE_PROJECT_DIR } else { (Get-Location).Path }
$reason = Get-Denial $Role $json $root
if ($reason) { [Console]::Error.WriteLine($reason); exit 2 }
exit 0
