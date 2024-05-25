using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class GpuIdentityEditModel : IGPUEdit
    {
        private readonly static ShaderModel _shader;

        static GpuIdentityEditModel()
        {
            _shader = ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }
        }

        void IGPUEdit.Apply(TextureModel source, FrameBufferModel destination, VertexArrayModel vertexArray)
        {
            destination.Use(FramebufferTarget.Framebuffer);
            source.Use(TextureUnit.Texture0);
            _shader.Use();
            vertexArray.Use();
            GL.DrawElements(PrimitiveType.Triangles, vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        void IDisposable.Dispose()
        {
            // static shader will live during the programs runtime.
        }
    }
}
