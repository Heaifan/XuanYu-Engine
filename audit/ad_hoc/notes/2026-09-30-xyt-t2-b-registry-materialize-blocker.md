# XYT-T2 B Registry materialization blocker

- In E:\\MyDoc\\project-VSCode\\XuanYuEngine, the requested 2906 denominator matches HEAD^ 45fd7e63, not current HEAD a41c96fc.
- Source scan: HEAD^ = 362 Core + 1867 World + 17 WarCore + 660 XYUI = 2906; current HEAD = 362 + 1872 + 17 + 660 = 2911. The current commit adds five committed World definitions through terrain visibility/cursor anchored zoom work.
- Existing registry remains an 8-record T-A file-level subset. Its Schema, selftest, and MergeGate pass only for that subset; they do not prove full-definition closure.
- Correct action is to stop materialization rather than fabricate 2906 entries or exclude five committed definitions. A future run needs an approved baseline change or five-definition adoption, then materialize against the actual chosen ref.
