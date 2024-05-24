﻿using OpenTK.Graphics.OpenGL4;
using Microsoft.Extensions.Logging;
using System.IO;
using Inchoqate.Logging;

namespace Inchoqate.GUI.Model
{
    public class ShaderModel : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<ShaderModel>();

        public readonly int Handle;


        public ShaderModel(Uri vertexPath, Uri fragmentPath, out bool success)
            : this(vertexPath.LocalPath, fragmentPath.LocalPath, out success)
        {
        } 

        public ShaderModel(string vertexPath, string fragmentPath, out bool success)
        {
            string VertexShaderSource = File.ReadAllText(vertexPath);
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            string FragmentShaderSource = File.ReadAllText(fragmentPath);
            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int successVertexShader);
            if (successVertexShader == 0)
            {
                _logger.LogError(
                    "OpenGL error while generating shader: Code:{error} | Info:{info}",
                    GL.GetError(),
                    GL.GetShaderInfoLog(VertexShader));
                success = false;
                goto clean_up;
            }

            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int successFragmentShader);
            if (successFragmentShader == 0)
            {
                _logger.LogError(
                    "OpenGL error while generating shader: Code:{error} | Info:{info}",
                    GL.GetError(),
                    GL.GetShaderInfoLog(successFragmentShader));
                success = false;
                goto clean_up;
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int successProgram);
            if (successProgram == 0)
            {
                _logger.LogError(
                    "OpenGL error while generating shader: Code:{error} | Info:{info}",
                    GL.GetError(),
                    GL.GetShaderInfoLog(Handle));
                success = false;
                goto clean_up;
            }

            // TODO: handle the case where the shader does
            // not have the required attributes.
            Use();

            int aPositionLoc = GL.GetAttribLocation(Handle, "aPosition");
            GL.EnableVertexAttribArray(aPositionLoc);
            GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int aTexCoordLoc = GL.GetAttribLocation(Handle, "aTexCoord");
            GL.EnableVertexAttribArray(aTexCoordLoc);
            GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            success = true;

        clean_up:
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        
        public void Use()
        {
            GL.UseProgram(Handle);
        }


        #region Clean up

        private bool disposedValue = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing">
        /// Wether this opject is being disposed right now or not. 
        /// If that is not the case the only other option should be that the GC is 
        /// finalizing the object right now, in which case, it is only safe to 
        /// dispose of unmanaged resources, because the managed ones have already 
        /// been taken care of by the GC.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~ShaderModel()
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