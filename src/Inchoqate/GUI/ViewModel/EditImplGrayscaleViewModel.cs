using Inchoqate.GUI.Model;
using Inchoqate.GUI.View;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using OpenTK.Mathematics;
using System.Windows.Media;
using Inchoqate.GUI.Converters;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqtae.GUI.ViewModel.Events;
using Newtonsoft.Json;
using Inchoqate.Converters;

namespace Inchoqate.GUI.ViewModel;

public class EditImplGrayscaleViewModel : EditBaseLinearShader, IEventRelayModel<IntensityChangedEvent>, IIntensityProperty, IGuidHolder
{
    private readonly EventTreeViewModel _eventTree;

    private readonly ExtSliderView _intenstityControl;
    private readonly ExtSliderView _weightsControl;

    public override ObservableCollection<ContentControl> OptionControls { get; }

    private double _intensity, _intensityChangeBegin;
    private Vector3 _weights;

    public const double IntensityMin = 0;
    public const double IntensityMax = 1;

    public Guid Id { get; } = Guid.NewGuid();

    public double Intensity
    {
        get => _intensity;
        set => SetProperty(ref _intensity, value,
            validateValue: (_, val) => val is >= IntensityMin and <= IntensityMax,
            onChanged: () => _shader?.SetUniform("intensity", (float)value));
    }

    [JsonConverter(typeof(Vector3JsonConverter))]
    public Vector3 Weights
    {
        get => _weights;
        set => SetProperty(ref _weights, value,
            onChanged: () => _shader?.SetUniform("weights", value));
    }


    public EditImplGrayscaleViewModel(EventTreeViewModel eventTree, BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(usage)
    {
        _eventTree = eventTree;

        Intensity = 2.0; // should be between 0 and 1, but yields interesting results for out of range values xd
        Weights = new(0.2126f, 0.7152f, 0.0722f);
        Title = "Grayscale";

        ExtSliderView intensityControl = new() { Minimum = 0, Maximum = 1, Values = [Intensity], ShowValues = [true] };
        intenstityControl.SetBinding(
            ExtSliderView.ValuesProperty,
            new Binding(nameof(Intensity))
                { Source = this, Mode = BindingMode.TwoWay, Converter = new ElementToArrayConverter<double>() });
        intensityControl.ThumbDragCompleted += (s, e) => Eventuate<IntensityChangedEvent, IIntensityProperty>(new(_intensityChangeBegin, _intensity));
        intensityControl.ThumbDragStarted += (s, e) => _intensityChangeBegin = Intensity;

        ExtSliderView weightsControl = new()
        {
            RangeCount = 3, Minimum = 0, Maximum = 1,
            BackgroundGradientBrushes = [Colors.Red, Colors.Green, Colors.Blue], ShowRanges = [true, true, true]
        };
        weightsControl.SetBinding(
            ExtSliderView.RangesProperty,
            new Binding(nameof(Weights))
                { Source = this, Mode = BindingMode.TwoWay, Converter = new Vector3ToDoubleArrConverter() });

        OptionControls = [
            new() { Content = intensityControl, Name = nameof(Intensity) },
            new() { Content = weightsControl, Name = nameof(Weights) }
        ];
    }


    public override ShaderModel? GetShader(out bool success) =>
        ShaderModel.FromUri(
            new("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new("/Shaders/Grayscale.frag", UriKind.RelativeOrAbsolute),
            out success);

    public bool Eventuate<TEvent, TParam>(TEvent @event) 
        where TEvent : IntensityChangedEvent, IParameterInjected<TParam>
    {
        @event.Parameter = this;
        return _eventTree.Novelty(@event, true);
    }
}