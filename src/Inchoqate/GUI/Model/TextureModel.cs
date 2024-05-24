﻿using System.IO;
using System.Windows.Media;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Inchoqate.GUI.Model
{
    public class TextureModel : IDisposable
    {
        private static readonly ILogger<TextureModel> _logger = FileLoggerFactory.CreateLogger<TextureModel>();

        public readonly int Handle;
        public readonly int Width, Height;

        // TODO: expose a setter
        public readonly Color BorderColor;


        public TextureModel(string path, Color borderColor = default)
        {
            Handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            StbImage.stbi_set_flip_vertically_on_load(1);

            using Stream stream = File.OpenRead(path);
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            Width = image.Width;
            Height = image.Height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            if (borderColor == default)
                borderColor = Color.FromRgb(255, 99, 71);

            float r = (float)borderColor.R / 255.0f;
            float g = (float)borderColor.G / 255.0f;
            float b = (float)borderColor.B / 255.0f;
            float a = (float)borderColor.A / 255.0f;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, [r, g, b, a]);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            _logger.LogInformation("Created texture from file: {path}", path);
        }


        public TextureModel(int width, int height)
        {
            Handle = GL.GenTexture();
            Width = width;
            Height = height;

            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            _logger.LogInformation("Created empty texture with dimensions: {width}x{height}", width, height);
        }


        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }


        #region Clean up

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteTexture(Handle);

                disposedValue = true;
            }
        }

        ~TextureModel()
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
