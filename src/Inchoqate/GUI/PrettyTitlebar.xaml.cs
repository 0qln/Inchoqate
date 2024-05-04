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

namespace Inchoqate.GUI
{
    /// <summary>
    /// Interaction logic for PrettyTitlebar.xaml
    /// </summary>
    public partial class PrettyTitlebar : UserControl
    {
        public Window ContentWindow { get; private set; }


        public void Initiate(Window contentWindow)
        {
            ContentWindow = contentWindow;

            Background = Brushes.Red;
        }

#pragma warning disable CS8618 
        public PrettyTitlebar()
        {
            InitializeComponent();
        }
#pragma warning restore CS8618 
    }
}
