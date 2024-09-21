using OpenTK.Graphics.OpenGL4;
using StbImageWriteSharp;
using System.IO;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.Model;

public class PixelBufferModel(int size, int width, int height) : IDisposable, IEditSourceModel, IEditDestinationModel
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<PixelBufferModel>();

    private bool _disposedValue;

    public byte[] Data { get; private set; } = new byte[size];
    public readonly int Width = width, Height = height;


    public static PixelBufferModel FromGpu(TextureModel buffer)
    {
        PixelBufferModel result = new(buffer.Stride * buffer.Height, buffer.Width, buffer.Height);
        result.LoadData(buffer);
        return result;
    }

    public void LoadData(TextureModel buffer)
    {
        buffer.Use();
        GL.GetnTexImage(TextureTarget.Texture2D, 0, TextureModel.GLPixelFormat, TextureModel.GLPixelType, Data.Length, Data);

        if (!GraphicsModel.CheckErrors())
            Logger.LogError("Failed to load data from texture");
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