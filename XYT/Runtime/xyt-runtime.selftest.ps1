$ErrorActionPreference='Stop'; $env:XYT_SKIP_GIT='1'; $root=Split-Path -Parent (Split-Path -Parent $PSScriptRoot); $pwsh=(Get-Command pwsh).Source
$module=Join-Path $root 'XYT\Runtime\xyt-runtime.ps1'; $tmp=Join-Path ([IO.Path]::GetTempPath()) ('xyt-runtime-'+[guid]::NewGuid().ToString('N')); New-Item -ItemType Directory $tmp|Out-Null
function Run([string]$name,[string]$code,[int]$expect,[switch]$fault){$out=Join-Path $tmp "$name.out"; $args=@('-NoProfile','-File',$module,'-Capability','P3-01','-AppCommand',$pwsh,'-AppScript',$code,'-TimeoutSeconds','2','-EvidenceRoot',$tmp); if($fault){$args+='-FakeHarnessFault'}; & $pwsh @args *> $out; if($LASTEXITCODE -ne $expect){throw "$name exit=$LASTEXITCODE expected=$expect"}; $r=Get-ChildItem $tmp -Filter 'P3-01-*.json'|Sort-Object LastWriteTime -Descending|Select-Object -First 1; if(!$r){throw "$name no evidence"}; Get-Content -Raw $r.FullName|ConvertFrom-Json}
try {
    $markers='【VulkanSurface】创建 Vulkan Surface 成功;逻辑设备 创建成功;Swapchain 创建成功;首帧 呈现 成功'
    $pass=Run 'pass' "Write-Output '$markers'" 0; if($pass.Result -ne 'PASS'){throw 'normal exit did not pass'}
    $timeout=Run 'timeout' 'Start-Sleep -Seconds 5' 1; if($timeout.Result -ne 'TIMEOUT'){throw 'timeout not captured'}
    $crash=Run 'crash' 'exit 17' 1; if($crash.Result -ne 'FAIL'){throw 'crash not captured'}
    $fault=Run 'harness-fault' "Write-Output '$markers'" 3 -fault; if(!$fault.HarnessFailure -or $fault.Result -ne 'UNCLASSIFIED'){throw 'harness fault became product failure'}
    'XYT RUNTIME HARNESS SELFTEST: PASS'; exit 0
} finally { Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue }
