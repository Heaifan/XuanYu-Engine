# Test Harness Integrity

## Rule

The harness is evidence-producing infrastructure. A product test result is not admissible until the harness that produced it is independently valid.

## Avalonia Headless minimum

The harness selftest must demonstrate all of the following with executable checks:

1. UI Dispatcher work can execute.
2. A Button Click can be dispatched.
3. A Pointer Event can be dispatched.
4. PlatformServices are installed for the test platform.
5. Fixtures are created and disposed per test.
6. Consecutive tests do not share mutated state.

The checks must use the real test framework and platform contracts. A helper that returns `true`, a synthetic source disconnected from production wiring, or a mock that bypasses the dispatcher does not establish integrity.

## Result interpretation

- `PASS`: every required check passed.
- `FAIL`: at least one required check failed; all dependent product evidence is `EVIDENCE INVALID`.
- `NOT_RUN`: no integrity proof exists; dependent product evidence is blocked.

`TEST_HARNESS` may be the sole root cause only when product behavior has not independently been proven faulty. If product and harness failures coexist, classify `MIXED`; do not let either category conceal the other.

## Required evidence record

Record the harness command, framework/platform, fixture isolation observation, each check result, and the exact test set made invalid by a failed harness. Keep this evidence separate from the product bug record and do not relabel an invalid result as `PRODUCT FAIL`.
