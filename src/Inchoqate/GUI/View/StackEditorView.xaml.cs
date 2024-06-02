using Inchoqate.GUI.ViewModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.View
{
    [ValueConversion(typeof(EditorNodeCollectionLinear), typeof(ObservableCollection<StackEditorNodeView>))]
    public class StackEditorNodeViewWrapper : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditorNodeCollectionLinear viewModels)
            {
                return new ObservableCollection<StackEditorNodeView>(
                    viewModels.Select(vm => new StackEditorNodeView() { ViewModel = vm }));
            }

            throw new ArgumentException(
                $"Expecting a ViewModelCollection of type {typeof(EditorNodeCollectionLinear)}"
                , nameof(value));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Interaction logic for StackEditorView.xaml
    /// </summary>
    public partial class StackEditorView : UserControl
    {
        private readonly StackEditorViewModel _viewModel;

        public StackEditorView()
        {
            InitializeComponent();

            _viewModel = new();
            DataContext = _viewModel;
        }
    }
}
