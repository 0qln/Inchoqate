using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.GUI.Model;
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

    public override Shader? GetShader(out bool success)
    {
        return Shader.FromUri(
            new("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new("/Shaders/NoGreen.frag", UriKind.RelativeOrAbsolute),
            out success);
    }
}