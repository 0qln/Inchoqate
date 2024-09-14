using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Inchoqate.GUI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application, INotifyPropertyChanged
{
    private StackEditorViewModel? _stackEditor;

    public App()
    {
        Startup += delegate { MainWindow!.Loaded += delegate { TestSerde(); }; };
    }

    public ResourceDictionary ThemeDictionary => Current.Resources.MergedDictionaries.First();

    public StackEditorViewModel? StackEditor
    {
        get => _stackEditor;
        set
        {
            _stackEditor = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void TestSerde()
    {
        var initial = EventSerdeModel.Deserialize<EventViewModelBase>(nameof(StackEditor));
        StackEditor = new(new("Stack Editor", initial));

        // StackEditor = new();
        //
        // StackEditor.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplGrayscaleViewModel()});
        // StackEditor.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplGrayscaleViewModel()});
        // StackEditor.EventTree.Undo();
        // StackEditor.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplNoGreenViewModel()});
        //
        // EventSerdeModel.Serialize(StackEditor.EventTree.Initial, nameof(StackEditor));
    }

    public void ChangeTheme(Uri uri)
    {
        ThemeDictionary.MergedDictionaries.Clear();
        ThemeDictionary.MergedDictionaries.Add(new() { Source = uri });
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}