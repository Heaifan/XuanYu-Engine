using System.ComponentModel;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class UiRoot
{
    UiVm? _hookedVm;
    LogPresenter? _hookedPresenter;

    void HookVm()
    {
        var vm = DataContext as UiVm;
        if (!ReferenceEquals(_hookedVm, vm))
        {
            if (_hookedVm is not null) _hookedVm.PropertyChanged -= OnVmPropertyChanged;
            _hookedVm = vm;
            if (vm is not null) vm.PropertyChanged += OnVmPropertyChanged;
        }
        var presenter = vm?.LogPresentation;
        if (!ReferenceEquals(_hookedPresenter, presenter))
        {
            if (_hookedPresenter is not null) _hookedPresenter.PropertyChanged -= OnLogPresenterChanged;
            _hookedPresenter = presenter;
            if (presenter is not null) presenter.PropertyChanged += OnLogPresenterChanged;
        }
        ClampLayout();
    }

    void OnLogPresenterChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LogPresenter.IsOpen)) ClampLayout();
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UiVm.InspectorIdentity)) return;
        RefreshDiagnosticRegistry();
        Dispatcher.UIThread.Post(RefreshDiagnosticRegistry, DispatcherPriority.Render);
    }
}
