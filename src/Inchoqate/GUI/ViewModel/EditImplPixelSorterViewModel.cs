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
using Sorting.Pixels.KeySelector;

namespace Inchoqate.GUI.ViewModel;

public class EditImplPixelSorterViewModel : EditBaseLinear, IEditModel<PixelBufferModel, PixelBufferModel>, IDeserializable<EditImplPixelSorterViewModel>
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
        Slider angleControl = new() { Minimum = 0, Maximum = double.Pi * 0.99, Value = 0 };
        angleControl.SetBinding(
            Slider.ValueProperty, 
            new Binding(nameof(Angle)) { Source = this, Mode=BindingMode.TwoWay });

        OptionControls = [
            new() { Content = angleControl, Name = nameof(Angle) }
        ];
    }

    public override bool Apply()
    {
        if (Sources.Length == 0)
            return false;

        PixelBufferModel destination = Destination;
        PixelBufferModel source = Sources[0];
        Debug.Assert(source.Data.Length == destination.Data.Length);
        unsafe
        {
            var pixelSorter = new Sorter32Bit(
                (Pixel32bitUnion*)Unsafe.AsPointer(ref source.Data[0]),
                source.Width,
                source.Height,
                source.Width * TextureModel.PixelDepth)
            {
                ParallelOpts =
                {
                    MaxDegreeOfParallelism = -1
                }
            };
            var comparer = new OrderedKeySelector.Ascending.Red();
            var sorter = new Sorter32Bit.PigeonholeSorter(comparer);
            pixelSorter.SortAngle(Angle, pixelSorter.GetAngleSorterInfo(sorter));
        }

        // We can swap the buffers instead of copying the data from the source to the destination
        // buffer as we are not going to use the source buffer anymore.
        (Destination, Sources[0]) = (Sources[0], Destination);
        return true;
    }

    /// <inheritdoc />
    public PixelBufferModel Destination { get; set; }

    /// <inheritdoc />
    public PixelBufferModel[] Sources { get; set; }
}