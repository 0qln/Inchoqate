using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Inchoqate.GUI.ViewModel
{
    public class EditImplNoGreenViewModel : EditBaseLinearShader
    {
        private readonly ObservableCollection<Control> _optionControls = [];

        public override ObservableCollection<Control> OptionControls => _optionControls;


        public EditImplNoGreenViewModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) : base(usage)
        {
        }


        public override ShaderModel? GetShader(out bool success) => 
            ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/NoGreen.frag", UriKind.RelativeOrAbsolute),
                out success);
    }
}
