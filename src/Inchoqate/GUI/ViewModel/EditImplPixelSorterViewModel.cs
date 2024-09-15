using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using Sorting;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;

namespace Inchoqate.GUI.ViewModel;

public class EditImplPixelSorterViewModel : EditBaseLinear, IEditModel<PixelBufferModel, PixelBufferModel>
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<EditImplPixelSorterViewModel>();

    public override ObservableCollection<ContentControl> OptionControls { get; }

    private double _angle;

    public double Angle
    {
        get => _angle;
        set => SetProperty(ref _angle, value);
    }


    public EditImplPixelSorterViewModel() 
    {
        Title = "Pixel Sorter";
        Slider angleControl = new() { Minimum = 0, Maximum = double.Pi * 2, Value = 0 };
        angleControl.SetBinding(
            Slider.ValueProperty, 
            new Binding(nameof(Angle)) { Source = this, Mode=BindingMode.TwoWay });

        OptionControls = [
            new() { Content = angleControl, Name = nameof(Angle) }
        ];
    }

    public override bool Apply()
    {
        // if (sources.Length == 0 || sources[0] != destination)
        // {
        //     // Makes the actual computation later easier.
        //     _logger.LogWarning("The source and destination buffers should be the same.");
        //     return false;
        // }
        // else
        {
            return Apply(Destination, Sources[0]);
        }

        return false;
    }

    /// <inheritdoc />
    public PixelBufferModel Destination { get; set; }

    /// <inheritdoc />
    public PixelBufferModel[] Sources { get; set; }

    public bool Apply(PixelBufferModel destination, PixelBufferModel source)
    {
        Debug.Assert(source.Data.Length == destination.Data.Length);
        Array.Copy(source.Data, destination.Data, source.Data.Length);
        // unsafe
        // {
        //     var pixelSorter = new Sorter32Bit(
        //         (Pixel32bitUnion*)Unsafe.AsPointer(ref source.Data[0]),
        //         source.Width,
        //         source.Height,
        //         source.Width * 4);
        //     var comparer = new PixelComparer.Ascending.Red();
        //     var sorter = new Sorter32Bit.IntrospectiveSorter(comparer);
        //     pixelSorter.SortAngle(0, pixelSorter.GetAngleSorterInfo(sorter));
        // }
        return true;
    }
}