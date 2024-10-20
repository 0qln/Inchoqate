using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.View.SharedConverters;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Inchoqate.GUI.ViewModel.Edits;

public class EditImplGrayscaleViewModel :
    EditBaseLinearShader,
    IEventDelegate<IntensityChangedEvent, IIntensityProperty>, IIntensityProperty,
    IEventDelegate<WeightsChangedEvent, IWeightsProperty>, IWeightsProperty,
    IDeserializable<EditImplGrayscaleViewModel>
{
    private double _intensity;
    private Vector3 _weights;

    public EditImplGrayscaleViewModel() : this(BufferUsageHint.StaticDraw)
    {
    }

    public EditImplGrayscaleViewModel(BufferUsageHint usage) : base(usage)
    {
        Intensity = 2.0; // yields interesting results for out of range values
        Weights = new(0.2126f, 0.7152f, 0.0722f);
        Title = "Grayscale";
    }

    public IEventReceiver? DelegationTarget { get; set; }

    public double Intensity
    {
        get => _intensity;
        set => SetProperty(ref _intensity, value, 
            validateValue: (this as IIntensityProperty).IsValid,
            onChanged: () => Shader?.SetUniform(nameof(_intensity), (float)value));
    }

    [JsonConverter(typeof(Vector3JsonConverter))]
    public Vector3 Weights
    {
        get => _weights;
        set => SetProperty(ref _weights, value,
            validateValue: (this as IWeightsProperty).IsValid,
            onChanged: () => Shader?.SetUniform(nameof(_weights), value));
    }


    public override Shader? GetShader(out bool success)
    {
        return Shader.FromSource(Shaders.BaseVert, Shaders.Grayscale, out success);
    }
}