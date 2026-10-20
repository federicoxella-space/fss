<#
.SYNOPSIS
    Applies a fault as a patch, runs a check against it, reverses the patch,
    and proves the tree came back.

.DESCRIPTION
    Item 3 of "Enforcement" in .claude/design/2026-09-30-agent-workflow.md.
    The tester writes core/Tests/ only; this is how its faults reach the code.
    The state of the tree is recorded with tree-state.ps1 before the patch and
    compared after it is reversed.

    -Patch is a unified diff that touches only paths of the point: those the
    point's diff changes against HEAD and its untracked files (point 5 of the
    workflow review plan), on both sides of every entry, a rename's or copy's
    source included. Never core/Tests/, where breaking a test would turn the
    check red without the code being at fault; nor docs/, .claude/, AGENTS.md,
    a CLAUDE.md, core/BannedSymbols.txt or the record files, which the point's
    faults are not about. The tree is hashed over everything but those paths.
    -Check is a command, run by pwsh in the repository root: a fault is caught
    when it exits non-zero.

    Exit codes: 0 the check went red and the tree is restored; 1 the check
    stayed green, the fault was not caught; 2 refused before anything was
    applied; 3 the tree did not return to where it was, or tree-state could
    not record it. 3 wins over the others.
#>
param([string]$Patch, [string]$Check, [string]$Repo = (Get-Location).Path, [switch]$SelfTest)

$Excluded = @(
    'core/Tests/', 'docs/', '.claude/', 'AGENTS.md', 'core/BannedSymbols.txt',
    'PLAN.md', 'DECISIONS-OUTSIDE-SPEC.md', 'SPEC-QUESTIONS.md', 'REVIEW-REQUEST.md',
    'PROMOTIONS.md', 'RULINGS.md', 'DECIDER-BRIEFS.md'
)
# The same paths as pathspecs, for tree-state: everything but them is hashed.
$Hashed = @('.') + @($Excluded | ForEach-Object { ":!$($_.TrimEnd('/'))" }) + ':!**/CLAUDE.md' + ':!CLAUDE.md'

function Test-Excluded([string]$P) {
    if ($P -match '(^|/)CLAUDE\.md$') { return $true }
    foreach ($e in $Excluded) {
        if ($e.EndsWith('/')) { if ($P.StartsWith($e, [StringComparison]::OrdinalIgnoreCase)) { return $true } }
        elseif ($P -ieq $e) { return $true }
    }
    return $false
}

# The point's paths: changed against HEAD, staged or not, and untracked.
function Get-Admitted([string]$Repo) {
    $changed = @(git -C $Repo -c core.quotePath=false diff --name-only HEAD)
    $untracked = @(git -C $Repo -c core.quotePath=false ls-files --others --exclude-standard)
    return @($changed + $untracked | Where-Object { $_ -and -not (Test-Excluded $_) } | Sort-Object -Unique)
}

