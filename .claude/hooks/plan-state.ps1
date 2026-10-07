<#
.SYNOPSIS
    Reads a plan and prints its state as JSON, so that the three plan skills
    stop restating in prose how a plan is read.

.DESCRIPTION
    Point 3 of the process plan. -Path is the plan, PLAN.md by default; with
    -Draft it is a review request, REVIEW-REQUEST.md by default, and the plan is
    the fenced block under "## Draft plan" (R-048). -Repo is the repository, the
    current directory by default.

    Italian and English outcome markers are both read (R-045). The commits of a
    closed point are those carrying its Plan-point: trailer within the plan's
    range, as /plan-status step 4 defines it; they are resolved only for the
    live PLAN.md and for a plan archived under .claude/plans/.
#>
param([string]$Repo = (Get-Location).Path, [string]$Path, [switch]$Draft, [switch]$SelfTest)

$Outcome = '^\*(Esito|Outcome):\*'
$Discharged = '^\*(Riserva sciolta|Reserve discharged):\*'

function Get-PlanLines([string]$File, [bool]$Draft) {
    $lines = @(Get-Content -LiteralPath $File -Encoding utf8)
    if (-not $Draft) { return , $lines }
    $at = [array]::IndexOf($lines, '## Draft plan')
    if ($at -lt 0) { return $null }
    $open = $at + 1
    while ($open -lt $lines.Count -and $lines[$open] -notmatch '^```') { $open++ }
    $close = $open + 1
    while ($close -lt $lines.Count -and $lines[$close] -notmatch '^```') { $close++ }
    if ($close -ge $lines.Count) { return $null }
    return , $lines[($open + 1)..($close - 1)]
}

function Read-Point([string[]]$sec) {
    $outcomeAt = -1
    for ($i = 0; $i -lt $sec.Count; $i++) { if ($sec[$i] -match $Outcome) { $outcomeAt = $i; break } }
    $criteria = if ($outcomeAt -ge 0) { $sec[0..($outcomeAt - 1)] } else { $sec }

    $status = 'open'
    if ($outcomeAt -ge 0) {
        # The outcome paragraph, joined: the marker follows the date sentence.
        $para = @()
        for ($i = $outcomeAt; $i -lt $sec.Count -and $sec[$i].Trim() -ne ''; $i++) { $para += $sec[$i] }
        $text = ($para -join ' ') -replace $Outcome, '' -replace '^\s*[^.]*\.\s*', ''
        $status = if ($text -match '^\**(FALLITO|FAILED)\b') { 'failed' }
        elseif ($text -match '^\**(Chiuso con riserva|Closed with reserve)\b') {
            if (@($sec | Where-Object { $_ -match $Discharged }).Count) { 'reserve-discharged' } else { 'reserve-open' }
        } else { 'closed' }
    }

    # The marker is a bullet of its own; failing that, a clause of "Closed by".
    $check = $null
    $bullet = @($criteria | Where-Object { $_ -match '^- \*\*Check: (weak|shallow)\*\*' } | Select-Object -First 1)
    if ($bullet.Count) { $null = $bullet[0] -match '^- \*\*Check: (weak|shallow)\*\*'; $check = $Matches[1] } else {
        $closedBy = ''
        $in = $false
        foreach ($l in $criteria) {
            if ($l -match '^- \*\*') { $in = $l -match '^- \*\*Closed by:\*\*' }
            if ($in) { $closedBy += " $l" }
        }
        if ($closedBy -match 'Check: (weak|shallow)') { $check = $Matches[1] }
    }

    $core = $null
    $coreReason = $false
    $coreLine = @($criteria | Where-Object { $_ -match '^- \*\*Core:\*\*' } | Select-Object -First 1)
    if ($coreLine.Count -and $coreLine[0] -match '^- \*\*Core:\*\*\s*(yes|no)\b(.*)$') {
        $core = $Matches[1]
        $coreReason = ($Matches[2] -replace '^[\s.,;:—–-]+', '') -ne ''
    }

    return [ordered]@{
        status     = $status
        check      = $check
        core       = $core
        coreReason = $coreReason
        serves     = [bool]@($criteria | Where-Object { $_ -match '^- \*\*Serves:\*\*' }).Count
    }
}

function Get-Archives([string]$Repo) {
    @(git -C $Repo log --reverse --diff-filter=A --format=%h -- .claude/plans/ | ForEach-Object {
        [pscustomobject]@{ commit = $_; files = @(git -C $Repo show --diff-filter=A --name-only --format= $_ -- .claude/plans/) }
    })
}

# The range of /plan-status step 4: from the archive commit before the plan's,
# exclusive, to its own, inclusive; for the live plan, from the last to HEAD.
function Get-Range([string]$Repo, [string]$Rel) {
    $arch = Get-Archives $Repo
    if ($Rel -eq 'PLAN.md') { return $(if ($arch.Count) { "$($arch[-1].commit)..HEAD" } else { 'HEAD' }) }
    for ($i = 0; $i -lt $arch.Count; $i++) {
        if ($arch[$i].files -contains $Rel) { return $(if ($i) { "$($arch[$i - 1].commit)..$($arch[$i].commit)" } else { $arch[$i].commit }) }
    }
    return $null
}

