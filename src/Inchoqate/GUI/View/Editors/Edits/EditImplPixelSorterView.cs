using System.Windows;
using System.Windows.Data;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.View.Converters;
using Inchoqate.GUI.View.DisplayComboBox;
using Inchoqate.GUI.View.Editors.Edits;
using Inchoqate.GUI.ViewModel.Edits;
using Inchoqate.GUI.ViewModel.Events;
using Sorting;
using Sorting.Pixels._32;

namespace Inchoqate.GUI.View.Edits;

public class EditImplPixelSorterView : EditBaseView
{
    private double _angleChangeBegin;

    public EditImplPixelSorterView(EditImplPixelSorterViewModel viewModel) : base(viewModel, [])
    {
        MultiSlider.MultiSlider angleControl = new()
        {
            Minimum = EditImplPixelSorterViewModel.AngleMin,
            Maximum = EditImplPixelSorterViewModel.AngleMax,
            Values = [0], ShowValues = [true]
        };
        angleControl.SetBinding(
            MultiSlider.MultiSlider.ValuesProperty,
            new Binding(nameof(EditImplPixelSorterViewModel.Angle))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = new ArrayBoxingConverter<double>()
            });
        angleControl.ThumbDragStarted += (_, _) => _angleChangeBegin = viewModel.Angle;
        angleControl.ThumbDragCompleted += (_, _) => viewModel.Delegate(new AngleChangedEvent { OldValue = _angleChangeBegin, NewValue = viewModel.Angle });

        DisplayComboBox.DisplayComboBox sortersControl = new()
        {
            Items =
            [
                new("Comb Sort", new CombSorterView())
            ]
        };
        sortersControl.ComboBox.SetBinding(
            DisplayComboBox.DisplayComboBox.SelectedItemProperty,
            new Binding(nameof(EditImplPixelSorterViewModel.Sorter))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = new SelectConverter<Sorter<Pixel32bitUnion>.ISorter, DisplayComboBoxItem>(
                    sorter => sortersControl.Items.FirstOrDefault(item => GetSorter(item) == sorter),
                    GetSorter)
            });
        sortersControl.ComboBox.SelectionChanged += (_, _) => viewModel.Delegate(new SorterChangedEvent { OldValue = viewModel.Sorter, NewValue = viewModel.Sorter });
    }

    private static Sorter<Pixel32bitUnion>.ISorter? GetSorter(DisplayComboBoxItem item)
    {
        var frameworkElement = item.Element as FrameworkElement;
        var sorterViewModel = frameworkElement?.DataContext as ISorterViewModel;
        return sorterViewModel?.Sorter;
    }
}