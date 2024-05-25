﻿using Microsoft.Extensions.Logging;
using Inchoqate.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class VertexArrayModel : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<VertexArrayModel>();

        public readonly int Handle;
        public readonly int IndexCount;

        private readonly BufferModel<uint> _elementBufferObject;
        private readonly BufferModel<float> _vertexBufferObject;


        public VertexArrayModel(uint[] indices, float[] vertices, BufferUsageHint usage)
        {
            Handle = GL.GenVertexArray();
            this.Use();

            _vertexBufferObject = new BufferModel<float>(BufferTarget.ArrayBuffer, vertices, usage);
            _vertexBufferObject.Use();

            _elementBufferObject = new BufferModel<uint>(BufferTarget.ElementArrayBuffer, indices, usage);
            _elementBufferObject.Use();

            IndexCount = indices.Length;
        }


        public void UpdateVertices(float[] vertices)
        {
            _vertexBufferObject.Update(vertices);
        }

        public void UpdateIndices(uint[] indices)
        {
            _elementBufferObject.Update(indices);
        }


        public void Use()
        {
            GL.BindVertexArray(Handle);
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteVertexArray(Handle);
                _elementBufferObject.Dispose();
                _vertexBufferObject.Dispose();

                disposedValue = true;
            }
        }

        ~VertexArrayModel()
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
