using System.Collections.ObjectModel;
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
        typeof(ObservableCollection<FrameworkElement>),
        typeof(DisplayComboBox),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(FrameworkElement),
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

    public ObservableCollection<FrameworkElement>? Items
    {
        get => (ObservableCollection<FrameworkElement>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public FrameworkElement? SelectedItem
    {
        get => (FrameworkElement?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItem = Items?.FirstOrDefault(x => x.Name.Equals(ComboBox.SelectedItem));
    }
}