function Get-DeciderCount([string]$Repo) {
    $rulings = Join-Path $Repo 'RULINGS.md'
    $register = Join-Path $Repo 'DECISIONS-OUTSIDE-SPEC.md'
    $w = 0
    if (Test-Path -LiteralPath $rulings) {
        $m = Select-String -LiteralPath $rulings -Pattern '^\*\*Register watermark:\*\* (none|D-(\d+))' | Select-Object -First 1
        if ($m -and $m.Matches[0].Groups[2].Success) { $w = [int]$m.Matches[0].Groups[2].Value }
    }
    $ids = @()
    if (Test-Path -LiteralPath $register) {
        $ids = @(Select-String -LiteralPath $register -Pattern '^### D-(\d+)' | ForEach-Object { [int]$_.Matches[0].Groups[1].Value } | Where-Object { $_ -gt $w })
    }
    return [ordered]@{
        count = $ids.Count
        first = if ($ids.Count) { 'D-{0:D3}' -f $ids[0] } else { $null }
        last  = if ($ids.Count) { 'D-{0:D3}' -f $ids[-1] } else { $null }
    }
}

function Get-PlanState([string]$Repo, [string]$Path, [bool]$Draft) {
    if (-not $Path) { $Path = Join-Path $Repo $(if ($Draft) { 'REVIEW-REQUEST.md' } else { 'PLAN.md' }) }
    $state = [ordered]@{ path = $Path; exists = $false; kind = $null; abandoned = $null; points = @(); state = 'none'; next = $null; stoppedAt = $null; reservesOpen = @(); range = $null }

    $state.warning = $null
    if (git -C $Repo ls-tree --name-only main -- PLAN.md 2>$null) { $state.warning = 'a live PLAN.md sits on main (R-048)' }
    elseif ((git -C $Repo branch --show-current 2>$null) -eq 'main' -and (Test-Path -LiteralPath (Join-Path $Repo 'PLAN.md'))) { $state.warning = 'a PLAN.md sits in the working tree of main (R-048)' }
    $state.decider = Get-DeciderCount $Repo

    if (-not (Test-Path -LiteralPath $Path)) { return $state }
    $lines = Get-PlanLines $Path $Draft
    if ($null -eq $lines) { return $state }
    $state.exists = $true

    $heads = @()
    for ($i = 0; $i -lt $lines.Count; $i++) { if ($lines[$i] -match '^#{1,2} ') { $heads += $i } }
    $firstPoint = $lines.Count
    foreach ($h in $heads) { if ($lines[$h] -match '^## \d+\. ') { $firstPoint = $h; break } }
    foreach ($l in $lines[0..($firstPoint - 1)]) {
        if ($l -match '^\*\*Kind:\*\*\s*(\w+)') { $state.kind = $Matches[1] }
        if ($l -match '^\*\*Abandoned:\*\*\s*(.*)$') { $state.abandoned = $Matches[1] }
    }

    if (-not $Draft) {
        $rel = [IO.Path]::GetRelativePath((Resolve-Path -LiteralPath $Repo), (Resolve-Path -LiteralPath $Path)) -replace '\\', '/'
        $state.range = Get-Range $Repo $rel
    }

    for ($k = 0; $k -lt $heads.Count; $k++) {
        if ($lines[$heads[$k]] -notmatch '^## (\d+)\. (.*)$') { continue }
        $p = [ordered]@{ number = [int]$Matches[1]; title = $Matches[2] }
        $end = if ($k + 1 -lt $heads.Count) { $heads[$k + 1] - 1 } else { $lines.Count - 1 }
        $sec = if ($end -gt $heads[$k]) { @($lines[($heads[$k] + 1)..$end]) } else { @() }
        $p += Read-Point $sec
        if ($p.status -ne 'open' -and $state.range) {
            # Anchored, and -E: unanchored, "Plan-point: 1" also matches 10 to 19.
            $p.commits = @(git -C $Repo log -E "--grep=^Plan-point: $($p.number)`$" --format=%h $state.range)
        }
        $state.points += $p
    }

    $open = @($state.points | Where-Object { $_.status -eq 'open' })
    $state.reservesOpen = @($state.points | Where-Object { $_.status -eq 'reserve-open' } | ForEach-Object { $_.number })
    # "After the failure" is by position in the file, not by number: a point added
    # under D-014 may sit before a lower-numbered one (R-074).
    $failedAt = -1
    for ($i = 0; $i -lt $state.points.Count; $i++) { if ($state.points[$i].status -eq 'failed') { $failedAt = $i; break } }
    if ($state.abandoned) { $state.state = 'abandoned' }
    elseif ($failedAt -ge 0 -and @($state.points | Select-Object -Skip ($failedAt + 1) | Where-Object { $_.status -eq 'open' }).Count) {
        $state.state = 'stopped'; $state.stoppedAt = $state.points[$failedAt].number
    }
    elseif ($open.Count) { $state.state = 'running'; $state.next = $open[0].number }
    else { $state.state = 'finished' }
    return $state
}

