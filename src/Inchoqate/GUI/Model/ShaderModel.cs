using OpenTK.Graphics.OpenGL4;
using Microsoft.Extensions.Logging;
using System.IO;
using Inchoqate.Logging;
using System;
using System.Windows;

namespace Inchoqate.GUI.Model
{
    public class ShaderModel : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<ShaderModel>();

        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations = [];


        public delegate ShaderModel? ModelGen(out bool success);


        public static ShaderModel FromSource(string vertexSource, string fragmentSource, out bool success)
        {
            return new(vertexSource, fragmentSource, out success);
        }

        public static ShaderModel FromPath(string vertexPath, string fragmentPath, out bool success)
        {
            var vertexSource = File.ReadAllText(vertexPath);
            var fragmentSource = File.ReadAllText(fragmentPath);

            return ShaderModel.FromSource(vertexSource, fragmentSource, out success);
        }

        public static ShaderModel? FromUri(Uri vertexPath, Uri fragmentPath, out bool success)
        {
            // In the xaml designer, the URI cannot be resolved and throws.
            // TODO: find a way to check if the resource can be located and remove
            // the try-catch block.
            try
            {
                var vertexResource = Application.GetResourceStream(vertexPath);
                using var vertexReader = new StreamReader(vertexResource.Stream);
                var vertexSource = vertexReader.ReadToEnd();

                var fragmentResource = Application.GetResourceStream(fragmentPath);
                using var fragmentReader = new StreamReader(fragmentResource.Stream);
                var fragmentSource = fragmentReader.ReadToEnd();

                return ShaderModel.FromSource(vertexSource, fragmentSource, out success);
            }
            catch (IOException e)
            {
                _logger.LogError(e,
                    "Application resource could not be located from one or more the given URIs: " +
                    "{vert}, {frag}. This a know error in the xaml designer.",
                    vertexPath.OriginalString,
                    fragmentPath.OriginalString);
                success = false;
                return null;
            }
        }

        public ShaderModel(string vertexSource, string fragmentSource, out bool success)
        {
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vertexSource);

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentSource);

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

            // Initiate uniform dict
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            for (int i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }

            success = true;

        clean_up:
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }


        public void SetUniform<T>(string name, T value)
            where T : struct
        {
            Use();
            ((Action)(value switch
            {
                int     val => () => GL.Uniform1(_uniformLocations[name], val),
                uint    val => () => GL.Uniform1(_uniformLocations[name], val),
                float   val => () => GL.Uniform1(_uniformLocations[name], val),
                double  val => () => GL.Uniform1(_uniformLocations[name], val),
                _ => () => _logger.LogWarning("Tried to set invalid uniform type {t}", typeof(T))
            }))();
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
