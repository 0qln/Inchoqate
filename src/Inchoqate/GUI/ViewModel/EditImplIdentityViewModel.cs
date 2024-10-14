using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.GUI.Model.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.ViewModel;

public class EditImplIdentityViewModel : EditBaseLinearShader
{
    public EditImplIdentityViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(usage)
    {
        Title = "Identity";
        OptionControls = [];
    }

    public override OptionControls OptionControls { get; }

    public override Shader? GetShader(out bool success) => Shader.FromSource(Shaders.BaseVert, Shaders.BaseFrag, out success);
}