using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.Logging;
using PointF = System.Drawing.PointF;

namespace Inchoqate.GUI.ViewModel;

public class PreviewImageViewModel : BaseViewModel, IDisposable
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<PreviewImageViewModel>();

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
    private Size _displaySize;
    private Size _boundsSize;
    private SolidColorBrush _voidColor = Brushes.Aquamarine;
    private RenderEditorViewModel? _editor; // will not be disposed

    /// <summary>
    /// The actual size in pixels of the image. Excluding the void.
    /// </summary>
    public Size DisplaySize
    {
        get => _displaySize;
        set => SetProperty(ref _displaySize, value);
    }

    /// <summary>
    /// The color of the void around the image.
    /// </summary>
    public SolidColorBrush VoidColor
    {
        get => _voidColor;
        set => SetProperty(ref _voidColor, value);
    }

    /// <summary>
    /// The actual size in pixels of the image. Including the void.
    /// </summary>
    public Size BoundsSize
    {
        get => _boundsSize;
        set => SetProperty(ref _boundsSize, value);
    }

    /// <summary>
    /// The shader used for the preview. Will be disposed.
    /// </summary>
    public Shader Shader { get; }

    /// <summary>
    /// The vertex array used for the preview. Will be disposed.
    /// </summary>
    public VertexArray VertexArray { get; }

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
                _editor.RenderSize = _editor.SourceSize;
                _editor.PropertyChanged += Editor_PropertyChanged;
            }
        }
    }

    private void Editor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_editor.SourceSize):
                if (_editor is not null) _editor.RenderSize = _editor.SourceSize;
                break;
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
    private const float ZoomMinInner = 0.0f;
    private const float ZoomMaxInner = 0.5f;

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
            SetProperty(ref _zoomValue, Math.Min(_zoomValue, ZoomMaxInner * value));
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
        VertexArray = new(sIndx: _indices, sVert: _vertices, BufferUsageHint.StaticDraw);
        VertexArray.Use();

        Shader = Shader.FromSource(Shaders.BaseVert, Shaders.BaseFrag, out var success);

        if (!success)
        {
            Logger.LogError("Failed to create preview image shader.");
        }

        PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(RenderEditor):
                case nameof(DisplaySize):
                case nameof(BoundsSize):
                case nameof(ZoomLimit):
                case nameof(Zoom):
                    ReloadLayout();
                    break;
                case nameof(VoidColor):
                    if (_editor is not null) _editor.VoidColor = VoidColor.Color;
                    break;
            }
        };
    }

    public void MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        var zoomDelta = (float)e.Delta > 0 ? ZoomMaxInner : -ZoomMaxInner;
        var relative = e.GetPosition((IInputElement)sender);
        var relativeXScaled = (float)(relative.X / BoundsSize.Width) * 2 - 1;
        var relativeYScaled = (float)(relative.Y / BoundsSize.Height) * 2 - 1;

        var newZoom = _zoomValue + (zoomDelta / _zoomLevels);
        newZoom = Math.Clamp(newZoom, ZoomMinInner, ZoomMaxInner * _zoomLimit);

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

        var panX = _panXDelta + _panXStart;
        var panY = _panYDelta + _panYStart;

        var left      = 0 + _zoomValue - panX;
        var right     = 1 - _zoomValue - panX;
        var top       = 1 - _zoomValue + panY;
        var bottom    = 0 + _zoomValue + panY;

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

        VertexArray.UpdateVertices(_vertices);

        ActualLayout = layout;
    }

    #region Clean up

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            VertexArray.Dispose();
            Shader.Dispose();

            _disposedValue = true;
        }
    }

    ~PreviewImageViewModel()
    {
        // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
        // The OpenGL resources have to be released from a thread with an active OpenGL Context.
        // The GC runs on a seperate thread, thus releasing unmanaged GL resources inside the finalizer
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