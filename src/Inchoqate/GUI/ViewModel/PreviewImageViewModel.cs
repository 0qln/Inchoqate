using MvvmHelpers;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows;
using OpenTK.Wpf;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using PointF = System.Drawing.PointF;

namespace Inchoqate.GUI.ViewModel;

public class PreviewImageViewModel : BaseViewModel, IDisposable
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageViewModel>();

    // Used for the final preview rendering.
    private readonly ShaderModel? _shader;
    private readonly VertexArrayModel _vertexArray;

    private float[] _vertices =
    [
        // Position             Texture coordinates
        1.0f,  1.0f, 0.0f,     1.0f, 1.0f, // top right
        1.0f, -1.0f, 0.0f,     1.0f, 0.0f, // bottom right
        -1.0f, -1.0f, 0.0f,     0.0f, 0.0f, // bottom left
        -1.0f,  1.0f, 0.0f,     0.0f, 1.0f  // top left
    ];

    private readonly uint[] _indices =
    [
        0, 1, 3,
        1, 2, 3
    ];

    // display properties
    private Size displaySize;
    private Size boundsSize;
    private SolidColorBrush voidColor = Brushes.Aquamarine;
    private RenderEditorViewModel? _editor; // will not be disposed

    /// <summary>
    /// The actual size in pixels of the image. Excluding the void.
    /// </summary>
    public Size DisplaySize
    {
        get => displaySize;
        set => SetProperty(ref displaySize, value);
    }

    /// <summary>
    /// The color of the void around the image.
    /// </summary>
    public SolidColorBrush VoidColor
    {
        get => voidColor;
        set => SetProperty(ref voidColor, value);
    }

    /// <summary>
    /// The actual size in pixels of the image. Including the void.
    /// </summary>
    public Size BoundsSize
    {
        get => boundsSize;
        set => SetProperty(ref boundsSize, value);
    }
        
    /// <summary>
    /// The editor applied on the image for the preview. Will not be disposed.
    /// </summary>
    public RenderEditorViewModel? RenderEditor
    {
        get => _editor;
        set
        {
            if (_editor == value) return;
                
            if (_editor is not null)
            {
                _editor.PropertyChanged -= Editor_PropertyChanged;
            }

            SetProperty(ref _editor, value);

            if (_editor is not null)
            {
                _editor.VoidColor = VoidColor.Color;
                _editor.RenderSize = BoundsSize;
                _editor.PropertyChanged += Editor_PropertyChanged;
            }
        }
    }

    private void Editor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case 
                nameof(_editor.Computed) or 
                nameof(_editor.RenderSize) or 
                nameof(_editor.SourceSize) or
                nameof(_editor.VoidColor):
                OnPropertyChanged(nameof(RenderEditor));
                break;
        }
    }


    // compute-state caching
    private float _panXDelta;
    private float _panYDelta;
    private float _panXStart;
    private float _panYStart;
    private float _zoomValue;

    // architecture constants
    private const float _zoomMin = 0.0f;
    private const float _zoomMax = 0.5f;

    // hyper parameters
    private float _panSensitivity = 1.0f;
    private float _zoomLimit = 0.97f;
    private float _zoomLevels = 20.0f;

    /// <summary>
    /// The sensitivity of the panning.
    /// </summary>
    public float PanSensitivity
    {
        get => _panSensitivity;
        set => SetProperty(ref _panSensitivity, value);
    }

    /// <summary>
    /// The number of zoom levels. Each zoom level is one mousewheel step.
    /// </summary>
    public float ZoomLevels
    {
        get => _zoomLevels;
        set => SetProperty(ref _zoomLevels, value);
    }

    /// <summary>
    /// Percentage (Element [0;1]).
    /// </summary>
    public float ZoomLimit
    {
        get => _zoomLimit;
        set
        {
            SetProperty(ref _zoomLimit, value);
            SetProperty(ref _zoomValue, Math.Min(_zoomValue, _zoomMax * value));
        }
    }

    /// <summary>
    /// Element [0;1].
    /// </summary>
    public float Zoom
    {
        get => _zoomValue * 2;
        set => SetProperty(ref _zoomValue, value / 2);
    }


    public PreviewImageViewModel()
    {
        _vertexArray = new(sIndx: _indices, sVert: _vertices, BufferUsageHint.StaticDraw);
        _vertexArray.Use();

        _shader = ShaderModel.FromUri(
            new("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
            new("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
            out bool success);

        if (!success)
        {
            // TODO: handle error
        }

        PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(Zoom) or nameof(ZoomLimit):
                    ReloadLayout();
                    break;

                case nameof(BoundsSize):
                    if (_editor is not null)
                        _editor.RenderSize = BoundsSize;
                    ReloadLayout();
                    break;

                case nameof(VoidColor):
                    if (_editor is not null)
                        _editor.VoidColor = VoidColor.Color;
                    break;

                case nameof(DisplaySize):
                    ReloadLayout();
                    break;

                case nameof(RenderEditor):
                    ReloadLayout();
                    break;
            }
        };
    }


    public void RenderToImage(GLWpfControl image)
    {
        if (_shader is null || _editor is null)
        {
            return;
        }

        if (_editor.Result is null)
        {
            var success = _editor.Compute();
            if (!success || _editor.Result is null)
            {
                return;
            }
        }
                 
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, image.Framebuffer);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _editor.Result.Data.Use(TextureUnit.Texture0);
        _shader.Use();
        _vertexArray.Use();
        GL.DrawElements(PrimitiveType.Triangles, _vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
    }


    public void MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        float zoomDelta = (float)e.Delta > 0 ? _zoomMax : -_zoomMax;
        Point relative = e.GetPosition((IInputElement)sender);
        float relativeXScaled = (float)(relative.X / BoundsSize.Width) * 2 - 1;
        float relativeYScaled = (float)(relative.Y / BoundsSize.Height) * 2 - 1;

        float newZoom = _zoomValue + (zoomDelta / _zoomLevels);
        newZoom = Math.Clamp(newZoom, _zoomMin, _zoomMax * _zoomLimit);

        _panXStart = (_panXStart + relativeXScaled * _zoomValue) - (relativeXScaled * newZoom);
        _panYStart = (_panYStart + relativeYScaled * _zoomValue) - (relativeYScaled * newZoom);
        _zoomValue = newZoom;
        OnPropertyChanged(nameof(Zoom));

        ReloadLayout();
    }

    public void DragDelta(object sender, DragDeltaEventArgs e)
    {
        var relativeXScaled = (float)(e.HorizontalChange / BoundsSize.Width);
        var relativeYScaled = (float)(e.VerticalChange / BoundsSize.Height);

        _panXDelta = relativeXScaled * (1 - _zoomValue * 2) * _panSensitivity;
        _panYDelta = relativeYScaled * (1 - _zoomValue * 2) * _panSensitivity;

        ReloadLayout();
    }

    public void DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _panXStart += _panXDelta;
        _panYStart += _panYDelta;
        _panXDelta = _panYDelta = 0;

        ReloadLayout();
    }

    public void DragStarted(object sender, DragStartedEventArgs e)
    {
    }

    public void ResetZoom()
    {
        _zoomValue = 0;
        _panXDelta = 0;
        _panYDelta = 0;
        _panXStart = 0;
        _panYStart = 0;

        ReloadLayout();
    }

    public struct RectCorners
    {
        public PointF TopLeft;
        public PointF TopRight;
        public PointF BottomLeft;
        public PointF BottomRight;

        public readonly override string ToString()
        {
            return $"TL: {TopLeft}, TR: {TopRight}, BL: {BottomLeft}, BR: {BottomRight}";
        }
    }

    private RectCorners _actualLayout;
    public RectCorners ActualLayout 
    {
        get => _actualLayout;
        set => SetProperty(ref _actualLayout, value);
    }

    private PointF _norm;
    public PointF Norm
    {
        get => _norm;
        private set => SetProperty(ref _norm, value);
    }

    public void ReloadLayout()
    {
        if (RenderEditor is null)
        {
            return;
        }

        float
            wNorm = (float)(BoundsSize.Width / DisplaySize.Width),
            hNorm = (float)(BoundsSize.Height / DisplaySize.Height);

        Norm = new(wNorm, hNorm);

        float panX = _panXDelta + _panXStart;
        float panY = _panYDelta + _panYStart;

        float left      = 0 + _zoomValue - panX;
        float right     = 1 - _zoomValue - panX;
        float top       = 1 - _zoomValue + panY;
        float bottom    = 0 + _zoomValue + panY;

        // align top
        float
            xOff = 0,
            yOff = (float)(DisplaySize.Height - BoundsSize.Height) / (float)DisplaySize.Height;

        var layout = new RectCorners
        {
            TopRight    = new(xOff + wNorm * right, yOff + hNorm * top),
            BottomRight = new(xOff + wNorm * right, yOff + hNorm * bottom),
            BottomLeft  = new(xOff + wNorm * left,  yOff + hNorm * bottom),
            TopLeft     = new(xOff + wNorm * left,  yOff + hNorm * top),
        };

        _vertices =
        [
            // Position     Texture coordinates
            1,  1, 0,      layout.TopRight.X,      layout.TopRight.Y,   // top right
            1, -1, 0,      layout.BottomRight.X,   layout.BottomRight.Y,// bottom right
            -1, -1, 0,      layout.BottomLeft.X,    layout.BottomLeft.Y, // bottom left
            -1,  1, 0,      layout.TopLeft.X,       layout.TopLeft.Y,    // top left
        ];

        _vertexArray.UpdateVertices(_vertices);

        ActualLayout = layout;
    }

    #region Clean up

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            _vertexArray.Dispose();
            _shader?.Dispose();

            disposedValue = true;
        }
    }

    ~PreviewImageViewModel()
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