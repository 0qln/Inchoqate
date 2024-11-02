namespace Inchoqate.GUI.ViewModel;

public class AppViewModel : BaseViewModel
{
    private ProjectViewModel? _project;

    public ProjectViewModel? Project
    {
        get => _project;
        set => SetProperty(ref _project, value);
    }
}