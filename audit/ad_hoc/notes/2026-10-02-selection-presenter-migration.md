# XYE-SRP-R1-B1-B Selection Presenter migration

- WorldEntitySelectionPresenter, ViewportSelectionPresenter, and ProjectContentSelectionPresenter were already pure projection objects; the remaining coupling was their storage in EditorShellContext and EventWiring access through `ctx.ContentSelectionPresenter`.
- Minimal migration: instantiate all three presenters in EditorShellComposition, pass World/Viewport presenters to existing routes, pass ProjectContent presenter explicitly to EditorShellEventWiring, and remove all three Presenter fields from EditorShellContext.
- Selection Owner, Picking, and Transform were not modified.
- Added SelectionPresenterBoundaryTests to enforce Context-free presenter composition and explicit wiring.
- Verification: Build passed with 0 errors and 7 pre-existing warnings; boundary test 1/1 passed; non-Vulkan Selection tests 31/31 passed; git diff --check passed. Full test suite reached 607 passes before an existing VulkanDeviceInfoTests native Silk.NET Vulkan access violation aborted the host.
