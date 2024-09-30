using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using Inchoqate.Converters;
using Inchoqate.Graphics;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.View;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Inchoqate.UserControls;
using Microsoft.Extensions.Logging;
using Sorting;
using Sorting.Pixels._32;

namespace Inchoqate.GUI.ViewModel;

public class EditImplPixelSorterViewModel :
    EditBaseLinear,
    IEdit<PixelBuffer, PixelBuffer>,
    IDeserializable<EditImplPixelSorterViewModel>,
    IEventDelegate<AngleChangedEvent>, IAngleProperty,
    IEventDelegate<SorterChangedEvent>, ISorterProperty
{
    public const double AngleMin = 0;
    public const double AngleMax = Math.PI;
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<EditImplPixelSorterViewModel>();

    private double _angle, _angleChangeBegin;
    private Sorter32Bit.ISorter? _sorterConfig;


    public EditImplPixelSorterViewModel()
    {
        Title = "Pixel Sorter";

        ExtSlider angleControl = new() { Minimum = AngleMin, Maximum = AngleMax, Values = [0], ShowValues = [true] };
        angleControl.SetBinding(
            ExtSlider.ValuesProperty,
            new Binding(nameof(Angle)) { Source = this, Mode = BindingMode.TwoWay, Converter = new ArrayBoxingConverter<double>() });
        angleControl.ThumbDragStarted += (_, _) => _angleChangeBegin = _angle;
        angleControl.ThumbDragCompleted += (_, _) => Delegate(new AngleChangedEvent { OldValue = _angleChangeBegin, NewValue = _angle });

        DisplayComboBox sortersControl = new()
        {
            Items =
            [
                (new CombSorterView(), "Comb Sort" ),
            ]
        };
        // sortersControl.ComboBox.SetBinding(
        //     Selector.SelectedItemProperty,
        //     new Binding(nameof(Sorter)) { Source = this, Mode = BindingMode.OneWay, Converter = new SelectConverter<Control, Sorter32Bit.ISorter>(view => ((IViewModel<Sorter32Bit.ISorter>)view.DataContext).Model) });
        // sortersControl.ComboBox.SelectionChanged += (_, _) => Delegate(new SorterChangedEvent { OldValue = _sorterConfig, NewValue = _sorterConfig });

        OptionControls =
        [
            (angleControl, nameof(Angle)),
            (sortersControl, nameof(Sorter))
        ];
    }

    public IEventTree<EventViewModelBase>? DelegationTarget { get; init; }

    public override ObservableCollection<(Control, string)> OptionControls { get; }

    // TODO: default value and not nullable
    public Sorter32Bit.ISorter? Sorter
    {
        get => _sorterConfig;
        set => SetProperty(ref _sorterConfig, value);
    }

    public double Angle
    {
        get => _angle;
        set => SetProperty(ref _angle, value,
            validateValue: (_, val) => val is >= AngleMin and <= AngleMax);
    }

    public override bool Apply()
    {
        if (Sources is null || Destination is null || Sources.Length == 0 || Sorter is null)
            return false;

        // SorterConfig = new Sorter<Pixel32bitUnion>.CombSorter(new PixelComparer.Descending.Red())
        // {
        //     Pureness = 1
        // };

        var destination = Destination;
        var source = Sources[0];
        Debug.Assert(source.Data.Length == destination.Data.Length);

        unsafe
        {
            // The sorter is very lightweight, allocations shouldn't be a problem.
            var sorter = new Sorter32Bit(
                (Pixel32bitUnion*)Unsafe.AsPointer(ref source.Data[0]),
                source.Width,
                source.Height,
                source.Width * Texture.PixelDepth)
            {
                ParallelOpts =
                {
                    MaxDegreeOfParallelism = 4
                }
            };

            sorter.SortAngleAsync(Angle, sorter.GetAngleSorterInfo(Sorter));
        }

        // We can swap the buffers instead of copying the data from the source to the destination
        // buffer as we are not going to use the source buffer anymore.
        // TODO Sort methods that utilize auxiliary arrays, such as pigeonhole sort, could 
        // TODO utilize the second buffer for less memory allocations. 
        (Destination, Sources[0]) = (Sources[0], Destination);
        return true;
    }

    /// <inheritdoc />
    public PixelBuffer? Destination { get; set; }

    /// <inheritdoc />
    public PixelBuffer[]? Sources { get; set; }

    IEventTree<AngleChangedEvent>? IEventDelegate<AngleChangedEvent>.DelegationTarget => DelegationTarget;
    IEventTree<SorterChangedEvent>? IEventDelegate<SorterChangedEvent>.DelegationTarget => DelegationTarget;

    public bool Delegate(AngleChangedEvent @event)
    {
        @event.Object = this;
        return DelegationTarget?.Novelty(@event, true) ?? false;
    }

    public bool Delegate(SorterChangedEvent @event)
    {
        @event.Object = this;
        return DelegationTarget?.Novelty(@event, true) ?? false;
    }
}