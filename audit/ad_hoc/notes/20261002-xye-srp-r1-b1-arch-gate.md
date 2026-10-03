# XYE-SRP-R1-B1 architecture gate audit

- Current audited worktree is clean and detached; no code-change evidence shows a newly added center node or test-ownership migration.
- Existing center nodes remain: `EditorShellContext` owns route references and mutable cross-domain state; `Scene3dFrameRoute` owns frame gating and Presented pending/success/failure promotion. These are existing architectural centers, not newly introduced by this audit.
- Ownership is mostly explicit at the local boundary: `ViewportRenderSceneStore` is the RenderScene writer, `Scene3dFrameRoute` owns frame state and presented pick/gizmo state, and Vulkan Session owns GPU/session resources plus presented camera/overlay state. The split ownership of Presented products must not be collapsed casually.
- Candidate Tree Purity cannot be certified from a clean worktree, source search, or test green status alone. It requires current candidate-tree provenance, ownership manifest, and exact changed-path evidence.
- 5+100 is an architecture/release structure gate and must remain separate from functional Truth or lane certification. A Lane PASS does not imply Integration PASS; integration still requires current ownership, dependency, candidate-tree, build/test, and applicable runtime evidence.
- Test ownership currently remains aggregated in `XuanYu.Engine.Tests`, which references Render, Vulkan, Editor.Windows, and other projects; architecture tests are centralized under `XuanYu.Engine.Tests/Architecture`. No current evidence shows that this ownership changed.
