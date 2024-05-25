using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public sealed class GpuGrayscaleEditModel : IGPUEdit
    {
        private readonly ShaderModel _shader;
        private readonly VertexArrayModel _vao;

        public GpuGrayscaleEditModel(VertexArrayModel vertexArray)
        {
            vertexArray.Use();

            _shader = ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Grayscale.frag", UriKind.RelativeOrAbsolute),
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }

            _vao = vertexArray;
        }

        void IGPUEdit.Apply(TextureModel source, FrameBufferModel destination)
        {
            destination.Use(FramebufferTarget.Framebuffer);
            source.Use(TextureUnit.Texture0);
            _shader.Use();
            _vao.Use();
            GL.DrawElements(PrimitiveType.Triangles, _vao.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        void IDisposable.Dispose()
        {
            // static shader will live during the programs runtime.
        }
    }
}
