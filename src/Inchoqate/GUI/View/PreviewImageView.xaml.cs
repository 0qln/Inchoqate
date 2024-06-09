using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.ViewModel;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using System.Diagnostics;
using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.View
{
    public partial class PreviewImageView : UserControl
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageView>();


        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                "Stretch",
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

            GLImage.Start(new GLWpfControlSettings { RenderContinuously = false });

            DataContext = _viewModel = new PreviewImageViewModel();
            _viewModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_viewModel.RenderEditor):
                        this.InvalidateVisual();
                        break;
                    case nameof(_viewModel.ActualLayout):
                        GLImage.InvalidateVisual();
                        break;
                }
            };
        }


        private Size GetDesiredImageSize(Size bounds)
        {
            if (_viewModel is null
                || _viewModel.RenderEditor is null
                || _viewModel.RenderEditor.RenderSize == default)
            {
                return default;
            }

            var renderSize = _viewModel.RenderEditor.RenderSize;

            if (double.IsInfinity(bounds.Width) || double.IsInfinity(bounds.Height))
            {
                return renderSize;
            }

            double aspectRatio = (double)renderSize.Height / renderSize.Width;
            double boundsRatio = bounds.Height / bounds.Width;

            return Stretch switch
            {
                Stretch.None => new Size(renderSize.Width, renderSize.Height),
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
            _viewModel.DisplaySize = GetDesiredImageSize(arrangeBounds);
            var renderSize = _viewModel.RenderEditor?.RenderSize ?? default;
            _viewModel.BoundsSize = renderSize;
            if (GLImage.Width != renderSize.Width || 
                GLImage.Height != renderSize.Height)
            {
                GLImage.Width = renderSize.Width;
                GLImage.Height = renderSize.Height;
            }
            Thumb.Width = arrangeBounds.Width;
            Thumb.Height = arrangeBounds.Height;
            Grid.Width = arrangeBounds.Width;
            Grid.Height = arrangeBounds.Height;
            Viewbox.Width = arrangeBounds.Width;
            Viewbox.Height = arrangeBounds.Height;
            GLImage.InvalidateVisual();
            return base.ArrangeOverride(arrangeBounds);
        }


        private void OpenTK_OnRender(TimeSpan delta)
        {
            _viewModel?.RenderToImage(GLImage);
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
}
