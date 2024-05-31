using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplPixelsortViewModel : EditBaseLinear
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<EditImplPixelsortViewModel>();

        private readonly Slider _intenstityControl;
        private readonly ObservableCollection<Control> _optionControls;

        public override ObservableCollection<Control> OptionControls => _optionControls;

        private double angle = 0;

        public double Angle
        {
            get => angle;
            set
            {
                SetProperty(ref angle, value);
            }
        }


        public EditImplPixelsortViewModel() 
        {
            _intenstityControl = new() { Minimum = 0, Maximum = double.Pi * 2, Value = 0 };
            _intenstityControl.SetBinding(
                Slider.ValueProperty, 
                new Binding("Angle") { Source = this, Mode=BindingMode.TwoWay });

            _optionControls = [
                _intenstityControl
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
}
