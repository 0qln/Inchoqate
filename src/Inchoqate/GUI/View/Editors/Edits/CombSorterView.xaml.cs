using System.Windows.Controls;
using Inchoqate.GUI.ViewModel.Edits;

namespace Inchoqate.GUI.View.Editors.Edits
{
    /// <summary>
    /// Interaction logic for CombSort.xaml
    /// </summary>
    public partial class CombSorterView : UserControl
    {
        public CombSorterView()
        {
            DataContext = new CombSorterSorterViewModel();

            InitializeComponent();
        }
    }
}
