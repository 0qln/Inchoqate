using Inchoqate.Miscellaneous;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Inchoqate.GUI
{
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


        public App()
        {
            BuildFiles.Initiate(clearOldData: true);
        }


        public void ChangeTheme(Uri uri)
        {
            ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }


        private void BorderlessWindowContainer_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
