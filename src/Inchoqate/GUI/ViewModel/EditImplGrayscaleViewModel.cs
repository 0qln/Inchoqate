using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using OpenTK.Mathematics;
using System.Windows.Media;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.View.Converters;
using Inchoqate.GUI.View.MultiSlider;
using Inchoqate.GUI.ViewModel.Events;
using Newtonsoft.Json;
using Inchoqate.GUI.View.SharedConverters;

namespace Inchoqate.GUI.ViewModel;

public class EditImplGrayscaleViewModel : 
    EditBaseLinearShader,
    IEventDelegate<IntensityChangedEvent>, IIntensityProperty,
    IDeserializable<EditImplGrayscaleViewModel>
{
    public IEventTree<EventViewModelBase>? DelegationTarget { get; init; }

    IEventTree<IntensityChangedEvent>? IEventDelegate<IntensityChangedEvent>.DelegationTarget => DelegationTarget;

    public override OptionControls OptionControls { get; }

    private double _intensity;
    private double _intensityChangeBegin;
    private Vector3 _weights;

    public const double IntensityMin = 0;
    public const double IntensityMax = 1;

    public double Intensity
    {
        get => _intensity;
        set => SetProperty(ref _intensity, value,
            validateValue: (_, val) => val is >= IntensityMin and <= IntensityMax,
            onChanged: () => Shader?.SetUniform(nameof(_intensity), (float)value));
    }

    [JsonConverter(typeof(Vector3JsonConverter))]
    public Vector3 Weights
    {
        get => _weights;
        set => SetProperty(ref _weights, value,
            validateValue: (_, vec) => vec.All(c => c is >= 0 and <= 1) && Math.Abs(vec.Sum() - 1) < 0.00001,
            onChanged: () => Shader?.SetUniform(nameof(_weights), value));
    }

    public EditImplGrayscaleViewModel() : this(BufferUsageHint.StaticDraw) { }

    public EditImplGrayscaleViewModel(BufferUsageHint usage) : base(usage)
    {
        Intensity = 2.0; // should be between 0 and 1, but yields interesting results for out of range values xd
        Weights = new(0.2126f, 0.7152f, 0.0722f);
        Title = "Grayscale";

        MultiSlider intensityControl = new() { Minimum = 0, Maximum = 1, Values = [Intensity], ShowValues = [true] };
        intensityControl.SetBinding(
            MultiSlider.ValuesProperty,
            new Binding(nameof(Intensity))
                { Source = this, Mode = BindingMode.TwoWay, Converter = new ArrayBoxingConverter<double>() });
        intensityControl.ThumbDragCompleted += (s, e) => Delegate(new() { OldValue = _intensityChangeBegin, NewValue = _intensity });
        intensityControl.ThumbDragStarted += (s, e) => _intensityChangeBegin = Intensity;

        MultiSlider weightsControl = new()
        {
            RangeCount = 3, Minimum = 0, Maximum = 1,
            BackgroundGradientBrushes = [Colors.Red, Colors.Green, Colors.Blue], ShowRanges = [true, true, true]
        };
        weightsControl.SetBinding(
            MultiSlider.RangesProperty,
            new Binding(nameof(Weights))
                { Source = this, Mode = BindingMode.TwoWay, Converter = new Vector3ToDoubleArrConverter() });

        OptionControls = [
            new(intensityControl, nameof(Intensity)),
            new(weightsControl, nameof(Weights))
        ];
    }


    public override Shader? GetShader(out bool success) => Shader.FromSource(Shaders.BaseVert, Shaders.Grayscale, out success);

    public bool Delegate(IntensityChangedEvent @event) 
    {
        @event.Object = this;
        return DelegationTarget?.Novelty(@event, true) ?? false;
    }
}