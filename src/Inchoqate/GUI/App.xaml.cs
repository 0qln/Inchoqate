using GUI;
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
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
        }


        public void ChangeTheme(Uri uri)
        {
            ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }


        private void BorderlessWindowContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var container = (Border)sender;
            var window = Window.GetWindow(container);
            
            if (window is null)
            {
                container.Loaded += (_, _) =>
                {
                    window = Window.GetWindow(container);
                    new BorderlessWindow(window).FixSizingGlitch();
                };
            }
            else
            {
                new BorderlessWindow(window).FixSizingGlitch();
            }
        }
    }
}
