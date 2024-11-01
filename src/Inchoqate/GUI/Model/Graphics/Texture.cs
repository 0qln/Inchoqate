using System.IO;
using System.Net.Http;
using System.Windows.Media;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Inchoqate.GUI.Model.Graphics;

public class Texture : IDisposable, IEditSource
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<Texture>();

    public readonly int Handle;

    public int Width { get; private init; }
    public int Height { get; private init; }
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

    public Uri? Source { get; private set; }

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
            var r = value.R / 255.0f;
            var g = value.G / 255.0f;
            var b = value.B / 255.0f;
            var a = value.A / 255.0f;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, [r, g, b, a]);

            if (Logger.CheckErrors())
            {
                Logger.LogError("Failed to set texture parameter in {propertyName}", nameof(BorderColor));
                return;
            }

            _borderColor = value;
        }
    }

    public TextureUnit Unit;


    private Texture(TextureUnit unit = TextureUnit.Texture0)
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

        Logger.CheckErrors("Failed to initiate default texture parameter");

        BorderColor = Color.FromRgb(255, 99, 71);
    }

    public static Texture? FromUri(Uri uri, TextureUnit unit = TextureUnit.Texture0)
    {
        Texture result;

         // todo: does this work?
        if (uri.IsFile)
        {
            var path = uri.LocalPath;
            if (!File.Exists(path))
            {
                Logger.LogError("File not found: {Path}", path);
                return null;
            }

            try
            {
                using Stream stream = File.OpenRead(path);
                result = FromStream(stream, unit);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while reading image data: {Path}", path);
                return null;
            }
        }
        else
        {
            try
            {
                using var client = new HttpClient();
                var response = client.GetAsync(uri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError("Failed to download image: {Uri}", uri);
                    return null;
                }

                using var stream = response.Content.ReadAsStream();
                result = FromStream(stream, unit);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to read image: {Uri}", uri);
                return null;
            }
        }

        result.Source = uri;
        return result;
    }

    public static Texture FromStream(Stream stream, TextureUnit unit = TextureUnit.Texture0)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        var image = ImageResult.FromStream(stream, PixelComponents);
        return FromData(image.Width, image.Height, image.Data, unit);
    }

    public static Texture FromData(int width, int height, byte[]? data = null, TextureUnit unit = TextureUnit.Texture0)
    {
        var result = new Texture(unit)
        {
            Width = width,
            Height = height
        };
        result.LoadData(width, height, data);
        return result;
    }

    public void LoadData(int width, int height, byte[]? data = null)
    {
        Use();
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, GLPixelFormat, GLPixelType, data);
        InitDefaults();
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        Logger.CheckErrors("Failed to load data into texture");
    }


    public void Use()
    {
        Use(Unit);
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        Logger.CheckErrors("Failed to use texture");
    }


    #region Clean up

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            GL.DeleteTexture(Handle);
            _disposed = Logger.CheckErrors("Failed to delete texture");
        }
    }

    ~Texture()
    {
        if (_disposed == false)
        {
            Logger.LogWarning("GPU Resource leak!");
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}