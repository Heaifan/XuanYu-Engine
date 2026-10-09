class XytDependencyError : System.Exception {
    [string]$ErrorCode
    [string]$CyclePath
    XytDependencyError([string]$Path) : base("CIRCULAR_TEST_DEPENDENCY: $Path") {
        $this.ErrorCode = 'CIRCULAR_TEST_DEPENDENCY'
        $this.CyclePath = $Path
    }
}

function New-XytDependencyError([string]$Path) { [XytDependencyError]::new($Path) }
