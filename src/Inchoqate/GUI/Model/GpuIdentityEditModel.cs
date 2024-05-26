using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Reflection.Metadata;

namespace Inchoqate.GUI.Model
{
    public class GpuIdentityEditModel : IGPUEdit
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<GpuIdentityEditModel>();

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


        private readonly ShaderModel _shader;
        private readonly VertexArrayModel _vao;


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


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _shader.Dispose();
                _vao.Dispose();

                disposedValue = true;
            }
        }

        ~GpuIdentityEditModel()
        {
            // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
            // The OpenGL resources have to be released from a thread with an active OpenGL Context.
            // The GC runs on a seperate thread, thus releasing unmanaged GL resources inside the finalizer
            // is not possible.
            if (disposedValue == false)
            {
                _logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
