using Avalonia.Threading;
using XuanYu.Editor.Assets;
using XuanYu.Editor.Composition;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.SceneDocument;
using XuanYu.Render.Abstractions;
using XuanYu.World;
using XuanYu.World.Scene;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public UiVm() : this(null) { }

    public UiVm(INativeHostSurfaceBridgeFactory? surfaceBridgeFactory,
        Func<bool>? isWriteThread = null, bool seedInitialScene = true,
        IEditorDialogService? dialogService = null)
        : this(surfaceBridgeFactory,
            EditorRuntimeComposition.CreateScene(new GridWorldPartitionStrategy(regionSize: 5), seedInitialScene),
            EditorRuntimeComposition.CreateMapSession(isWriteThread ?? (() => Dispatcher.UIThread.CheckAccess())),
            isWriteThread, dialogService)
    { }

    public UiVm(INativeHostSurfaceBridgeFactory? surfaceBridgeFactory, SceneStateOwner sceneState,
        MapEditSession mapSession, Func<bool>? isWriteThread = null,
        IEditorDialogService? dialogService = null)
    {
        ViewportProbe.RefreshLoadedAssemblies();
        ArgumentNullException.ThrowIfNull(sceneState); ArgumentNullException.ThrowIfNull(mapSession);
        _documentStatusPresenter = new DocumentStatusPresenter(() => IsSceneDirty);
        _terrainImportCoordinator.Changed += RaiseImportBindings;
        _documentStatusPresenter.PropertyChanged += (_, args) => OnPropertyChanged(args.PropertyName);
        _editorState = new EditorStateOwner(isWriteThread ?? (() => Dispatcher.UIThread.CheckAccess()));
        _contextState = new EditorContextOwner(); _authoringState = new EditorAuthoringOwner();
        _sceneState = sceneState; _worldMutation = sceneState;
        _selectionCoordinator = new SelectionCoordinator(_editorState, _sceneState, _projectTreePresenter);
        var sceneStorage = new SceneStorageService();
        _sceneDocumentCoordinator = new SceneDocumentCoordinator(
            new SceneDocumentSaveTransaction(sceneStorage),
            new SceneDocumentLoadTransaction(sceneStorage, new GlbImportService(), _partitionStrategy));
        _historyCoordinator = new HistoryCoordinator(
            _historyOwner, ApplyHistory, FenceAppliedXycHistory, message => FooterMessage = message);
        _sceneState.RenderSnapshotChanged += _ => RefreshWorldProjectionBindings();
        if (_sceneState.RenderSnapshot.HasEntity) _sceneState.EnsureEntityCount(10);
        SurfaceBridgeFactory = surfaceBridgeFactory;
        if (dialogService is not null) _dialogService = dialogService;
        RunCommand = new RelayCommand(name => Run(name?.ToString() ?? string.Empty));
        SelectToolCommand = new RelayCommand(TrySelectTool, CanSelectTool);
        SwitchWorkspaceCommand = new RelayCommand(SwitchWorkspace);
        SelectRegionAuthoringModeCommand = new RelayCommand(SelectRegionAuthoringModeCommandTarget);
        ToggleSnapCommand = new RelayCommand(_ => TryToggleSnap()); ToggleEditorModeCommand = new RelayCommand(_ => ToggleEditorMode());
        InteractionCommand = new RelayCommand(name => RunInteraction(name?.ToString() ?? string.Empty));
        ToggleLogCommand = new RelayCommand(_ => IsLogOpen = !IsLogOpen);
        SelectLogFilterCommand = new RelayCommand(name => SetLogFilter(name?.ToString() ?? "全部"));
        ClearLogsCommand = new RelayCommand(_ => ClearLogs()); MapSession = mapSession; MapSession.MarkBaseline();
        AttachMapSession(MapSession); MapSession.ClearSelection(); InitializeMapManifest(); InitLogs(); InitNotificationPresenter();
        ViewportInput = UiVmViewportInputComposition.Create(this);
    }
}
