using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Inchoqate.Converters;

namespace Inchoqate.UserControls;

/// <summary>
///     Interaction logic for ExtComboBoxView.xaml
/// </summary>
public partial class DisplayComboBox : UserControl
{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(ObservableCollection<(Control Content, string Name)>),
        typeof(DisplayComboBox),
        new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(ValueTuple<Control, string>),
        typeof(DisplayComboBox),
        new FrameworkPropertyMetadata(ValueTuple.Create<Control, string>(null, null),
            FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure));


    public DisplayComboBox()
    {
        InitializeComponent();
    }

    public ObservableCollection<(Control Content, string Name)>? Items
    {
        get => (ObservableCollection<(Control Content, string Name)>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public ValueTuple<Control, string>? SelectedItem
    {
        get => (ValueTuple<Control, string>?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItem = Items?.FirstOrDefault(x => x.Name.Equals(ComboBox.SelectedItem));
    }
}

public class NameSelector() : SelectConverter<(Control, string), string>(x => x.Item2)
{
}

public class ControlSelector() : SelectConverter<(ContentControl, string), Control>(x => x.Item1)
{
}