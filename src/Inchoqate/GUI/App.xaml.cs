using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Inchoqate.GUI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public AppViewModel DataContext { get; } = new();

    public RenderEditorViewModel ActiveEditor => DataContext.Project.Editors[DataContext.Project.ActiveEditor];

    public App()
    {
        // project.PropertyChanged += (_, e) =>
        // {
        //     switch (e.PropertyName)
        //     {
        //         case nameof(project.StackEditor):
        //             if (PreviewImage.DataContext is PreviewImageViewModel pvm)
        //                 if (StackEditor.DataContext is StackEditorViewModel svm)
        //                     pvm.RenderEditor = svm;
        //
        //             _activeEditor = project.StackEditor;
        //             break;
        //     }
        // };
    }

    public ResourceDictionary ThemeDictionary => Current.Resources.MergedDictionaries.First();

    public void ChangeTheme(Uri uri)
    {
        ThemeDictionary.MergedDictionaries.Clear();
        ThemeDictionary.MergedDictionaries.Add(new() { Source = uri });
    }

}