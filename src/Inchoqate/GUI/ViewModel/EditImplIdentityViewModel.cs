using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplIdentityViewModel : EditBaseLinearShader
    {
        private readonly ObservableCollection<ContentControl> _optionControls = [];

        public override ObservableCollection<ContentControl> OptionControls => _optionControls;


        public EditImplIdentityViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(usage)
        {
        }


        public override ShaderModel? GetShader(out bool success) => 
            ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
                out success);
    }
}
