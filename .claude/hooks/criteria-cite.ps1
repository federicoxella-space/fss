<#
.SYNOPSIS
    Checks that every rule of the criteria checklist cites a ruling or a
    register entry that exists.

.DESCRIPTION
    Point 1 of the workflow review plan (review B). A rule is a heading
    "### K<n> — ...", and its text runs to the next heading. Every rule must
    carry at least one identifier R-NNN or D-NNN, and every identifier must have
    a heading "### R-NNN" in RULINGS.md or "### D-NNN" in
    DECISIONS-OUTSIDE-SPEC.md. A line that begins with K<n> outside a heading is
    a rule the parser would skip, and is refused. Whether a citation says what
    the rule claims is not checked here: the code-reviewer reads it.

    Exit codes: 0 every rule cites only identifiers that exist; 1 a rule cites
    none, cites one that does not exist, or is not a heading, or the checklist
    holds no rule; 2 a file cannot be read.
#>
param(
    [string]$Path = '.claude/skills/plan-explain/criteria.md',
    [string]$Repo = (Get-Location).Path,
    [switch]$SelfTest
)

# Returns the list of problems; empty means the checklist passes.
function Get-Problems([string]$Checklist, [string]$Repo) {
    $known = @{}
    foreach ($f in @('RULINGS.md', 'DECISIONS-OUTSIDE-SPEC.md')) {
        $p = Join-Path $Repo $f
        if (-not (Test-Path -LiteralPath $p -PathType Leaf)) { return , @("cannot read $f") }
        foreach ($m in (Select-String -LiteralPath $p -Pattern '^### ([RD]-\d{3})\b')) { $known[$m.Matches[0].Groups[1].Value] = $true }
    }
    if (-not (Test-Path -LiteralPath $Checklist -PathType Leaf)) { return , @("cannot read $Checklist") }

    $problems = @()
    $rules = [ordered]@{}
    $current = $null
    foreach ($line in (Get-Content -LiteralPath $Checklist)) {
        if ($line -match '^#{1,6}\s') {
            $current = $null
            if ($line -match '^### (K\d+)\b') {
                $current = $Matches[1]
                if ($rules.Contains($current)) { $problems += "$current is a heading twice" }
                $rules[$current] = $line
            } elseif ($line -match '^#{1,6}\s+(K\d+)\b') { $problems += "$($Matches[1]) is a heading at the wrong level" }
            continue
        }
        if ($line -match '^\s*(K\d+)\b') { $problems += "a rule outside a heading: $($Matches[1])" }
        if ($current) { $rules[$current] += "`n$line" }
    }
    if ($rules.Count -eq 0) { $problems += 'the checklist holds no rule' }
    foreach ($k in $rules.Keys) {
        $ids = @([regex]::Matches($rules[$k], '\b[RD]-\d{3}\b') | ForEach-Object { $_.Value } | Select-Object -Unique)
        if ($ids.Count -eq 0) { $problems += "${k} cites nothing" }
        foreach ($id in $ids) { if (-not $known[$id]) { $problems += "${k} cites $id, which has no heading" } }
    }
    return , $problems
}

if ($SelfTest) {
    $dir = Join-Path ([IO.Path]::GetTempPath()) "criteria-cite-$PID"
    $null = New-Item -ItemType Directory -Force $dir
    Set-Content -Encoding utf8 (Join-Path $dir 'RULINGS.md') "# Rulings`n`n### R-101 — one`n`n### R-103 — two`n"
    Set-Content -Encoding utf8 (Join-Path $dir 'DECISIONS-OUTSIDE-SPEC.md') "# Register`n`n### D-121 · one`n"
    function List($name, $text) { $p = Join-Path $dir $name; Set-Content -Encoding utf8 $p $text; $p }
    $cases = @(
        @((List 'ok.md' "# C`n`n### K1 — a`n`nCites (``R-101``) and D-121.`n`n### K2 — b`n`nCites R-103.`n"), 0),
        @((List 'missing.md' "# C`n`n### K1 — a`n`nCites R-999.`n"), 1),
        @((List 'none.md' "# C`n`n### K1 — a`n`nCites nothing.`n`n### K2 — b`n`nCites R-101.`n"), 1),
        @((List 'empty.md' "# C`n`nNo rules here.`n"), 1),
        @((List 'prose.md' "# C`n`n### K1 — a`n`nCites R-101.`n`nK2 a rule written as prose, R-103.`n"), 1),
        @((List 'level.md' "# C`n`n### K1 — a`n`nCites R-101.`n`n## K2 — b`n`nCites nothing.`n"), 1),
        @((List 'twice.md' "# C`n`n### K1 — a`n`nCites nothing.`n`n### K1 — again`n`nCites R-101.`n"), 1),
        @((List 'later.md' "# C`n`n### K1 — a`n`nCites R-101.`n`n## Notes`n`nR-999 outside any rule.`n"), 0)
    )
    $failed = 0
    foreach ($c in $cases) {
        $got = [int]((Get-Problems $c[0] $dir).Count -ne 0)
        if ($got -ne $c[1]) { $failed++; Write-Host "FAIL $(Split-Path -Leaf $c[0]): expected $($c[1]), got $got" }
    }
    # The entry point itself: exit codes.
    foreach ($e in @(@('ok.md', 0), @('missing.md', 1))) {
        pwsh -NoProfile -File $PSCommandPath -Path (Join-Path $dir $e[0]) -Repo $dir *> $null
        if ($LASTEXITCODE -ne $e[1]) { $failed++; Write-Host "FAIL entry point $($e[0]): exit $LASTEXITCODE, expected $($e[1])" }
    }
    Remove-Item -Recurse -Force $dir
    Write-Host "criteria-cite self-test: $($cases.Count + 2) cases, $failed failed"
    exit ([int]($failed -ne 0))
}

$checklist = [IO.Path]::GetFullPath($Path, $Repo)
$problems = Get-Problems $checklist $Repo
if ($problems | Where-Object { $_ -like 'cannot read*' }) { $problems | ForEach-Object { Write-Host "criteria-cite: $_" }; exit 2 }
if ($problems.Count) { $problems | ForEach-Object { Write-Host "criteria-cite: $_" }; exit 1 }
Write-Host 'criteria-cite: every rule cites only identifiers that exist'
exit 0
