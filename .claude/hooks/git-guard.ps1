<#
.SYNOPSIS
    PreToolUse hook on the committer's Bash: admits a few git commands, and
    validates a commit before it is made.

.DESCRIPTION
    Item 2 of "Enforcement" in .claude/design/2026-09-30-agent-workflow.md.
    Reads the hook's JSON on stdin; exit 0 allows, exit 2 denies with the
    reason on stderr.

    Admitted: git status, diff, log, show; git add with explicit paths; git
    commit -F <file>. Nothing joined to another command, redirected or
    substituted. On commit: no staged path under docs/; the subject carries an
    identifier or says that none applies; Plan-point: at most once, as
    "Plan-point: <integer>" alone on its line.
#>
param([switch]$SelfTest)

$Identifier = '\b(NFR|FR|DEC|AC|SQ|A|R|D)-[A-Z0-9]'
$NoneApplies = '(?i)\bno (identifier|requirement|decision) applies\b'

function Split-Words([string]$Command) {
    [regex]::Matches($Command, '"[^"]*"|''[^'']*''|\S+') | ForEach-Object {
        $t = $_.Value
        if ($t.Length -ge 2 -and ($t[0] -eq '"' -or $t[0] -eq "'") -and $t[-1] -eq $t[0]) { $t.Substring(1, $t.Length - 2) } else { $t }
    }
}

# Returns $null if the message passes, or the reason it does not.
function Get-MessageDenial([string]$Message) {
    $subject = ($Message -split "`r?`n")[0]
    if ($subject -notmatch $Identifier -and $subject -notmatch $NoneApplies) {
        return 'git-guard: the subject carries no identifier and does not say that none applies'
    }
    $mentions = [regex]::Matches($Message, '(?i)plan-point').Count
    if ($mentions -gt 1) { return 'git-guard: Plan-point appears more than once' }
    if ($mentions -eq 1 -and $Message -notmatch '(?m)^Plan-point: \d+\r?$') {
        return 'git-guard: Plan-point must read "Plan-point: <integer>" alone on its line'
    }
    return $null
}

function Get-Denial([string]$Json) {
    try { $in = $Json | ConvertFrom-Json } catch { return 'git-guard: input is not JSON' }
    $cmd = [string]$in.tool_input.command
    $cwd = if ($in.cwd) { $in.cwd } else { (Get-Location).Path }

    if ($cmd -match '[;&|`<>\r\n]' -or $cmd.Contains('$(')) {
        return 'git-guard: one git command, not joined, piped, redirected or substituted'
    }
    $w = @(Split-Words $cmd)
    if ($w.Count -lt 2 -or $w[0] -ne 'git') { return 'git-guard: only git status, diff, log, show, add, commit' }
    $sub = $w[1]
    $rest = @($w | Select-Object -Skip 2)

    switch -CaseSensitive ($sub) {
        { $_ -in 'status', 'diff', 'log', 'show' } {
            if ($rest | Where-Object { $_ -like '--output*' }) { return 'git-guard: --output writes a file' }
            return $null
        }
        'add' {
            $paths = @($rest | Where-Object { $_ -ne '--' })
            if ($paths.Count -eq 0) { return 'git-guard: git add needs explicit paths' }
            foreach ($p in $paths) {
                if ($p.StartsWith('-')) { return "git-guard: git add takes no option ($p)" }
                if ($p -in '.', './', '*', ':/' -or $p -match '[*?\[]' -or $p.StartsWith(':')) {
                    return "git-guard: '$p' is not an explicit path"
                }
            }
            return $null
        }
        'commit' {
            if ($rest.Count -ne 2 -or $rest[0] -cne '-F' -or $rest[1] -eq '-') {
                return 'git-guard: the only commit admitted is git commit -F <file>'
            }
            $file = [IO.Path]::GetFullPath($rest[1], $cwd)
            if (-not (Test-Path -LiteralPath $file -PathType Leaf)) { return "git-guard: no message file at $file" }
            $staged = @(git -C $cwd diff --cached --name-only 2>$null)
            if ($LASTEXITCODE -ne 0) { return 'git-guard: cannot read the staged set' }
            $docs = @($staged | Where-Object { $_ -like 'docs/*' })
            if ($docs) { return "git-guard: staged under docs/: $($docs -join ', ')" }
            return Get-MessageDenial (Get-Content -LiteralPath $file -Raw -Encoding utf8)
        }
        default { return "git-guard: git $sub is not admitted" }
    }
}

