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
    internal class Texture
    {
        public int Handle { get; set; } = GL.GenTexture();


        public Texture(string sourceFile)
        {
            Use();
            
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(sourceFile), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.ProxyTexture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }


        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
