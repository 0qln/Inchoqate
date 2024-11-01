using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel;

public class AppViewModel : BaseViewModel
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<AppViewModel>();

    private ProjectViewModel? _project;

    public ProjectViewModel? Project
    {
        get => _project;
        set => SetProperty(ref _project, value);
    }

}