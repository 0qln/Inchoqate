using Inchoqate.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.Main
{
    /// <summary>
    /// Interaction logic for EditorInputs.xaml
    /// </summary>
    public partial class EditorInputs : UserControl
    {
        public EditorInputs()
        {
            InitializeComponent();
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            app?.ChangeTheme(new Uri(BuildFiles.Get("Themes/Blue.xaml")));
        }
    }
}
