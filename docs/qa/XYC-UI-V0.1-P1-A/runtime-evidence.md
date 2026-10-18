# Runtime evidence summary

Candidate identity: `BASE=6b1979899d7421ee63d75833ebc7ceb0648f1505;OVERLAY-SHA256=00706F8E4CD2A4DDB1E68D1E4A08253FF8E64BF07333EE42E48196C909414ECE`
Editor.App.exe SHA-256: `DE13C9CF9C40C876B9F62C49D864F0FD665E77FE73EA5CCC1EE8896E0538B74C`
XuanYu.Editor.UI.dll SHA-256: `7C63D8D41C91501BA2E032D0C0A6E5FACE75D84C7E32ECDD50BB83CB2429C165`

Each run used an isolated real Editor process. The manifest includes PID, HWND, requested and actual size, DPI, theme, actual TOP control list, statuses, request/response records, state deltas, capture metadata and session close result. No physical mouse or keyboard input was used.

| Profile | PID | HWND | Client px | DPI / scale | Theme pixel sample | TOP nodes / verified IDs | Actual state effect | Restore | Close |
|---|---:|---|---:|---|---|---:|---|---|---|
| Light-1360 | 24584 | 0x131054 | 1360x820 | 96 / 1 | 255,255,255 | 203 / 6 | VIEW.ALL position delta 1450; revision 3->4 | True | True |
| Dark-1360 | 15948 | 0x110734 | 1360x820 | 96 / 1 | 42,56,66 | 203 / 6 | VIEW.ALL position delta 1450; revision 3->4 | True | True |
| Light-1920 | 30616 | 0x30072C | 1920x820 | 96 / 1 | 255,255,255 | 203 / 6 | VIEW.ALL position delta 1450; revision 3->4 | True | True |
| Dark-1920 | 24404 | 0x161054 | 1920x820 | 96 / 1 | 42,56,66 | 203 / 6 | VIEW.ALL position delta 1450; revision 3->4 | True | True |

All runtime negative checks passed per run: missing control, disabled control, disallowed SAVE invoke, incorrect session and bounded timeout. Duplicate ID and disappearance-before-invoke are covered by targeted tests.

PNG sidecar/manifest SHA-256 verification: PASS (12 captures checked).

The four canonical files are `Light-1360.png`, `Dark-1360.png`, `Light-1920.png`, and `Dark-1920.png` under the corresponding profile directories. Each directory also retains pre/post-invoke PNGs and JSON sidecars. `runtime-evidence.json` is the source manifest.
