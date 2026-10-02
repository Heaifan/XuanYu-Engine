# Handoff ghost ACTIVE Task regression

- Governance-C contract: Task completion MUST release its ACTIVE ownership; normal lifecycle is ACTIVE -> release -> RELEASED -> close -> CLOSED.
- RELEASED remains visible only for dirty/provenance explanation; CLOSED must be removed from the live registry while retaining audit history.
- Interrupted ACTIVE tasks must not be TTL-released. Only an authorized Coordinator may reap them, recording CloseReason=ABANDONED/<reason>; unauthorized reap must fail.
- Before declaring UNKNOWN modification, exhaust Active/Released Task Registry and dependency ownership as explanations.
- Evidence: `tools/handoff/task-flight-plan-lifecycle.selftest.ps1` and `docs/governance/handoff-task-flight-plan-contract.md` in HANDOFF-TASK-FLIGHT-PLAN-C; implementation was absent in the audited detached baseline, so the selftest correctly failed closed rather than modeling or mocking the registry.
