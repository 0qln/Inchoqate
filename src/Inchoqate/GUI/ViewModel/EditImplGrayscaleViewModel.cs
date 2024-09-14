using Inchoqate.GUI.Model;
using Inchoqate.GUI.View;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using OpenTK.Mathematics;
using System.Windows.Media;
using Inchoqate.GUI.Converters;
using Newtonsoft.Json;
using Inchoqate.Converters;

namespace Inchoqate.GUI.ViewModel;

public class EditImplGrayscaleViewModel : EditBaseLinearShader
{
    [JsonIgnore]
    public override ObservableCollection<ContentControl> OptionControls { get; }

    private double _intensity;
    private Vector3 _weights;

    public const double IntensityMin = 0;
    public const double IntensityMax = 1;

    public double Intensity
    {
        get => _intensity;
        set
        {
            var val = Math.Clamp(value, IntensityMin, IntensityMax);
            _shader?.SetUniform(nameof(_intensity), (float)val);
            SetProperty(ref _intensity, val);
        }
    }

    [JsonConverter(typeof(Vector3JsonConverter))]
    public Vector3 Weights
    {
        get => _weights;
        set
        {
            _shader?.SetUniform(nameof(_weights), value);
            SetProperty(ref _weights, value);
        }
    }


    public EditImplGrayscaleViewModel(BufferUsageHint usage) : base(usage)
    {
        Intensity = 2.0; // should be between 0 and 1, but yields interesting results for out of range values xd
        Weights = new(0.2126f, 0.7152f, 0.0722f);
        Title = "Grayscale";

        ExtSliderView intensityControl = new() { Minimum = 0, Maximum = 1, Values = [Intensity], ShowValues = [true] };
        intensityControl.SetBinding(
            ExtSliderView.ValuesProperty, 
            new Binding(nameof(Intensity)) { Source = this, Mode=BindingMode.TwoWay, Converter = new ElementToArrayConverter<double>() });

        ExtSliderView weightsControl = new() { RangeCount = 3, Minimum = 0, Maximum = 1, BackgroundGradientBrushes = [Colors.Red, Colors.Green, Colors.Blue], ShowRanges = [true, true, true]  };
        weightsControl.SetBinding(
            ExtSliderView.RangesProperty,
            new Binding(nameof(Weights)) { Source = this, Mode = BindingMode.TwoWay, Converter = new Vector3ToDoubleArrConverter() });

        OptionControls = [
            new() { Content = intensityControl, Name = nameof(Intensity) },
            new() { Content = weightsControl, Name = nameof(Weights) }
        ];
    }

    public EditImplGrayscaleViewModel() : this(BufferUsageHint.StaticDraw)
    {
    }


    public override ShaderModel? GetShader(out bool success) => 
        ShaderModel.FromUri(
            new("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new("/Shaders/Grayscale.frag", UriKind.RelativeOrAbsolute),
            out success);
}