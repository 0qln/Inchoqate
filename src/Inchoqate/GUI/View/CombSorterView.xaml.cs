using System.Windows.Controls;
using Inchoqate.GUI.View.Converters;
using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.View
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

    public class PurenessConverter() : SelectConverter<int?, string?>(convertBack: s => int.TryParse(s, out var value) ? value : null);
}
