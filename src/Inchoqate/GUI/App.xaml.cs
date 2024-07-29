using System.Windows;

namespace Inchoqate.GUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public ResourceDictionary ThemeDictionary
    {
        // TODO: Could probably get this via its name with some query logic.
        get { return Resources.MergedDictionaries[0]; }
    }

    public void ChangeTheme(Uri uri)
    {
        ThemeDictionary.MergedDictionaries.Clear();
        ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
    }
}