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
                ObservableCollectionBase<StackEditorNodeView> result = new StackEditorNodeCollection();

                // TODO: extract this into class 'EventSourceMirror'.
                // Mirror relevant events that are applied to the source collection.
                source.EventRelayed += (sender, e) =>
                {
                    Debug.Assert(sender == source);

                    switch (e.Event)
                    {
                        case SwapItemsEvent @event:
                            @event.Apply(result);
                            @event.Occured += (s, e) => { @event.Apply(result); };
                            @event.Reverted += (s, e) => { @event.Revert(result); };
                            break;

                        case AddItemEvent<EditBaseLinear> @event:
                            @event.Apply(result, x => new StackEditorNodeView() { ViewModel = x, BackingCollection = source });
                            @event.Occured += (s, e) => { @event.Apply(result, x => new StackEditorNodeView() { ViewModel = x, BackingCollection = source }); };
                            @event.Reverted += (s, e) => { @event.Revert(result, x => result.First(y => y.ViewModel == x)); };
                            break;
                    }
                };

                return result; 
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

            DataContext = _viewModel = new();
        }
    }
}
