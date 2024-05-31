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
    /// Interaction logic for IntensityEditOptionView.xaml
    /// </summary>
    public partial class EditOptIntensityView : UserControl
    {
        //private readonly EditOptBaseViewModel<float> _viewModel;
        
        public EditOptIntensityView()
        {
            InitializeComponent();

            //_viewModel = new EditOptBaseViewModel<float>(1.0f);
            //DataContext = _viewModel;
        }
    }
}
