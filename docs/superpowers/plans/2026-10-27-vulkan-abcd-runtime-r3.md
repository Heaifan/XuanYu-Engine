# Vulkan A-D Runtime R3 Plan

Base: `28190731bc8f07917e218babe25ec922b0bae7f6` / Tree `7e49a831dc067ce95bbf75b3ad93ad5c643d22d4`.

## Scope

- F1: make upload completion uncertainty a bounded, explicit terminal state; preserve GPU resource safety and aggregate cleanup failures.
- F2: propagate Acquire/Swapchain recovery failure through the PresentLoop terminal state without changing successful synchronization.
- V1: execute A MapMemory failure injection on a real offscreen Vulkan device; assess C real multi-label/descriptor lifecycle evidence independently.
- Keep the original 22 issue IDs; link F1/F2 regressions to R3-03/R1-02 unless evidence requires a child issue.
- No Owner decomposition, unrelated Render audit, merge, push, or changes to the main checkout.

## File Ownership

- F1: label upload/cache/quarantine lifecycle sources and `test_VulkanMapLabelUploadState.cs`, `test_VulkanMapLabelUploadShutdown.cs`, `test_VulkanLabelUploadDeviceLostShutdown.cs`.
- F2: PresentLoop lifecycle/acquire/run sources and a new `test_VulkanPresentLoopRecoveryTerminalState.cs`.
- V1 investigation: read-only fixture discovery and A/C evidence; no writes to F1/F2 paths.
- Parent: version/changelog, issue register, plan, integration, builds, final Tree and evidence record.

## Required State Contracts

- Upload: NotSubmitted, Complete, CompletionUnknown, DeviceLost. Only known-complete work can enter ordinary retirement. Unknown completion must terminate retries and preserve live handles until an explicit safe device teardown boundary.
- PresentLoop: Running, Stopped, Failed with a stable failure reason. Recovery failure must be observable to the owner; `Start` cannot report success for a dead worker.
- Successful QueueSubmit/Acquire/Present and Fence order remain unchanged.

## Verification

1. Add tests first and demonstrate expected failure.
2. Run scoped fault-boundary tests separately from real-device runs.
3. Run affected Vulkan project build/tests serially through repository scripts.
4. Verify A/C runtime evidence only if a real fixture and supported fault route exist; label unavailable injections NOT RUN with the concrete limitation.
5. Verify BuildIdentity, version/changelog, 5+100, diff scope, and clean Candidate after each parent-owned fix commit.
