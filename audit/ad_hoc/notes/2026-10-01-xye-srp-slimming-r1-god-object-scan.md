# XYE SRP Slimming R1 God Object Scan

- Scope: read-only scan of the current XuanYuEngine checkout, all `*.cs`, focusing on UiVm/ViewModel/Manager/Service/Controller/Snapshot/Context/Coordinator/Factory declarations.
- Evidence rule: use the live checkout only; do not infer current architecture from historical SHA, dirty counts, or earlier reports.
- Current scan baseline: 632 C# files. Production declarations matched 1 Service, 9 Context-related types, and 11 Snapshot-related types; no production declaration matched UiVm, ViewModel, Manager, Controller, Coordinator, or Factory.
- Highest SRP risk: `EditorShellContext` is a composition-root state bag with about 65 public members spanning UI controls, business objects, route registry, presenters, transform application, input/ground state, render state, and lifecycle/store references.
- Next risk: `VulkanRenderContext` combines Vulkan instance/device/surface setup, function loading, physical-device selection orchestration, legacy resource creation, frame entry, error handling, cleanup, and IDisposable lifecycle.
- Medium candidates: `EditorInputService` couples settings loading, snapshot construction/replacement, active-drag cancellation, revision state, singleton lifetime, and notification; `EditorInputBindingSnapshot` couples lookup/index data, mutable drag session state, and builder logic across partial files.
- Snapshot records and small builders are generally data/translation objects, not God Objects; rank by behavioral responsibility and lifecycle ownership, not field count alone.
- No code, filename, abstraction, commit, or branch state was changed by the audit.
