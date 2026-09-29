# Regression Witness Policy

## Purpose

Regression Witness proves the same test was genuinely RED before a fix and GREEN after it. A passing test added after the fix is useful evidence, but it is not a historical RED witness.

## Required record

Each formal fix may claim `REGRESSION WITNESS = COMPLETE` only when one record contains:

`BugId`, `BugSymptom`, `PreFixBaseline`, `WitnessTest`, `PreFixResult`, `PostFixBaseline`, `PostFixResult`, `RequiredEvidenceTier`, `HarnessStatus`, `RootCauseClassification`, and `RegressionScope`.

`PreFixResult` must be `FAIL`, `PostFixResult` must be `PASS`, and both results must refer to the same `WitnessTest`. `HarnessStatus` must be `PASS`; otherwise product evidence is invalid. `RootCauseClassification` must not be `UNKNOWN`.

## Evidence states

- `REGRESSION WITNESS`: same-test PRE-FIX `FAIL` and POST-FIX `PASS`.
- `Coverage Test`: useful coverage without a proven RED-to-GREEN pair.
- `Characterization Test`: records existing behavior without proving a defect boundary.
- `Static Guard`: detects a structural condition without executing the runtime symptom.
- `Retroactive Test`: test was added after the bug was fixed; it must record `RETROACTIVE` and `PRE-FIX NOT EXECUTED`.

Retroactive evidence may cite a historical commit, diff, incident, or runtime evidence. It must never be written as `PreFixResult = FAIL` unless that result was actually executed.

## Harness integrity

High-risk Avalonia Headless harnesses must independently prove: UI Dispatcher execution, Button Click dispatch, Pointer Event dispatch, correct PlatformServices installation, fixture lifecycle isolation, and no state pollution across consecutive tests. A failed harness selftest makes dependent product-test evidence `EVIDENCE INVALID`, not `PRODUCT FAIL`.

## Root cause

The allowed classifications are `PRODUCT`, `TEST_HARNESS`, `MIXED`, and `UNKNOWN`. Harness causes include wrong UI thread or Dispatcher, incorrect Avalonia initialization, fixture state leakage, incorrect mocks, timing, and platform mismatch. A harness finding cannot erase an already evidenced product cause; when both exist, use `MIXED`. `UNKNOWN` is blocked.

## Gate output

`scripts/governance/regression-witness-gate.ps1` emits: `REGRESSION WITNESS`, `PRE-FIX RESULT`, `POST-FIX RESULT`, `RETROACTIVE`, `HARNESS INTEGRITY`, `ROOT CAUSE CLASSIFICATION`, `REQUIRED EVIDENCE TIER`, and `GATE STATUS`.

## Non-goals

This policy does not permit reset, checkout, stash, worktree creation, test deletion, weakened assertions, or rewriting shared workspace history to manufacture RED evidence.
