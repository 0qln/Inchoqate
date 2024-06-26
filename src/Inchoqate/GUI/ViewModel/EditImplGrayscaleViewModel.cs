﻿using Inchoqate.GUI.Model;
using Inchoqate.GUI.View;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using OpenTK.Mathematics;
using System.Windows.Media;
using Inchoqate.GUI.Converters;

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
                new Binding(nameof(Intensity)) { Source = this, Mode=BindingMode.TwoWay, Converter = new ElementToArrayConverter<double>() });

            _weightsControl = new() { RangeCount = 3, Minimum = 0, Maximum = 1, BackgroundGradientBrushes = [Colors.Red, Colors.Green, Colors.Blue], ShowRanges = [true, true, true]  };
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
}
