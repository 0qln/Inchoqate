using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
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
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MaxDepth = null,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        TypeNameHandling = TypeNameHandling.All,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        // ReferenceResolverProvider = ()  => new RefResolver()
    };

    public class RefResolver : IReferenceResolver
    {
        /// <inheritdoc />
        public object ResolveReference(object context, string reference)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetReference(object context, object value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsReferenced(object context, object value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void AddReference(object context, string reference, object value)
        {
            throw new NotImplementedException();
        }
    }


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

        StackEditor = new();
        
        StackEditor.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplGrayscaleViewModel()});
        StackEditor.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplGrayscaleViewModel()});
        StackEditor.EventTree.Undo();
        StackEditor.Edits?.Eventuate<LinearEditAddedEvent, ICollection<EditBaseLinear>>(new() { Item = new EditImplNoGreenViewModel()});
        
        File.WriteAllText(
            "./stack-editor-event-tree.json", 
            JsonConvert.SerializeObject(
                StackEditor.EventTree.InitialEvent, Formatting.None, SerializerSettings
            )
        );
        
        // void FlatEvents(List<EventViewModelBase> collection, EventViewModelBase e)
        // {
        //     collection.Add(e);
        //
        //     foreach (var next in e.Next)
        //     {
        //         FlatEvents(collection, next.Value);
        //     }
        // }

        var events = JsonConvert.DeserializeObject<EventViewModelBase>(
            File.ReadAllText("./stack-editor-event-tree.json"),
            SerializerSettings
        )!;
        
        StackEditor = new(new("Stack Editor", events));
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