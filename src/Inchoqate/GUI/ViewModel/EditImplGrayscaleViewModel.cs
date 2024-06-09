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
using System.Globalization;
using System.Diagnostics;
using System.Windows.Media;

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplGrayscaleViewModel : EditBaseLinearShader
    {
        private readonly ExtSliderView _intenstityControl;
        private readonly ExtSliderView _weightsControl;
        private readonly ObservableCollection<ContentControl> _optionControls;

        public override ObservableCollection<ContentControl> OptionControls => _optionControls;

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


        public EditImplGrayscaleViewModel(BufferUsageHint usage) : base(usage)
        {
            Intensity = 2.0; // should be between 0 and 1, but yields interesting results for out of range values xd
            Weights = new(0.2126f, 0.7152f, 0.0722f);
            Title = "Grayscale";

            _intenstityControl = new() { Minimum = 0, Maximum = 1, Values = [Intensity], ShowValues = [true] };
            _intenstityControl.SetBinding(
                ExtSliderView.ValuesProperty, 
                new Binding(nameof(Intensity)) { Source = this, Mode=BindingMode.TwoWay, Converter = new DoubleToDoubleArrConverter() });

            _weightsControl = new() { RangeCount = 3, Minimum = 0, Maximum = 1, BackgroundGradientBrushes = [Colors.Red, Colors.Green, Colors.Blue], ShowValues = [true, false]  };
            _weightsControl.SetBinding(
                ExtSliderView.RangesProperty,
                new Binding(nameof(Weights)) { Source = this, Mode = BindingMode.TwoWay, Converter = new Vector3ToDoubleArrConverter() });

            _optionControls = [
                new() { Content = _intenstityControl, Name = nameof(Intensity) },
                new() { Content = _weightsControl, Name = nameof(Weights) }
            ];
        }

        public EditImplGrayscaleViewModel() : this(BufferUsageHint.StaticDraw)
        {
        }


        public override ShaderModel? GetShader(out bool success) => 
            ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Grayscale.frag", UriKind.RelativeOrAbsolute),
                out success);
    }


    public class Vector3ToDoubleArrConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vec = (Vector3)value;
            return new double[] { vec.X, vec.Y, vec.Z };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arr = (double[])value;
            return new Vector3((float)arr[0], (float)arr[1], (float)arr[2]);
        }
    }

    public class DoubleToDoubleArrConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (double)value;
            return new double[] { val };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arr = (double[])value;
            return arr[0];
        }
    }
}
