<!--
  Merge this with a merge commit or a rebase. Do not squash.

  Every commit closing a plan point carries a `Plan-point: <n>` trailer, and
  /plan-status resolves closed points to their commits by looking it up.
  Squashing collapses those trailers into one message and breaks that lookup
  for the whole history, retroactively. It is the key the record is indexed by,
  not a preference about tidy history.
-->

## Do not squash

This branch's commits carry `Plan-point:` trailers that `/plan-status` resolves.
A squash merge destroys them and breaks the lookup across the whole repository,
not only here. Merge commit or rebase.

## What changed

<!-- One or two lines. The plan and the register hold the detail. -->

## Plan

<!-- The plan this branch carried, now in .claude/plans/, and its exit condition. -->

## What was not verified

<!-- Checks that were satisfied by reading rather than running, points whose
     reviewers did not run, clauses left pending. Say it here rather than
     letting a reviewer discover it. -->
