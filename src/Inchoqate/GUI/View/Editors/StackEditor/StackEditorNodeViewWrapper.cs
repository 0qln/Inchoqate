using System.Globalization;
using System.Windows.Data;
using Inchoqate.GUI.View.Edits;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Editors;

namespace Inchoqate.GUI.View.Editors.StackEditor;

// Creating new NodeViews each time the collection is changed will discard the 
// state of the view each time, which breaks thumbs and other state dependent controls.

[ValueConversion(typeof(EditorNodeViewModelCollectionLinear), typeof(StackEditorNodeCollection))]
public class StackEditorNodeViewWrapper : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        var source = (EditorNodeViewModelCollectionLinear)value;
        return StackEditorNodeCollection.Mirror(source,
            x => new StackEditorNodeView { Contents = ViewForAttribute.CreateViewFor(x.GetType()), SelfContainer = source },
            x => x.ViewContents);
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}