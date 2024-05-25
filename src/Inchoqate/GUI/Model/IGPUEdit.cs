using Inchoqate.GUI.Model;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public interface IGPUEdit : IDisposable
    {
        void Apply(TextureModel source, FrameBufferModel destination);
    }
}
