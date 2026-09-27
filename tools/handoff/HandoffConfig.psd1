@{
    ActiveBranch = 'feat/v0.3-world-authoring-r1'
    Remote = 'origin'

    CanonicalWorkspaces = @(
        'D:\MyDoc\project-vsCode\XuanyuEngine'
        'E:\MyDoc\project-VSCode\XuanYuEngine'
    )

    PreferredDotnetByDrive = @{
        D = 'D:\MyApp\sdk-dotnet\dotnet.exe'
        E = 'E:\MyApp\sdk-dotnet\dotnet.exe'
    }

    ResolverScript = 'scripts\resolve-dotnet.ps1'
    BootstrapScript = 'scripts\xye-bootstrap.ps1'

    RemoteWinsWhenBehind = $true
    LegacyWorktreesBlockHandoff = $false
}
