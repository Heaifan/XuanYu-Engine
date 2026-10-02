# VK-SRP-A Draw Dispatch Exclusivity

- In XuanYuEngine Vulkan FrameEntry dispatch, a special handler followed by an ordinal fallback (`draw.Kind < EntityFill`) caused Terrain, MapGround, MapBounds, and MapVectorOverlay entries to be consumed by both their special handler and DrawAssist.
- The durable contract is one RenderDrawKind to one explicit handler. Use a dedicated switch over the enum, explicit unsupported failure, and explicit pipeline binding cases; never infer business routing from enum order or a generic fallback.
- A valid regression must test who consumes the entry, not merely whether RenderDrawPlan contains it. Source-contract coverage should make the old implementation RED and the explicit switch GREEN.
- Dispatch contract tests are T1/T2 evidence only. They do not establish Vulkan visual visibility (T3/T4), and stale source-literal tests must be reported rather than satisfied with compatibility comments or fake branches.
