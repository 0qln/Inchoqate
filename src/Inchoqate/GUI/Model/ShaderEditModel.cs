using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Model
{
    public abstract class ShaderEditModel : IGPUEdit, IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<ShaderEditModel>();  

        protected static readonly ReadOnlyMemory<float> _vertices = (float[])
        [
            // Position             Texture coordinates
             1.0f,  1.0f, 0.0f,     1.0f, 1.0f, // top right
             1.0f, -1.0f, 0.0f,     1.0f, 0.0f, // bottom right
            -1.0f, -1.0f, 0.0f,     0.0f, 0.0f, // bottom left
            -1.0f,  1.0f, 0.0f,     0.0f, 1.0f  // top left
        ];

        protected static readonly ReadOnlyMemory<uint> _indices = (uint[])
        [
            0, 1, 3,
            1, 2, 3
        ];

        protected readonly ShaderModel? _shader;
        protected readonly VertexArrayModel _vao;


        public ShaderEditModel(BufferUsageHint usage)
        {
            _vao = new VertexArrayModel(_indices, _vertices, usage);
            _vao.Use();

            _shader = GetShader(out bool success);
            if (!success)
            {
                _logger.LogWarning("ShaderEditModel deriviant failed to generate the shader.");
            }
        }


        /// <summary>
        /// Generate the shader for this shader edit model.
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public abstract ShaderModel? GetShader(out bool success); 


        public bool Apply(TextureModel source, FrameBufferModel destination)
        {
            if (_shader is null)
            {
                return false;
            }

            destination.Use(FramebufferTarget.Framebuffer);
            source.Use(TextureUnit.Texture0);
            _shader.Use();
            _vao.Use();
            GL.DrawElements(PrimitiveType.Triangles, _vao.IndexCount, DrawElementsType.UnsignedInt, 0);
            return true;
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _shader?.Dispose();
                _vao.Dispose();

                disposedValue = true;
            }
        }

        ~ShaderEditModel()
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
