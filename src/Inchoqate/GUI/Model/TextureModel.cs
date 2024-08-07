﻿using System.IO;
using System.Windows.Media;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Inchoqate.GUI.Model;

public class TextureModel : IDisposable, IEditSourceModel
{
    private static readonly ILogger<TextureModel> _logger = FileLoggerFactory.CreateLogger<TextureModel>();

    public readonly int Handle;

    public int Width { get; private set; }
    public int Height { get; private set; }
    /// <summary>The bytes per row of the texture.</summary>
    public int Stride => Width * PixelDepth;
    /// <summary>Pixel depth in bytes.</summary>
    public const int PixelDepth = 4;
    /// <summary>The components for each pixel.</summary>
    public const ColorComponents PixelComponents = ColorComponents.RedGreenBlueAlpha;
    /// <summary>The pixel format.</summary>
    public const PixelFormat GLPixelFormat = PixelFormat.Rgba;
    /// <summary>The pixel type.</summary>
    public const PixelType GLPixelType = PixelType.UnsignedByte;

    private Color _borderColor;

    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            if (_borderColor == value)
            {
                return;
            }

            Use();
            float r = (float)value.R / 255.0f;
            float g = (float)value.G / 255.0f;
            float b = (float)value.B / 255.0f;
            float a = (float)value.A / 255.0f;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, [r, g, b, a]);

            _borderColor = value;
        }
    }

    public TextureUnit Unit;


    private TextureModel(TextureUnit unit = TextureUnit.Texture0)
    {
        Handle = GL.GenTexture();
        Unit = unit;
        Use();
    }

    private void InitDefaults()
    {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        BorderColor = Color.FromRgb(255, 99, 71);
    }

    public static TextureModel FromFile(string path, TextureUnit unit = TextureUnit.Texture0)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        using Stream stream = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(stream, PixelComponents);
        return FromData(image.Width, image.Height, image.Data, unit);
    }

    public static TextureModel FromData(int width, int height, byte[]? data = null, TextureUnit unit = TextureUnit.Texture0)
    {
        var result = new TextureModel(unit)
        {
            Width = width,
            Height = height
        };
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, GLPixelFormat, GLPixelType, data);
        result.InitDefaults();
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        return result;
    }


    public void Use()
    {
        Use(Unit);
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