using System.Windows;
using System.Windows.Controls;

namespace Inchoqate.GUI.View.DisplayComboBox;

/// <summary>
///     Interaction logic for ExtComboBoxView.xaml
/// </summary>
public partial class DisplayComboBox : UserControl
{
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(DisplayComboBoxItems),
        typeof(DisplayComboBox),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(DisplayComboBoxItem),
        typeof(DisplayComboBox),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure));


    public DisplayComboBox()
    {
        InitializeComponent();
    }

    public DisplayComboBoxItems? Items
    {
        get => (DisplayComboBoxItems?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public DisplayComboBoxItem? SelectedItem
    {
        get => (DisplayComboBoxItem?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItem = Items?.FirstOrDefault(x => x.Name.Equals(ComboBox.SelectedItem));
    }
}