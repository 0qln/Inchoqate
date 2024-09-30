using System.IO;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using StbImageWriteSharp;

namespace Inchoqate.Graphics;

public class PixelBuffer(int size, int width, int height) : IDisposable, IEditSource, IEditDestination
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<PixelBuffer>();

    private bool _disposedValue;

    public byte[] Data { get; private set; } = new byte[size];
    public readonly int Width = width, Height = height;


    public static PixelBuffer FromGpu(Texture buffer)
    {
        PixelBuffer result = new(buffer.Stride * buffer.Height, buffer.Width, buffer.Height);
        result.LoadData(buffer);
        return result;
    }

    public void LoadData(Texture buffer)
    {
        buffer.Use();
        GL.GetnTexImage(TextureTarget.Texture2D, 0, Texture.GLPixelFormat, Texture.GLPixelType, Data.Length, Data);

        Logger.CheckErrors("Failed to load data from texture");
    }

    public void SaveToFile(string path)
    {
        StbImageWrite.stbi_flip_vertically_on_write(1);
        using Stream stream = File.OpenWrite(path);
        ImageWriter writer = new();
        writer.WritePng(Data, Width, Height, ColorComponents.RedGreenBlueAlpha, stream);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Data = null!;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}