if ($SelfTest) {
    $root = (git -C $PSScriptRoot rev-parse --show-toplevel)
    $fx = Join-Path $PSScriptRoot 'fixtures/plan-state'
    $failed = 0
    $cases = 0
    function Expect([string]$what, [bool]$ok) {
        $script:cases++
        if (-not $ok) { $script:failed++; Write-Host "FAIL $what" }
    }
    function Of([string]$file, [bool]$draft = $false) { Get-PlanState $root (Join-Path $fx $file) $draft }
    function Status($s, [int]$n) { @($s.points | Where-Object { $_.number -eq $n })[0].status }

    $s = Of 'absent.md'
    Expect 'no plan: exists false, state none' (-not $s.exists -and $s.state -eq 'none')

    $s = Of 'abandoned.md'
    Expect 'abandoned: declaration read' ($s.abandoned -like '2026-10-01*')
    Expect 'abandoned: state abandoned, no next point' ($s.state -eq 'abandoned' -and $null -eq $s.next)

    $s = Of 'failed-then-open.md'
    Expect 'FAILED with open points after it: stopped at 2' ($s.state -eq 'stopped' -and $s.stoppedAt -eq 2 -and $null -eq $s.next)
    Expect 'FAILED point reported failed' ((Status $s 2) -eq 'failed')

    $s = Of 'out-of-order.md'
    Expect 'point 8 placed before 7 has FAILED: stopped at 8, no next' ($s.state -eq 'stopped' -and $s.stoppedAt -eq 8 -and $null -eq $s.next)

    $s = Of 'reserve-open.md'
    Expect 'reserve open: status reserve-open' ((Status $s 1) -eq 'reserve-open')
    Expect 'reserve open: listed, plan goes on to 2' ($s.reservesOpen -contains 1 -and $s.next -eq 2)

    $s = Of 'reserve-discharged.md'
    Expect 'reserve discharged: status reserve-discharged' ((Status $s 1) -eq 'reserve-discharged')
    Expect 'reserve discharged: none open' ($s.reservesOpen.Count -eq 0)

    $s = Of 'all-closed.md'
    Expect 'every point closed: finished' ($s.state -eq 'finished' -and $s.points.Count -eq 2)
    Expect 'check markers weak and shallow read' ($s.points[0].check -eq 'weak' -and $s.points[1].check -eq 'shallow')
    Expect 'Kind and Serves read' ($s.kind -eq 'simulator' -and $s.points[0].serves -and -not $s.points[1].serves)

    $s = Of 'mixed-markers.md'
    Expect 'Esito closes a point' ((Status $s 1) -eq 'closed')
    Expect 'Outcome closes a point' ((Status $s 2) -eq 'closed')
    Expect 'FAILED in English is failed' ((Status $s 3) -eq 'failed')
    Expect 'Closed with reserve, Reserve discharged' ((Status $s 4) -eq 'reserve-discharged')
    Expect 'Chiuso con riserva without discharge' ((Status $s 5) -eq 'reserve-open')
    Expect 'FALLITO in prose after the date sentence is not a failure' ((Status $s 6) -eq 'closed')
    Expect 'Check: shallow inside Closed by' ($s.points[1].check -eq 'shallow')

    $s = Of 'core-no-reason.md'
    Expect 'Core: no without a reason' ($s.points[0].core -eq 'no' -and -not $s.points[0].coreReason)
    Expect 'Core: no with a reason' ($s.points[1].core -eq 'no' -and $s.points[1].coreReason)
    Expect 'Core absent is null' ($null -eq $s.points[2].core)

    $s = Of 'request-with-draft.md' $true
    Expect 'draft: read from the fenced block' ($s.exists -and $s.kind -eq 'process' -and $s.points.Count -eq 2 -and $s.next -eq 1)
    Expect 'draft: the request outside the block is not a point' (-not @($s.points | Where-Object { $_.title -like 'Outside*' }).Count)
    Expect 'draft: no commits resolved' ($null -eq $s.range)

    # The five archived plans: 31 points closed, each resolved to exactly one commit.
    $closed = 0
    foreach ($f in Get-ChildItem (Join-Path $root '.claude/plans') -Filter *.md) {
        $s = Get-PlanState $root $f.FullName $false
        foreach ($p in $s.points) {
            if ($p.status -eq 'open') { Expect "archive $($f.Name) point $($p.number) closed" $false; continue }
            $closed++
            Expect "archive $($f.Name) point $($p.number): one commit, found $(@($p.commits).Count)" (@($p.commits).Count -eq 1)
        }
    }
    Expect "archives: 31 points closed, found $closed" ($closed -eq 31)

    # The entry point: JSON on stdout.
    $out = pwsh -NoProfile -File $PSCommandPath -Repo $root -Path (Join-Path $fx 'failed-then-open.md') | ConvertFrom-Json
    Expect 'entry point prints JSON' ($out.state -eq 'stopped')

    Write-Host "plan-state self-test: $cases cases, $failed failed"
    exit ([int]($failed -ne 0))
}

Get-PlanState $Repo $Path $Draft.IsPresent | ConvertTo-Json -Depth 5 -EscapeHandling EscapeNonAscii
