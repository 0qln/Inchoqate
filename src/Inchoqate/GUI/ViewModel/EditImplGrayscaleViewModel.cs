using Inchoqate.GUI.Model;
using Inchoqate.GUI.View;
using System.ComponentModel;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplGrayscaleViewModel : EditBaseLinearShader
    {
        private readonly Slider _intenstityControl;
        private readonly ObservableCollection<Control> _optionControls;

        public override ObservableCollection<Control> OptionControls => _optionControls;

        private double intensity;
        public const double IntensityMin = 0;
        public const double IntensityMax = 1;

        public double Intensity
        {
            get => intensity;
            set
            {
                var val = Math.Clamp(value, IntensityMin, IntensityMax);
                _shader?.SetUniform("intensity", (float)value);
                SetProperty(ref intensity, value);
            }
        }


        public EditImplGrayscaleViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) 
            : base(usage)
        {
            // should be between 0 and 1, but yields interesting results for out of range values xd
            Intensity = -2.0;
            _intenstityControl = new() { Minimum = 0, Maximum = 1, Value = Intensity };
            _intenstityControl.SetBinding(
                Slider.ValueProperty, 
                new Binding("Intensity") { Source = this, Mode=BindingMode.TwoWay });

            _optionControls = [
                _intenstityControl
            ];
        }


        public override ShaderModel? GetShader(out bool success) => 
            ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Grayscale.frag", UriKind.RelativeOrAbsolute),
                out success);
    }
}
