using System.Windows.Controls;
using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.View.Edits
{
    /// <summary>
    /// Interaction logic for CombSort.xaml
    /// </summary>
    public partial class CombSorterView : UserControl
    {
        public CombSorterView()
        {
            DataContext = new CombSorterViewModel();

            InitializeComponent();
        }
    }
}
