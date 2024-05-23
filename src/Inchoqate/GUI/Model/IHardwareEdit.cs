using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public interface IHardwareEdit
    {
        void Apply(
            TextureModel source,
            FrameBufferModel destination,
            VertexArrayModel vertexArray);
    }
}
