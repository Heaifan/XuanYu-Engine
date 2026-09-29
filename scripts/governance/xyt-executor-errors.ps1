class XytDependencyError : System.Exception {
    [string]$ErrorCode
    [string]$CyclePath
    XytDependencyError([string]$Path) : base("CIRCULAR_HANDOFF_DEPENDENCY: $Path") {
        $this.ErrorCode = 'CIRCULAR_HANDOFF_DEPENDENCY'
        $this.CyclePath = $Path
    }
}

function New-XytDependencyError([string]$CyclePath) {
    [XytDependencyError]::new($CyclePath)
}
