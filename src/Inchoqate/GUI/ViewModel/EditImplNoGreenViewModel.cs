using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.ViewModel;

public class EditImplNoGreenViewModel : EditBaseLinearShader
{
    public EditImplNoGreenViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(usage)
    {
        Title = "No Green";
        OptionControls = [];
    }

    public override ObservableCollection<ContentControl> OptionControls { get; }

    public override ShaderModel? GetShader(out bool success)
    {
        return ShaderModel.FromUri(
            new("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new("/Shaders/NoGreen.frag", UriKind.RelativeOrAbsolute),
            out success);
    }
}