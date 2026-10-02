# Dirty provenance classifier governance note

- Dirty provenance must distinguish current ownership, friendly active ownership, active dependency consumption, released ownership, conflict, and unknown; “not produced by the current task” is not sufficient evidence for UNKNOWN.
- ExpectedDependencies may identify either an owner TaskId or a scope root such as `XuanYu.Render.Vulkan`; scope-root declarations must match child paths by directory boundary.
- For SRP/Vulkan, XYT consuming `XuanYu.Render.Vulkan` and ACTIVE SRP owning `XuanYu.Render.Vulkan/**` is `FRIENDLY_ACTIVE_DEPENDENCY`, with CodingAllowed=YES and FinalEvidenceEligible=NO.
