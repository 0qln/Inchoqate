using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public sealed class GpuIdentityEditModel : IGPUEdit
    {
        private readonly ShaderModel _shader;
        private readonly VertexArrayModel _vao;

        private static readonly float[] _vertices =
        [
            // Position             Texture coordinates
             1.0f,  1.0f, 0.0f,     1.0f, 1.0f, // top right
             1.0f, -1.0f, 0.0f,     1.0f, 0.0f, // bottom right
            -1.0f, -1.0f, 0.0f,     0.0f, 0.0f, // bottom left
            -1.0f,  1.0f, 0.0f,     0.0f, 1.0f  // top left
        ];

        private static readonly uint[] _indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        public GpuIdentityEditModel(BufferUsageHint usage = BufferUsageHint.StaticDraw)
        {
            _vao = new VertexArrayModel(_indices, _vertices, usage);
            _vao.Use();

            _shader = ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }
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
            // TODO
        }
    }
}
