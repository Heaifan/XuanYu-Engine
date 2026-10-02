# XYE SHELLCONTEXT Ownership Map

- Scope: read-only member classification for the live `EditorShellContext` in XuanYuEngine at the audited checkout.
- Evidence: the context declares 65 public members and one `ApplyRouteSet` method; `EditorShellComposition` performs broad dependency wiring, `EditorShellEventWiring` consumes UI references, and `EditorShellCompositionRuntime` directly reads/writes render/session/application state.
- Ownership rule: a reference registry is not the owner of the referenced state. UI controls belong to composition/UI wiring; `WorldState` and project data belong to domain/project state; render counters and scene stores belong to render/session state; pointer/placement state belongs to input/tool state; lifecycle objects belong to lifecycle orchestration; route references belong to composition/route construction.
- Highest migration risk: UI references, the 27 primary route references, the 16 secondary route references, `WorldState`, `RenderSeq`, `SessionActive`, and the three transform appliers, because many wiring/runtime files depend on them through `ctx`.
- Medium risk: project data, render mode, tool/input state objects, selection presenters, and lifecycle/render-store references.
- Lower risk: pure presenter references and route registry members after ownership is explicitly established; no code changes were made during the audit.
