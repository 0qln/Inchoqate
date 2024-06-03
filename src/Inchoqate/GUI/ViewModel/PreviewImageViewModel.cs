﻿using MvvmHelpers;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows;
using OpenTK.Wpf;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Specialized;
using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class PreviewImageViewModel : BaseViewModel, IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageViewModel>();

        private readonly ShaderModel? _shader;
        private readonly VertexArrayModel _vertexArray;

        // will not be disposed
        private IEditorModel<TextureModel, FrameBufferModel>? _editor; 
        public IEditorModel<TextureModel, FrameBufferModel>? RenderEditor
        {
            get => _editor;
            set
            {
                if (_editor == value)
                {
                    return;
                }
                if (_editor is not null)
                    _editor.EditsChanged -= Editor_EditsChanged;
                SetProperty(ref _editor, value);
                if (imageSource is not null)
                    _editor!.SetSource(TextureModel.FromFile(imageSource));
                _editor!.RenderSize = SourceSize;
                _editor!.VoidColor = VoidColor.Color;
                _editor!.EditsChanged += Editor_EditsChanged;
                ReloadLayout();
            }
        }

        private void Editor_EditsChanged(object? sender, EventArgs e)
        {
            EditorChanged?.Invoke(sender, e);
        }

        public event EventHandler? EditorChanged;

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

        private string? imageSource;
        private Size renderSize;
        private Size sourceSize;
        private Size boundsSize;
        private SolidColorBrush voidColor = Brushes.Aquamarine;

        public string? ImageSource
        {
            get => imageSource;
            set
            {
                if (value is null) return;

                SetProperty(ref imageSource, value);
                TextureModel texture = TextureModel.FromFile(imageSource);
                _editor?.SetSource(texture);
                SourceSize = new(texture.Width, texture.Height);
                ReloadLayout();
            }
        }

        public Size SourceSize
        {
            get => sourceSize;
            private set
            {
                SetProperty(ref sourceSize, value);
                if (_editor is not null)
                    _editor.RenderSize = value;
            }
        }

        public Size RenderSize
        {
            get => renderSize;
            set
            {
                if (renderSize == value) return;
                SetProperty(ref renderSize, value);
                ReloadLayout();
            }
        }

        public SolidColorBrush VoidColor
        {
            get => voidColor;
            set
            {
                if (_editor is not null)
                    _editor.VoidColor = value.Color;
                SetProperty(ref voidColor, value);
            }
        }

        public Size BoundsSize
        {
            get => boundsSize;
            set
            {
                if (boundsSize == value) return;
                SetProperty(ref boundsSize, value);
                ReloadLayout();
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
        private float panSensitivity = 1.0f;
        private float zoomLimit = 0.97f;
        private float zoomLevels = 20.0f;

        public float PanSensitivity
        {
            get => panSensitivity;
            set => SetProperty(ref panSensitivity, value);
        }

        public float ZoomLevels
        {
            get => zoomLevels;
            set => SetProperty(ref zoomLevels, value);
        }

        /// <summary>
        /// Percentage (Element [0;1])
        /// </summary>
        public float ZoomLimit
        {
            get => zoomLimit;
            set
            {
                SetProperty(ref zoomLimit, value);
                _zoomValue = Math.Min(_zoomValue, _zoomMax * value);
                ReloadLayout();
            }
        }

        /// <summary>
        /// Element [0;1]
        /// </summary>
        public float Zoom
        {
            get => _zoomValue * 2;
            set
            {
                _zoomValue = value / 2;
                ReloadLayout();
            }
        }


        public PreviewImageViewModel()
        {
            _vertexArray = new VertexArrayModel(sIndx: _indices, sVert: _vertices, BufferUsageHint.StaticDraw);
            _vertexArray.Use();

            _shader = ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }
        }


        public void RenderToImage(GLWpfControl image)
        {
            if (_shader is null || _editor is null)
            {
                return;
            }

            var fb = _editor.Compute(out bool success);

            if (success == false || fb is null)
            {
                return;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, image.Framebuffer);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            fb.Data.Use(TextureUnit.Texture0);
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

            float newZoom = _zoomValue + (zoomDelta / zoomLevels);
            newZoom = Math.Clamp(newZoom, _zoomMin, _zoomMax * zoomLimit);

            _panXStart = (_panXStart + relativeXScaled * _zoomValue) - (relativeXScaled * newZoom);
            _panYStart = (_panYStart + relativeYScaled * _zoomValue) - (relativeYScaled * newZoom);
            _zoomValue = newZoom;

            ReloadLayout();
        }

        public void DragDelta(object sender, DragDeltaEventArgs e)
        {
            var relativeXScaled = (float)(e.HorizontalChange / BoundsSize.Width);
            var relativeYScaled = (float)(e.VerticalChange / BoundsSize.Height);

            _panXDelta = relativeXScaled * (1 - _zoomValue * 2) * panSensitivity;
            _panYDelta = relativeYScaled * (1 - _zoomValue * 2) * panSensitivity;

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

        public void ReloadLayout()
        {
            float
                wNorm = (float)(BoundsSize.Width / RenderSize.Width),
                hNorm = (float)(BoundsSize.Height / RenderSize.Height);

            float panX = _panXDelta + _panXStart;
            float panY = _panYDelta + _panYStart;

            float left      = 0 + _zoomValue - panX;
            float right     = 1 - _zoomValue - panX;
            float top       = 1 - _zoomValue + panY;
            float bottom    = 0 + _zoomValue + panY;

            // align top
            float 
                xOff = 0, 
                yOff = (float)(RenderSize.Height - BoundsSize.Height) / (float)(RenderSize.Height);

            _vertices =
            [
                // Position     Texture coordinates
                 1,  1, 0,      xOff + wNorm * right, yOff + hNorm * top,    // top right
                 1, -1, 0,      xOff + wNorm * right, yOff + hNorm * bottom, // bottom right
                -1, -1, 0,      xOff + wNorm * left,  yOff + hNorm * bottom, // bottom left
                -1,  1, 0,      xOff + wNorm * left,  yOff + hNorm * top,    // top left
            ];

            _vertexArray.UpdateVertices(_vertices);
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
}
