using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.ViewModel;

public class EditImplIdentityViewModel : EditBaseLinearShader
{
    public EditImplIdentityViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(usage)
    {
        Title = "Identity";
        OptionControls = [];
    }

    public override ObservableCollection<(Control, string)> OptionControls { get; }

    public override Shader? GetShader(out bool success)
    {
        return Model.Shader.FromUri(
            new("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
            out success);
    }
}