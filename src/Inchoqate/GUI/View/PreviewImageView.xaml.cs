using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Inchoqate.GUI.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;

namespace Inchoqate.GUI.View;

public partial class PreviewImageView : UserControl
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<PreviewImageView>();

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(
            nameof(Stretch),
            typeof(Stretch),
            typeof(PreviewImageView),
            new FrameworkPropertyMetadata(
                Stretch.Uniform,
                FrameworkPropertyMetadataOptions.AffectsArrange));


    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }


    private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PreviewImageView control && 
            control.DataContext is PreviewImageViewModel viewModel)
        {
            viewModel.VoidColor = (SolidColorBrush)e.NewValue;
        }
    }


    static PreviewImageView()
    {
        BackgroundProperty.OverrideMetadata(
            typeof(PreviewImageView), 
            new FrameworkPropertyMetadata(
                new SolidColorBrush(default),
                FrameworkPropertyMetadataOptions.AffectsRender | 
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                OnBackgroundPropertyChanged));
    }


    private readonly PreviewImageViewModel _viewModel;


    public PreviewImageView()
    {
        InitializeComponent();

        // If this is left enabled, the window that contains the
        // PreviewImage will multiply CommandBindings uncontrollably.
        GLImage.RegisterToEventsDirectly = false;

        GLImage.Start(new GLWpfControlSettings { RenderContinuously = false });

        DataContext = _viewModel = new();
        _viewModel.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(_viewModel.RenderEditor):
                    this.InvalidateVisual();
                    GLImage.InvalidateVisual();
                    break;
                case nameof(_viewModel.ActualLayout):
                    GLImage.InvalidateVisual();
                    break;
            }
        };
    }


    private Size GetDesiredImageSize(Size bounds, Size sourceSize)
    {
        if (_viewModel.RenderEditor is null)
        {
            return bounds;
        }

        if (double.IsInfinity(bounds.Width) || double.IsInfinity(bounds.Height))
        {
            return sourceSize;
        }

        double aspectRatio = (double)sourceSize.Height / sourceSize.Width;
        double boundsRatio = bounds.Height / bounds.Width;

        return Stretch switch
        {
            Stretch.None => new Size(sourceSize.Width, sourceSize.Height),
            Stretch.Fill => bounds,
            Stretch.Uniform => boundsRatio > aspectRatio
                ? new(bounds.Width, bounds.Width * aspectRatio)
                : new(bounds.Height / aspectRatio, bounds.Height),
            Stretch.UniformToFill => boundsRatio > aspectRatio
                ? new(bounds.Height / aspectRatio, bounds.Height)
                : new(bounds.Width, bounds.Width * aspectRatio),
            _ => throw new ArgumentException(nameof(Stretch)),
        };
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var sourceSize = _viewModel.RenderEditor?.SourceSize ?? default;

        _viewModel.DisplaySize = GetDesiredImageSize(arrangeBounds, sourceSize);
        _viewModel.BoundsSize = new(
            width: Math.Max(sourceSize.Width, arrangeBounds.Width),
            height: Math.Max(sourceSize.Height, arrangeBounds.Height)
        );

        // This will always run before the GLImage is rendered. (WPF pipeline)
        // Prepare the GL image to render the image in source size before
        // rendering to the screen with bounds size.
        GLImage.Width = sourceSize.Width;
        GLImage.Height = sourceSize.Height;
            
        Thumb.Width = arrangeBounds.Width;
        Thumb.Height = arrangeBounds.Height;
        Grid.Width = arrangeBounds.Width;
        Grid.Height = arrangeBounds.Height;
        Viewbox.Width = arrangeBounds.Width;
        Viewbox.Height = arrangeBounds.Height;

        return base.ArrangeOverride(arrangeBounds);
    }


    private void OpenTK_OnRender(TimeSpan delta)
    {
        var editor = _viewModel.RenderEditor;
        var shader = _viewModel.Shader;
        var vertex = _viewModel.VertexArray;
        if (editor is null)
        {
            return;
        }

        if (_viewModel.RenderEditor?.Result is null)
        {
            var success = editor.Compute();

            if (!success || editor.Result is null)
            {
                Logger.LogError("Failed to compute preview image.");
                return;
            }
        }

        // The GL image should only render at BoundsSize once the editor has
        // Finished working on the image before rendering to the screen.
        // Otherwise, the image will be distorted and glitched.
        GLImage.Width = (int)_viewModel.BoundsSize.Width;
        GLImage.Height = (int)_viewModel.BoundsSize.Height;
                 
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLImage.Framebuffer);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        editor.Result!.Data.Use(TextureUnit.Texture0);
        shader.Use();
        vertex.Use();
        GL.DrawElements(PrimitiveType.Triangles, vertex.IndexCount, DrawElementsType.UnsignedInt, 0);

    }

    private void Viewbox_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        _viewModel?.MouseWheel(sender, e);
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        _viewModel?.DragDelta(sender, e);
    }

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        _viewModel?.DragStarted(sender, e);
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _viewModel?.DragCompleted(sender, e);
    }

    private void Thumb_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        _viewModel?.ResetZoom();
    }
}