function Invoke-Mutation([string]$Repo, [string]$Patch, [string]$Check) {
    if (-not $Patch -or -not $Check) { Write-Host 'mutate: -Patch and -Check are both required'; return 2 }
    $Patch = [IO.Path]::GetFullPath($Patch, (Get-Location).Path)
    if (-not (Test-Path -LiteralPath $Patch -PathType Leaf)) { Write-Host "mutate: no patch at $Patch"; return 2 }
    $paths = @(git -C $Repo apply --numstat $Patch | ForEach-Object { ($_ -split "`t", 3)[2] })
    if ($LASTEXITCODE -ne 0 -or $paths.Count -eq 0) { Write-Host 'mutate: the patch cannot be read'; return 2 }
    # --numstat names a rename's or copy's destination only; its source is read
    # from the patch's own header, a line no hunk can begin with (R-073).
    $paths += @(Select-String -LiteralPath $Patch -Pattern '^(rename|copy) from (.+)$' | ForEach-Object { $_.Matches[0].Groups[2].Value })
    $excluded = @($paths | Where-Object { Test-Excluded $_ })
    if ($excluded) { Write-Host "mutate: the patch touches paths no fault may touch: $($excluded -join ', ')"; return 2 }
    $admitted = Get-Admitted $Repo
    $outside = @($paths | Where-Object { $admitted -notcontains $_ })
    if ($outside) { Write-Host "mutate: the patch touches paths outside the point's diff: $($outside -join ', ')"; return 2 }
    git -C $Repo apply --check $Patch
    if ($LASTEXITCODE -ne 0) { Write-Host 'mutate: the patch does not apply to the working tree'; return 2 }

    # A tree-state that cannot hash prints nothing, and two empty answers would
    # read as "returned": a failed call is a tree not known to be restored (R-071).
    $before = & "$PSScriptRoot/tree-state.ps1" -Repo $Repo -Path $Hashed
    if ($LASTEXITCODE -ne 0 -or -not $before) { Write-Host 'mutate: tree-state failed before the patch, nothing applied'; return 3 }
    git -C $Repo apply $Patch
    try {
        Push-Location $Repo
        try { pwsh -NoProfile -Command $Check | Out-Host; $checkExit = $LASTEXITCODE } finally { Pop-Location }
    } finally {
        git -C $Repo apply -R $Patch
    }
    $after = & "$PSScriptRoot/tree-state.ps1" -Repo $Repo -Path $Hashed
    if ($LASTEXITCODE -ne 0 -or -not $after) { Write-Host 'mutate: tree-state failed after the check, the restore is not proved'; return 3 }
    if ($after -ne $before) { Write-Host 'mutate: the tree did not return to where it was'; return 3 }
    if ($checkExit -eq 0) { Write-Host 'mutate: the check stayed green, the fault was not caught'; return 1 }
    Write-Host "mutate: the check went red (exit $checkExit), the tree is restored"
    return 0
}

