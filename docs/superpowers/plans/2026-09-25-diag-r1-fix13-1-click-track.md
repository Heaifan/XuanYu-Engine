# DIAG-R1-FIX13.1 Click-to-Track

## Scope

- Split diagnostic hover preview from explicit click tracking.
- Observe Avalonia tunnel clicks without marking them handled.
- Observe Native Viewport left-down and keep production forwarding intact.
- Reuse existing PopupRoot registration for Context Toolbar popup clicks.
- Add focused regression coverage; do not change input routing, Vulkan rendering, or popup architecture.

## Steps

1. Add failing tests for click lock/switch, hover protection, native click observation, passthrough, and diagnostic self-exclusion.
2. Implement preview/track entry points and Avalonia tunnel click observation.
3. Add Native Viewport clicked phase and connect it to explicit tracking.
4. Verify focused tests, affected builds, architecture/5+100/diff gates, then run the real editor for IPO acceptance.
5. Commit, push, and verify the remote tip.
