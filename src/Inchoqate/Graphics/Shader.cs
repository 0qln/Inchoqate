using System.IO;
using System.Windows;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Inchoqate.Graphics;

public class Shader : IDisposable
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<Shader>();

    public readonly int Handle;

    private readonly Dictionary<string, int> _uniformLocations = [];


    public static Shader FromSource(string vertexSource, string fragmentSource, out bool success)
    {
        return new(vertexSource, fragmentSource, out success);
    }

    public static Shader FromPath(string vertexPath, string fragmentPath, out bool success)
    {
        var vertexSource = File.ReadAllText(vertexPath);
        var fragmentSource = File.ReadAllText(fragmentPath);

        return FromSource(vertexSource, fragmentSource, out success);
    }

    private static string GetSource(Uri uri)
    {
        var resource = Application.GetResourceStream(uri);
        if (resource is null)
            throw new IOException($"The resource at {resource} could not be found.");
        using var reader = new StreamReader(resource.Stream);
        return reader.ReadToEnd();
    }

    public static Shader? FromUri(Uri vertex, Uri fragment, out bool success)
    {
        // In the xaml designer, the URI cannot be resolved and throws.
        try
        {
            return FromSource(GetSource(vertex), GetSource(fragment), out success);
        }
        catch (IOException e)
        {
            Logger.LogError(e,
                "Application resource could not be located from one or more the given URIs: " +
                "{vert}, {frag}. This a know error in the xaml designer.",
                vertex.OriginalString,
                fragment.OriginalString);
            success = false;
            return null;
        }
    }

    private sealed class ShaderPart : IDisposable
    {
        private readonly int _programHandle;

        public int Handle { get; }


        public ShaderPart(string source, ShaderType type, int programHandle, out bool success)
        {
            _programHandle = programHandle;
            Handle = GL.CreateShader(type);

            GL.ShaderSource(Handle, source);
            GL.CompileShader(Handle);
            GL.GetShader(Handle, ShaderParameter.CompileStatus, out var compileSuccess);

            if (compileSuccess == 0 || Logger.CheckErrors())
            {
                Logger.LogError(
                    "OpenGL error while generating shader: Code:{error} | Info:{info}",
                    GL.GetError(),
                    GL.GetShaderInfoLog(Handle));
                success = false;
                return;
            }

            success = true;
        }

        private bool _disposed;
    
        // ReSharper disable once UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                GL.DetachShader(_programHandle, Handle);
                GL.DeleteShader(Handle);
                _disposed = !Logger.CheckErrors("Failed to delete shader part.");
            }
        } 

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~ShaderPart()
        {
            if (_disposed == false)
            {
                Logger.LogWarning("GPU Resource leak!");
            }
        }
    }

    public Shader(string vertexSource, string fragmentSource, out bool success)
    {
        Handle = GL.CreateProgram();

        using var vertex = new ShaderPart(vertexSource, ShaderType.VertexShader, Handle, out success);
        if (!success)
        {
            Logger.LogError("Failed to create vertex shader");
            return;
        }

        using var fragment = new ShaderPart(fragmentSource, ShaderType.FragmentShader, Handle, out success);
        if (!success)
        {
            Logger.LogError("Failed to create fragment shader");
            return;
        }

        GL.AttachShader(Handle, vertex.Handle);
        GL.AttachShader(Handle, fragment.Handle);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var successProgram);
        if (successProgram == 0 || Logger.CheckErrors())
        {
            Logger.LogError(
                "OpenGL error while generating shader: Code:{error} | Info:{info}",
                GL.GetError(),
                GL.GetShaderInfoLog(Handle));
            success = false;
            return;
        }

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

        success = !Logger.CheckErrors("Failed to setup up a shader");
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

        if (Logger.CheckErrors())
            Logger.LogError("Failed to set uniform: [{n}, {v}]", name, value);

        return true;
    }

        
    public void Use()
    {
        GL.UseProgram(Handle);

        Logger.CheckErrors("Failed to use shader");
    }


    #region Clean up

    private bool _disposed;

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
        if (!_disposed)
        {
            GL.DeleteProgram(Handle);
            _disposed = !Logger.CheckErrors("Failed to delete shader");
        }
    }

    ~Shader()
    {
        // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
        // The OpenGL resources have to be released from a thread with an active OpenGL Context.
        // The GC runs on a separate thread, thus releasing unmanaged GL resources inside the finalizer
        // is not possible.
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