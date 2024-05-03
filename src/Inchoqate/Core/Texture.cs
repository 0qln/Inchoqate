using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Core
{
    internal class Texture /*: IDisposable*/
    {
        public readonly int Handle;


        public Texture(string path)
        {
            Handle = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            StbImage.stbi_set_flip_vertically_on_load(1);
            
            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

        }


        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }


        //#region Clean up

        //private bool disposedValue = false;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        GL.DeleteTexture(Handle);

        //        disposedValue = true;
        //    }
        //}

        //~Texture()
        //{
        //    // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
        //    // The OpenGL resources have to be released from a thread with an active OpenGL Context.
        //    // The GC runs on a seperate thread, thus releasing unmanaged GL resources inside the finalizer
        //    // is not possible.
        //    if (disposedValue == false)
        //    {
        //        // TODO: Use logging
        //        Console.WriteLine("[WARN] GPU Resource leak! Did you forget to call Dispose()?");
        //    }
        //}

        //public void Dispose()
        //{
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}

        //#endregion
    }
}
