using Inchoqate.GUI.Events;
using Inchoqate.GUI.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.View
{
    public class StackEditorNodeCollection : ObservableCollectionBase<StackEditorNodeView>
    {
    }


    // Creating new NodeViews each time the collection is changed will discard the 
    // state of the view each time, which breaks thumbs and other state dependand controls.

    [ValueConversion(typeof(EditorNodeCollectionLinear), typeof(StackEditorNodeCollection))]
    public class StackEditorNodeViewWrapper : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditorNodeCollectionLinear source)
            {
                return StackEditorNodeCollection.Mirror(
                    source, 
                    x => new StackEditorNodeView() { ViewModel = x, BackingCollection = source }, 
                    x => x.ViewModel); 
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
        public StackEditorView()
        {
            InitializeComponent();

            DataContext = new StackEditorViewModel();
        }
    }
}