if ($SelfTest) {
    $repo = Join-Path ([IO.Path]::GetTempPath()) "mutate-$PID"
    $null = New-Item -ItemType Directory -Force (Join-Path $repo 'core/Runtime'), (Join-Path $repo 'core/Tests'), (Join-Path $repo 'docs')
    git -C $repo init -q
    git -C $repo config user.email 'self-test@example.invalid'
    git -C $repo config user.name 'self-test'
    $file = Join-Path $repo 'core/Runtime/a.txt'
    Set-Content $file 'right'
    Set-Content (Join-Path $repo 'core/Tests/t.txt') 'test'
    Set-Content (Join-Path $repo 'docs/a.txt') 'doc'
    Set-Content (Join-Path $repo 'docs/b.txt') 'doc'
    Set-Content (Join-Path $repo 'core/Runtime/c.txt') 'untouched'
    git -C $repo add .
    git -C $repo commit -q -m 'base'
    # The coder's code is uncommitted when the tester runs: the fault is a patch against it, not against HEAD.
    Set-Content $file 'right', 'more'
    git -C $repo add core/Runtime/a.txt
    Set-Content $file 'wrong', 'more'
    git -C $repo diff "--output=$repo/fault.patch" -- core/Runtime/a.txt
    Set-Content $file 'right', 'more'
    Set-Content (Join-Path $repo 'core/Tests/t.txt') 'test2'
    git -C $repo diff "--output=$repo/outside.patch" -- core/Tests/t.txt
    git -C $repo checkout -q -- core/Tests/t.txt
    # A rename into core/Runtime: --numstat names only the destination.
    git -C $repo mv docs/a.txt core/Runtime/moved.txt
    git -C $repo diff --cached -M "--output=$repo/rename.patch" -- docs/a.txt core/Runtime/moved.txt
    git -C $repo mv core/Runtime/moved.txt docs/a.txt
    # A copy out of docs/: its source is outside too, though it writes nothing there.
    Copy-Item (Join-Path $repo 'docs/a.txt') (Join-Path $repo 'core/Runtime/copied.txt')
    git -C $repo add core/Runtime/copied.txt
    git -C $repo diff --cached -C --find-copies-harder "--output=$repo/copy.patch" -- docs/a.txt core/Runtime/copied.txt
    git -C $repo rm -q -f core/Runtime/copied.txt
    Set-Content (Join-Path $repo 'stale.patch') "--- a/core/Runtime/a.txt`n+++ b/core/Runtime/a.txt`n@@ -1 +1 @@`n-absent`n+wrong`n"
    # The point's diff reaches past core/Runtime: an untracked file of bench/ is
    # the point's, and a fault may be put in it (point 5 of the workflow review plan).
    $null = New-Item -ItemType Directory -Force (Join-Path $repo 'bench')
    Set-Content (Join-Path $repo 'bench/x.txt') 'right'
    Set-Content (Join-Path $repo 'bench.patch') "--- a/bench/x.txt`n+++ b/bench/x.txt`n@@ -1 +1 @@`n-right`n+wrong`n"
    $benchCatches = "if (@(Get-Content bench/x.txt)[0] -eq 'wrong') { exit 1 }"
    # A committed file the point does not change is outside its diff.
    Set-Content (Join-Path $repo 'untouched.patch') "--- a/core/Runtime/c.txt`n+++ b/core/Runtime/c.txt`n@@ -1 +1 @@`n-untouched`n+wrong`n"
    # docs/ and core/Tests/ stay out even when the point changes them.
    Set-Content (Join-Path $repo 'docs/b.txt') 'doc', 'changed'
    Set-Content (Join-Path $repo 'docsdiff.patch') "--- a/docs/b.txt`n+++ b/docs/b.txt`n@@ -1,2 +1,2 @@`n doc`n-changed`n+wrong`n"
    Set-Content (Join-Path $repo 'core/Tests/t.txt') 'test', 'changed'
    Set-Content (Join-Path $repo 'testsdiff.patch') "--- a/core/Tests/t.txt`n+++ b/core/Tests/t.txt`n@@ -1,2 +1,2 @@`n test`n-changed`n+wrong`n"
    # Every other path no fault may touch, each new in the point's diff.
    $excludedFiles = @('AGENTS.md', 'core/CLAUDE.md', 'CLAUDE.md', 'core/BannedSymbols.txt', '.claude/x.txt',
        'PLAN.md', 'DECISIONS-OUTSIDE-SPEC.md', 'SPEC-QUESTIONS.md', 'REVIEW-REQUEST.md', 'PROMOTIONS.md', 'RULINGS.md', 'DECIDER-BRIEFS.md')
    $excludedCases = @()
    $k = 0
    foreach ($x in $excludedFiles) {
        $null = New-Item -ItemType Directory -Force (Split-Path -Parent (Join-Path $repo $x))
        Set-Content (Join-Path $repo $x) 'right'
        $name = "excluded-$k.patch"; $k++
        Set-Content (Join-Path $repo $name) "--- a/$x`n+++ b/$x`n@@ -1 +1 @@`n-right`n+wrong`n"
        $excludedCases += , @($name, 'exit 1', 2, "a patch on $x, though it is in the diff")
    }
    # A check that leaves a file outside core/Runtime: the hash covers the tree.
    $benchResidue = "Set-Content bench/residue.txt 'x'; exit 1"
    # A rename whose source is a committed file the point does not change.
    git -C $repo mv core/Runtime/c.txt core/Runtime/d.txt
    git -C $repo diff --cached -M "--output=$repo/rename-untouched.patch" -- core/Runtime/c.txt core/Runtime/d.txt
    git -C $repo mv core/Runtime/d.txt core/Runtime/c.txt
    $catches = "if ((Get-Content core/Runtime/a.txt)[0] -eq 'wrong') { exit 1 }"
    $misses = 'exit 0'
    $edits = "Set-Content core/Runtime/a.txt 'rewritten'; exit 1"
    $ignores = "Set-Content core/Runtime/b.txt 'new'; exit 1"
    $cases = @(
        @('fault.patch', $catches, 0, 'a fault the check catches'),
        @('fault.patch', $misses, 1, 'a fault the check misses'),
        @('outside.patch', $catches, 2, 'a patch on core/Tests/'),
        @('bench.patch', $benchCatches, 0, 'a fault in an untracked bench/ file of the point'),
        @('untouched.patch', $catches, 2, 'a patch on a file outside the point''s diff'),
        @('docsdiff.patch', $catches, 2, 'a patch on docs/, though docs/ is in the diff'),
        @('testsdiff.patch', $catches, 2, 'a patch on core/Tests/, though it is in the diff'),
        @('rename-untouched.patch', $catches, 2, 'a rename from a file outside the point''s diff'),
        @('rename.patch', $catches, 2, 'a rename from docs/ into core/Runtime'),
        @('copy.patch', $catches, 2, 'a copy from docs/ into core/Runtime'),
        @('stale.patch', $catches, 2, 'a patch that does not apply'),
        @('missing.patch', $catches, 2, 'no patch file'),
        @('fault.patch', $edits, 3, 'a check that rewrites the patched file'),
        @('fault.patch', $ignores, 3, 'a check that leaves a new file in core/Runtime'),
        @('bench.patch', $benchResidue, 3, 'a check that leaves a new file in bench/')
    ) + $excludedCases
    $failed = 0
    foreach ($c in $cases) {
        $got = Invoke-Mutation $repo (Join-Path $repo $c[0]) $c[1] 6>$null | Select-Object -Last 1
        $content = (Get-Content $file) -join '|'
        if ($got -ne $c[2]) { $failed++; Write-Host "FAIL $($c[3]): exit $got, expected $($c[2])" }
        if ($c[2] -ne 3 -and $content -ne 'right|more') { $failed++; Write-Host "FAIL $($c[3]): the file reads '$content' after mutate" }
        # The exit-3 cases disturb the tree on purpose; put it back before the next case.
        Set-Content $file 'right', 'more'
        Remove-Item -Force -ErrorAction Ignore (Join-Path $repo 'core/Runtime/b.txt'), (Join-Path $repo 'bench/residue.txt')
        Set-Content (Join-Path $repo 'bench/x.txt') 'right'
    }
    if (@(Get-Content (Join-Path $repo 'bench/x.txt'))[0] -ne 'right') { $failed++; Write-Host 'FAIL the bench/ fault was not undone' }
    # tree-state cannot hash a file it must read: exit 3, the patch never applied.
    $lock = [IO.File]::Open((Join-Path $repo 'core/Runtime/locked.txt'), 'Create', 'ReadWrite', 'None')
    try { $got = Invoke-Mutation $repo (Join-Path $repo 'fault.patch') $catches 6>$null 2>$null | Select-Object -Last 1 } finally { $lock.Dispose() }
    Remove-Item -Force (Join-Path $repo 'core/Runtime/locked.txt')
    if ($got -ne 3) { $failed++; Write-Host "FAIL tree-state failing: exit $got, expected 3" }
    if (((Get-Content $file) -join '|') -ne 'right|more') { $failed++; Write-Host 'FAIL tree-state failing: the patch was applied' }
    # The entry point itself: arguments, exit code.
    $patch = Join-Path $repo 'fault.patch'
    pwsh -NoProfile -File $PSCommandPath -Repo $repo -Patch $patch -Check $catches *>$null
    if ($LASTEXITCODE -ne 0) { $failed++; Write-Host "FAIL entry point, caught fault: exit $LASTEXITCODE" }
    pwsh -NoProfile -File $PSCommandPath -Repo $repo -Patch $patch -Check $misses *>$null
    if ($LASTEXITCODE -ne 1) { $failed++; Write-Host "FAIL entry point, missed fault: exit $LASTEXITCODE" }

    Remove-Item -Recurse -Force $repo
    Write-Host "mutate self-test: $($cases.Count + 3) cases, $failed failed"
    exit ([int]($failed -ne 0))
}

exit (Invoke-Mutation $Repo $Patch $Check | Select-Object -Last 1)
