﻿using Inchoqate.GUI.Model;
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

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplGrayscaleViewModel : EditBaseLinearShader
    {
        private readonly Slider _intenstityControl;
        private readonly ExtSliderView _weightsControl;
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


        public EditImplGrayscaleViewModel(BufferUsageHint usage) : base(usage)
        {
            Intensity = 2.0; // should be between 0 and 1, but yields interesting results for out of range values xd
            Weights = new(0.2126f, 0.7152f, 0.0722f);
            Title = "Grayscale";

            _intenstityControl = new() { Minimum = 0, Maximum = 1, Value = Intensity };
            _intenstityControl.SetBinding(
                Slider.ValueProperty, 
                new Binding("Intensity") { Source = this, Mode=BindingMode.TwoWay });

            _weightsControl = new() { RangeCount = 3, Minimum = 0, Maximum = 1 };
            _weightsControl.SetBinding(
                ExtSliderView.ValuesProperty,
                new Binding("Weights") { Source = this, Mode = BindingMode.TwoWay, Converter = new Vector3ToDoubleArrConverter() });

            _optionControls = [
                _intenstityControl,
                _weightsControl 
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
            //var val1 = new ObservableStruct<double>(vec.X);
            //var val2 = new ObservableStruct<double>(vec.Y + vec.X);
            //return new ObservableItemCollection<ObservableStruct<double>>() { val1, val2 };
            //return new ObservableCollection<double> { (double)vec.X, (double)vec.Y + (double)vec.X };
            //return new ObservableItemCollection<ObservableStruct<double>>(((float[])[vec.X, vec.Y, vec.Z]).Select(x => new ObservableStruct<double>(x)));
            return new double[] { vec.X, vec.Y + vec.X };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var arr = (ObservableItemCollection<ObservableStruct<double>>)value;
            //var arr = (ObservableCollection<double>)value;
            var arr = (double[])value;
            //return new Vector3(0, (float)arr[0].Value, (float)arr[1].Value - (float)arr[0].Value);
            var x = arr[0];
            var y = arr[1] - arr[0];
            var z = 1 - y - x;
            return new Vector3((float)x, (float)y, (float)z);
            //return new Vector3((float)arr[0].Value, (float)arr[1].Value, (float)arr[2].Value);
        }
    }
}
