using OpenTK.Graphics.OpenGL4;
using Microsoft.Extensions.Logging;
using System.IO;
using Inchoqate.Logging;
using System.Windows;
using OpenTK.Mathematics;

namespace Inchoqate.GUI.Model;

public class ShaderModel : IDisposable
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<ShaderModel>();

    public readonly int Handle;

    private readonly Dictionary<string, int> _uniformLocations = [];


    public static ShaderModel FromSource(string vertexSource, string fragmentSource, out bool success)
    {
        return new(vertexSource, fragmentSource, out success);
    }

    public static ShaderModel FromPath(string vertexPath, string fragmentPath, out bool success)
    {
        var vertexSource = File.ReadAllText(vertexPath);
        var fragmentSource = File.ReadAllText(fragmentPath);

        return FromSource(vertexSource, fragmentSource, out success);
    }

    public static ShaderModel? FromUri(Uri vertexPath, Uri fragmentPath, out bool success)
    {
        // In the xaml designer, the URI cannot be resolved and throws.
        try
        {
            var vertexResource = Application.GetResourceStream(vertexPath);
            if (vertexResource is null) throw new IOException(
                $"The resource at {vertexPath.OriginalString} could not be found.");
            using var vertexReader = new StreamReader(vertexResource.Stream);
            var vertexSource = vertexReader.ReadToEnd();

            var fragmentResource = Application.GetResourceStream(fragmentPath);
            if (fragmentResource is null) throw new IOException(
                $"The resource at {vertexPath.OriginalString} could not be found.");
            using var fragmentReader = new StreamReader(fragmentResource.Stream);
            var fragmentSource = fragmentReader.ReadToEnd();

            return FromSource(vertexSource, fragmentSource, out success);
        }
        catch (IOException e)
        {
            Logger.LogError(e,
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
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);

        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var successVertexShader);
        if (successVertexShader == 0)
        {
            Logger.LogError(
                "OpenGL error while generating shader: Code:{error} | Info:{info}",
                GL.GetError(),
                GL.GetShaderInfoLog(vertexShader));
            success = false;
            goto clean_up;
        }

        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var successFragmentShader);
        if (successFragmentShader == 0)
        {
            Logger.LogError(
                "OpenGL error while generating shader: Code:{error} | Info:{info}",
                GL.GetError(),
                GL.GetShaderInfoLog(successFragmentShader));
            success = false;
            goto clean_up;
        }

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var successProgram);
        if (successProgram == 0)
        {
            Logger.LogError(
                "OpenGL error while generating shader: Code:{error} | Info:{info}",
                GL.GetError(),
                GL.GetShaderInfoLog(Handle));
            success = false;
            goto clean_up;
        }

        // TODO: handle the case where the shader does
        // not have the required attributes.
        Use();

        var aPositionLoc = GL.GetAttribLocation(Handle, "aPosition");
        GL.EnableVertexAttribArray(aPositionLoc);
        GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var aTexCoordLoc = GL.GetAttribLocation(Handle, "aTexCoord");
        GL.EnableVertexAttribArray(aTexCoordLoc);
        GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        // Initiate uniform dict
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            var location = GL.GetUniformLocation(Handle, key);
            _uniformLocations.Add(key, location);
        }

        success = true;

        clean_up:
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }


    public bool SetUniform<T>(string name, T value)
        where T : struct
    {
        if (!_uniformLocations.TryGetValue(name, out var index))
        {
            Logger.LogWarning("Tried to set non existent uniform attribute \"{name}\"", name);
            return false;
        }

        Use();

        ((Action)(value switch
        {
            int     val => () => GL.Uniform1(index, val),
            uint    val => () => GL.Uniform1(index, val),
            float   val => () => GL.Uniform1(index, val),
            double  val => () => GL.Uniform1(index, val),
            Vector3 val => () => GL.Uniform3(index, val),
            _ => () => Logger.LogWarning("Tried to set invalid uniform type {t}", typeof(T))
        }))();

        return true;
    }

        
    public void Use()
    {
        GL.UseProgram(Handle);
    }


    #region Clean up

    private bool _disposedValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing">
    /// Whether this object is being disposed right now or not. 
    /// If that is not the case the only other option should be that the GC is 
    /// finalizing the object right now, in which case, it is only safe to 
    /// dispose of unmanaged resources, because the managed ones have already 
    /// been taken care of by the GC.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteProgram(Handle);

            _disposedValue = true;
        }
    }

    ~ShaderModel()
    {
        // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
        // The OpenGL resources have to be released from a thread with an active OpenGL Context.
        // The GC runs on a separate thread, thus releasing unmanaged GL resources inside the finalizer
        // is not possible.
        if (_disposedValue == false)
        {
            Logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}