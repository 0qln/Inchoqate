using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Inchoqate.GUI.ViewModel;

public class EditImplIdentityViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) 
    : EditBaseLinearShader(usage)
{
    private readonly ObservableCollection<ContentControl> _optionControls = [];

    public override ObservableCollection<ContentControl> OptionControls => _optionControls;

    public override ShaderModel? GetShader(out bool success) => 
        ShaderModel.FromUri(
            new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
            out success);
}