if ($SelfTest) {
    $repo = Join-Path ([IO.Path]::GetTempPath()) "git-guard-$PID"
    $null = New-Item -ItemType Directory -Force $repo
    git -C $repo init -q
    $null = New-Item -ItemType Directory -Force (Join-Path $repo 'docs')
    Set-Content (Join-Path $repo 'docs/a.md') 'x'
    Set-Content (Join-Path $repo 'b.txt') 'x'
    function Msg($name, $text) { Set-Content -NoNewline -Encoding utf8 (Join-Path $repo $name) $text; $name }
    $ok      = Msg 'ok.txt' "FR-C-04: a subject`n`nBody.`n`nPlan-point: 3`n"
    $noPoint = Msg 'nopoint.txt' "R-045: a subject`n`nBody.`n"
    $words   = Msg 'words.txt' "Tidy the hooks folder, no identifier applies`n"
    $noId    = Msg 'noid.txt' "Tidy the hooks folder`n`nPlan-point: 3`n"
    $prose   = Msg 'prose.txt' "D-111: a subject`n`nPlan-point: none, it installs the plan`n"
    $twice   = Msg 'twice.txt' "D-111: a subject`n`nPlan-point: 3`nPlan-point: 4`n"
    $inBody  = Msg 'inbody.txt' "D-111: a subject`n`nThis closes no Plan-point.`n"
    function J($c) { @{ tool_name = 'Bash'; tool_input = @{ command = $c }; cwd = $repo } | ConvertTo-Json -Compress }

    $cases = @(
        @('git status', 'allow'),
        @('git status --porcelain', 'allow'),
        @('git diff --cached --name-only', 'allow'),
        @('git log -3 --format=%B', 'allow'),
        @('git show HEAD', 'allow'),
        @('git add core/Runtime/A.cs PLAN.md', 'allow'),
        @('git add -- "path with space/a.md"', 'allow'),
        @('git add -A', 'deny'),
        @('git add .', 'deny'),
        @('git add -u', 'deny'),
        @('git add --all', 'deny'),
        @('git add', 'deny'),
        @('git add core/*.cs', 'deny'),
        @('git add :/', 'deny'),
        @('git add a && rm b', 'deny'),
        @('git add a; rm b', 'deny'),
        @('git add a || rm b', 'deny'),
        @('git log | head', 'deny'),
        @('git log > out.txt', 'deny'),
        @('git add $(cat list)', 'deny'),
        @('git add `cat list`', 'deny'),
        @("git add a`nrm b", 'deny'),
        @('git diff --output=x.patch', 'deny'),
        @('git push', 'deny'),
        @('git push origin main', 'deny'),
        @('git commit --amend -F ok.txt', 'deny'),
        @('git commit -m "FR-C-04: x"', 'deny'),
        @('git commit -a -F ok.txt', 'deny'),
        @('git -C . status', 'deny'),
        @('git reset --hard', 'deny'),
        @('rm -rf docs', 'deny'),
        @("git commit -F $ok", 'allow'),
        @("git commit -F $noPoint", 'allow'),
        @("git commit -F $words", 'allow'),
        @("git commit -F $noId", 'deny'),
        @("git commit -F $prose", 'deny'),
        @("git commit -F $twice", 'deny'),
        @("git commit -F $inBody", 'deny'),
        @('git commit -F missing.txt', 'deny'),
        @('git commit -F -', 'deny')
    )
    $failed = 0
    git -C $repo add b.txt
    foreach ($c in $cases) {
        $got = if (Get-Denial (J $c[0])) { 'deny' } else { 'allow' }
        if ($got -ne $c[1]) { $failed++; Write-Host "FAIL expected $($c[1]), got ${got}: $($c[0])" }
    }
    # A staged path under docs/ stops a commit whose message passes.
    git -C $repo add docs/a.md
    if (-not (Get-Denial (J "git commit -F $ok"))) { $failed++; Write-Host 'FAIL commit with docs/a.md staged was allowed' }
    # The entry point itself: stdin, exit code.
    foreach ($e in @(@('git add -A', 2), @('git status', 0))) {
        (J $e[0]) | pwsh -NoProfile -File $PSCommandPath 2>$null
        if ($LASTEXITCODE -ne $e[1]) { $failed++; Write-Host "FAIL entry point '$($e[0])': exit $LASTEXITCODE, expected $($e[1])" }
    }
    Remove-Item -Recurse -Force $repo
    Write-Host "git-guard self-test: $($cases.Count + 3) cases, $failed failed"
    exit ([int]($failed -ne 0))
}

$reason = Get-Denial ([Console]::In.ReadToEnd())
if ($reason) { [Console]::Error.WriteLine($reason); exit 2 }
exit 0
