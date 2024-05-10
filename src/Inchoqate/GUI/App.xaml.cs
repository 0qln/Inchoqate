using Inchoqate.Miscellaneous;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

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

        private void WindowContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Border border)
            {
                border.Child.Clip = new RectangleGeometry
                {
                    RadiusX = border.CornerRadius.TopLeft,
                    RadiusY = border.CornerRadius.TopLeft,
                    Rect = new Rect
                    {
                        Width = e.NewSize.Width - border.BorderThickness.Left - border.BorderThickness.Right,
                        Height = e.NewSize.Height - border.BorderThickness.Top - border.BorderThickness.Bottom
                    }
                };
            }
        }
    }
}
