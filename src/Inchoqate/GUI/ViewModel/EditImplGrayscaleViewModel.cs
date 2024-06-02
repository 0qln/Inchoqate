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
using OpenTK.Mathematics;

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplGrayscaleViewModel : EditBaseLinearShader
    {
        private readonly Slider _intenstityControl;
        private readonly ObservableCollection<Control> _optionControls;

        public override ObservableCollection<Control> OptionControls => _optionControls;

        private double intensity;
        private Vector3 weights;

        public const double IntensityMin = 0;
        public const double IntensityMax = 1;

        public double Intensity
        {
            get => intensity;
            set
            {
                var val = Math.Clamp(value, IntensityMin, IntensityMax);
                _shader?.SetUniform("intensity", (float)val);
                SetProperty(ref intensity, val);
            }
        }

        public Vector3 Weights
        {
            get => weights;
            set
            {
                _shader?.SetUniform("weights", value);
                SetProperty(ref weights, value);
            }
        }


        public EditImplGrayscaleViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) 
            : base(usage)
        {
            Intensity = 2.0; // should be between 0 and 1, but yields interesting results for out of range values xd
            Weights = new(0.2126f, 0.7152f, 0.0722f);
            Title = "Grayscale";

            _intenstityControl = new() { Minimum = 0, Maximum = 1, Value = Intensity };
            _intenstityControl.SetBinding(
                Slider.ValueProperty, 
                new Binding("Intensity") { Source = this, Mode=BindingMode.TwoWay });

            _optionControls = [
                _intenstityControl
                // TODO: add a control for the weights.
            ];
        }


        public override ShaderModel? GetShader(out bool success) => 
            ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Grayscale.frag", UriKind.RelativeOrAbsolute),
                out success);
    }
}
