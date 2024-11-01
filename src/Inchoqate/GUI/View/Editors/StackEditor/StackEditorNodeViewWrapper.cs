using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Inchoqate.GUI.View.Editors.Edits;
using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.GUI.ViewModel.Edits;

namespace Inchoqate.GUI.View.Editors.StackEditor;

// Creating new NodeViews each time the collection is changed will discard the 
// state of the view each time, which breaks thumbs and other state dependent controls.

[ValueConversion(typeof(EditorNodeViewModelCollection), typeof(StackEditorNodeCollection))]
public class StackEditorNodeViewWrapper : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        var source = (EditorNodeViewModelCollection)value;
        var result = new StackEditorNodeCollection { DelegationTarget = source.DelegationTarget };
        result.Mirror(source,
            x => new()
            {
                Edit = (EditBaseView)Activator.CreateInstance(x.GetType().GetCustomAttribute<ForViewAttribute>()!.ViewType)!,
                SelfContainer = result,
            },
            x => x.Edit!.ViewModel);
        return result;
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}