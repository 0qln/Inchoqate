using System.Diagnostics;
using System.Runtime.CompilerServices;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using Sorting;
using Sorting.Pixels._32;

namespace Inchoqate.GUI.ViewModel.Edits;

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

    private double _angle;
    private Sorter<Pixel32bitUnion>.ISorter? _sorterConfig;


    public EditImplPixelSorterViewModel()
    {
        Title = "Pixel Sorter";
    }

    public double Angle
    {
        get => _angle;
        set => SetProperty(ref _angle, value, validateValue: (this as IAngleProperty).IsValid);
    }

    public override bool Apply()
    {
        if (Sources is null || Destination is null || Sources.Length == 0 || Sorter is null)
            return false;

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

    /// <inheritdoc />
    public IEventReceiver? DelegationTarget { get; set; }

    // TODO: default value and not nullable
    public Sorter<Pixel32bitUnion>.ISorter? Sorter
    {
        get => _sorterConfig;
        set => SetProperty(ref _sorterConfig, value);
    }
}