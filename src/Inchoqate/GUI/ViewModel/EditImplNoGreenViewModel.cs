using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.ViewModel;

public class EditImplNoGreenViewModel : EditBaseLinearShader, IDeserializable<EditImplNoGreenViewModel>
{
    public EditImplNoGreenViewModel() : this(BufferUsageHint.StaticDraw) { }

    public EditImplNoGreenViewModel(BufferUsageHint usage) : base(usage)
    {
        Title = "No Green";
        OptionControls = [];
    }

    public override ObservableCollection<(Control, string)> OptionControls { get; }

    public override Shader? GetShader(out bool success) => Shader.FromSource(Shaders.BaseVert, Shaders.NoGreen, out success);
}