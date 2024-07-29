using OpenTK.Graphics.OpenGL4;
using StbImageWriteSharp;
using System.IO;

namespace Inchoqate.GUI.Model;

public class PixelBufferModel(int size, int width, int height) : IDisposable, IEditSourceModel, IEditDestinationModel
{
    private bool disposedValue;

    public byte[] Data { get; private set; } = new byte[size];
    public readonly int Width = width, Height = height;

    public static PixelBufferModel FromGpu(FrameBufferModel buffer)
    {
        PixelBufferModel result = new(buffer.Data.Stride * buffer.Data.Height, buffer.Data.Width, buffer.Data.Height);
        buffer.Use(FramebufferTarget.Framebuffer);
        GL.ReadPixels(0, 0, buffer.Data.Width, buffer.Data.Height, TextureModel.GLPixelFormat, TextureModel.GLPixelType, result.Data);
        return result;
    }

    public void SaveToFile(string path)
    {
        StbImageWrite.stbi_flip_vertically_on_write(1);
        using Stream stream = File.OpenWrite(path);
        ImageWriter writer = new();
        writer.WritePng(Data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Data = null!;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}