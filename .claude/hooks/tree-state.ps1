<#
.SYNOPSIS
    Prints one hash of the working tree, so that two calls tell whether
    anything changed between them.

.DESCRIPTION
    Point 2 of the process plan. The hash covers git status --porcelain=v1
    -uall, git diff HEAD --binary, and the content of every untracked file not
    ignored, which the first two name without reading. Ignored files, bin/ and
    obj/ among them, are outside it.

    -Path limits all three to the given pathspecs; mutate.ps1 passes
    core/Runtime and harness. -Repo is the repository, the current directory
    by default.
#>
param([string]$Repo = (Get-Location).Path, [string[]]$Path = @(), [switch]$SelfTest)

function Get-TreeState([string]$Repo, [string[]]$Path = @()) {
    $tmp = New-TemporaryFile
    try {
        $status = @(git -C $Repo status --porcelain=v1 -uall -- @Path)
        # --binary: without it a changed binary file reads "Binary files differ" whatever its content.
        git -C $Repo diff HEAD --binary --no-ext-diff --no-textconv "--output=$tmp" -- @Path
        $diff = (Get-FileHash -LiteralPath $tmp -Algorithm SHA256).Hash
        $untracked = @(git -C $Repo ls-files --others --exclude-standard -- @Path | Sort-Object -CaseSensitive)
        $lines = @($status) + "diff $diff" + @($untracked | ForEach-Object {
            "untracked $_ $((Get-FileHash -LiteralPath (Join-Path $Repo $_) -Algorithm SHA256).Hash)"
        })
        $bytes = [Text.Encoding]::UTF8.GetBytes($lines -join "`n")
        return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)).ToLowerInvariant()
    } finally { Remove-Item -LiteralPath $tmp -Force }
}

if ($SelfTest) {
    $repo = Join-Path ([IO.Path]::GetTempPath()) "tree-state-$PID"
    $null = New-Item -ItemType Directory -Force $repo
    git -C $repo init -q
    git -C $repo config user.email 'self-test@example.invalid'
    git -C $repo config user.name 'self-test'
    Set-Content (Join-Path $repo 'a.txt') 'one'
    [IO.File]::WriteAllBytes((Join-Path $repo 'b.bin'), [byte[]](0, 1, 2, 3))
    Set-Content (Join-Path $repo '.gitignore') 'ignored.txt'
    git -C $repo add a.txt b.bin .gitignore
    git -C $repo commit -q -m 'base'

    $failed = 0
    $steps = 0
    $prev = Get-TreeState $repo
    function Expect-Change([string]$what) {
        $script:steps++
        $now = Get-TreeState $repo
        if ($now -eq $script:prev) { $script:failed++; Write-Host "FAIL hash unchanged after: $what" }
        $script:prev = $now
    }
    function Expect-Same([string]$what) {
        $script:steps++
        $now = Get-TreeState $repo
        if ($now -ne $script:prev) { $script:failed++; Write-Host "FAIL hash changed after: $what" }
    }

    Expect-Same 'nothing, a second call on an unchanged tree'
    Set-Content (Join-Path $repo 'a.txt') 'two'
    Expect-Change 'a tracked file edited'
    git -C $repo add a.txt
    Expect-Change 'the edited file staged'
    Set-Content (Join-Path $repo 'u.txt') 'one'
    Expect-Change 'an untracked file added'
    Set-Content (Join-Path $repo 'u.txt') 'two'
    Expect-Change 'the untracked file edited'
    [IO.File]::WriteAllBytes((Join-Path $repo 'b.bin'), [byte[]](0, 1, 2, 4))
    Expect-Change 'a tracked binary file edited'
    [IO.File]::WriteAllBytes((Join-Path $repo 'b.bin'), [byte[]](0, 1, 2, 5))
    Expect-Change 'the binary file edited again'
    Set-Content (Join-Path $repo 'ignored.txt') 'x'
    Expect-Same 'an ignored file added'
    # -Path: a change outside the pathspec leaves the hash of the pathspec alone.
    $null = New-Item -ItemType Directory -Force (Join-Path $repo 'in')
    Set-Content (Join-Path $repo 'in/c.txt') 'one'
    $steps++
    $scoped = Get-TreeState $repo @('in')
    Set-Content (Join-Path $repo 'u.txt') 'three'
    if ((Get-TreeState $repo @('in')) -ne $scoped) { $failed++; Write-Host 'FAIL -Path hash changed by a file outside it' }
    $steps++
    Set-Content (Join-Path $repo 'in/c.txt') 'two'
    if ((Get-TreeState $repo @('in')) -eq $scoped) { $failed++; Write-Host 'FAIL -Path hash unchanged by a file inside it' }
    # The entry point itself: one line, the same hash.
    $steps++
    $out = @(pwsh -NoProfile -File $PSCommandPath -Repo $repo)
    if ($out.Count -ne 1 -or $out[0] -ne (Get-TreeState $repo)) { $failed++; Write-Host "FAIL entry point printed: $out" }

    Remove-Item -Recurse -Force $repo
    Write-Host "tree-state self-test: $steps cases, $failed failed"
    exit ([int]($failed -ne 0))
}

Get-TreeState $Repo $Path
