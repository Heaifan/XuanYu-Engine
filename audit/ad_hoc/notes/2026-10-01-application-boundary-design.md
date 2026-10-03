# XYE-SRP-R1-A2 Application Boundary Design

- Application should be defined as a boundary and coordination role, not as a new `ApplicationService` class.
- Startup belongs to the composition/startup workflow: construct routes and lifecycle owners, load project/content, create World, derive RenderScene, then publish an application-ready state.
- Shutdown belongs to the lifecycle coordinator: stop input/timers, stop Scene3D, dispose native resources, clear session generations, and release application references. Domain data should not own UI or Vulkan shutdown.
- Command dispatch belongs to an application command route/dispatcher role: map semantic commands to camera/tool/transform/session operations; it must not contain Domain invariants, UI control mutation, or GPU resource code.
- Transaction commit belongs to a domain/application transaction boundary. Validate first, update derived/render state only when the commit path is valid, then write authoritative World state and dirty/revision state at one explicit commit point.
- State coordination belongs to the composition root/application state coordinator: expose immutable/read-only projections, own generation/revision transitions, and reconcile World, RenderScene, Selection, Input, and Session state without duplicating domain ownership.
- Current `EditorShellContext` is the evidence-based composition root but is overloaded; future slimming should reduce its stored state through ownership/projection boundaries, not by introducing a god-service.
