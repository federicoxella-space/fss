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
      documenter  DECISIONS-OUTSIDE-SPEC.md, PLAN.md, SPEC-QUESTIONS.md, and
                  only by an Edit whose new_string begins with its old_string

    A path outside the repository is denied to every role.
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
$DocumenterFiles = @('DECISIONS-OUTSIDE-SPEC.md', 'PLAN.md', 'SPEC-QUESTIONS.md')

# An entry ending in '/' is a folder and matches itself and everything under it.
function Test-Listed([string]$Rel, [string[]]$List) {
    foreach ($e in $List) {
        if ($e.EndsWith('/')) {
            if ($Rel -ieq $e.TrimEnd('/') -or $Rel.StartsWith($e, [StringComparison]::OrdinalIgnoreCase)) { return $true }
        } elseif ($Rel -ieq $e) { return $true }
    }
    return $false
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
        'documenter' {
            if (-not (Test-Listed $rel $DocumenterFiles)) { return "path-guard: the documenter may not write $rel" }
            if ($tool -ne 'Edit') { return 'path-guard: the documenter only inserts, by Edit' }
            $old = [string]$in.tool_input.old_string
            $new = [string]$in.tool_input.new_string
            if ($old -eq '' -or -not $new.StartsWith($old, [StringComparison]::Ordinal)) {
                return 'path-guard: insert-only; new_string must begin with a non-empty old_string'
            }
        }
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
        @('documenter', (J 'Edit' 'F:\repo\PLAN.md' 'anchor' "anchor`nmore"), 'allow'),
        @('documenter', (J 'Edit' 'F:\repo\DECISIONS-OUTSIDE-SPEC.md' 'x' 'xy'), 'allow'),
        @('documenter', (J 'Edit' 'F:\repo\SPEC-QUESTIONS.md' 'x' 'xy'), 'allow'),
        @('documenter', (J 'Edit' 'F:\repo\PLAN.md' 'anchor' 'changed'), 'deny'),
        @('documenter', (J 'Edit' 'F:\repo\PLAN.md' '' 'new'), 'deny'),
        @('documenter', (J 'Write' 'F:\repo\PLAN.md'), 'deny'),
        @('documenter', (J 'Edit' 'F:\repo\RULINGS.md' 'x' 'xy'), 'deny'),
        @('documenter', (J 'Edit' 'F:\repo\core\Runtime\X.cs' 'x' 'xy'), 'deny'),
        @('reviewer', (J 'Write' 'F:\repo\core\Runtime\X.cs'), 'deny'),
        @('coder', '{not json', 'deny'),
        @('coder', '{"tool_name":"Read","tool_input":{"file_path":"F:\\repo\\docs\\x"}}', 'allow')
    )
    $failed = 0
    foreach ($c in $cases) {
        $got = if (Get-Denial $c[0] $c[1] $root) { 'deny' } else { 'allow' }
        if ($got -ne $c[2]) { $failed++; Write-Host "FAIL $($c[0]) expected $($c[2]), got ${got}: $($c[1])" }
    }
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
