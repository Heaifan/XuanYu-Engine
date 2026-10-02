# Current Handoff

## R2 Architecture

```text
Handclap     = Event Ledger + ACK + Context Transfer = ZERO Authority
Handoff      = Legacy compatibility shell = ZERO Authority
Authority    = tools/governance/**
```

Authority ownership is explicit:

```text
Candidate    = tools/governance/candidate/**
Coordinator  = tools/governance/coordinator/**
Ownership    = tools/governance/ownership/**
Release      = tools/governance/release/**
Task         = tools/governance/task/**
Workspace    = tools/governance/workspace/**
```

## Current Contract

- Handclap records events, acknowledgements, and context transfer only.
- Handoff only forwards Handclap fact commands and rejects retired Authority commands.
- PREPARE, JOIN, ADVANCE, CLOSE, RELEASE, ownership grant, mutex control, and Candidate gate are Legacy/Historical semantics, not the current recommended flow.
- Authority selftests run through the owner under `tools/governance/**`; they do not call Legacy Handoff Authority.
- Automated selftests establish technical evidence only; they do not establish product or real-device acceptance.

## Evidence Boundary

This document intentionally contains no branch, HEAD, Ahead/Behind, Dirty count, active-wave state, or other snapshot facts. Such facts belong in timestamped run evidence, not a long-lived current-handoff contract.

## R2 Convergence Result

R2 convergence is determined by the current unified test run, runtime legacy-caller scan, `git diff --check`, and the applicable 5+100 rule. No Commit or Push is implied by this document.
