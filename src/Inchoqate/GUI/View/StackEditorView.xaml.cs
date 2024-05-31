using Inchoqate.GUI.ViewModel;
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

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for StackEditorView.xaml
    /// </summary>
    public partial class StackEditorView : UserControl
    {
        private readonly StackEditorViewModel _viewModel;

        public StackEditorView()
        {
            InitializeComponent();

            DataContext = _viewModel;
        }
    }
}
