using System.Windows;

namespace Inchoqate.GUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public ResourceDictionary ThemeDictionary => Current.Resources.MergedDictionaries.First();

    public void ChangeTheme(Uri uri)
    {
        ThemeDictionary.MergedDictionaries.Clear();
        ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
    }
}