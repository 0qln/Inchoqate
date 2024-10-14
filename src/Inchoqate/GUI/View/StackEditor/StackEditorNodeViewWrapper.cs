using System.Globalization;
using System.Windows.Data;
using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.View.StackEditor;

// Creating new NodeViews each time the collection is changed will discard the 
// state of the view each time, which breaks thumbs and other state dependent controls.

[ValueConversion(typeof(EditorNodeCollectionLinear), typeof(StackEditorNodeCollection))]
public class StackEditorNodeViewWrapper : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EditorNodeCollectionLinear source)
        {
            return StackEditorNodeCollection.Mirror(
                source, 
                x => new StackEditorNodeView { ViewModel = x, SelfContainer = source }, 
                x => x.ViewModel); 
        }

        throw new ArgumentException(
            $"Expecting a ViewModelCollection of type {typeof(EditorNodeCollectionLinear)}", 
            nameof(value));
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}