using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class GpuNoGreenEditModel(BufferUsageHint usage = BufferUsageHint.StaticDraw) 
        : ShaderEditModel(usage)
    {
        public override ShaderModel? GetShader(out bool success) => 
            ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/NoGreen.frag", UriKind.RelativeOrAbsolute),
                out success);
    }
}

