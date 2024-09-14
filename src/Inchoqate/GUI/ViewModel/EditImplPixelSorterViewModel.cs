using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.ViewModel;

public class EditImplPixelSorterViewModel : EditBaseLinear
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

    public override bool Apply(IEditDestinationModel destination, params IEditSourceModel[] sources)
    {
        if (sources.Length == 0 || sources[0] != destination)
        {
            // Makes the actual computation later easier.
            _logger.LogWarning("The source and destination buffers should be the same.");
            return false;
        }
        else
        {
            if (destination is PixelBufferModel buffer)
            {
                return Apply(buffer, buffer);
            }
        }

        return false;
    }

    public bool Apply(PixelBufferModel destination, PixelBufferModel source)
    {
        throw new NotImplementedException();
    }
}