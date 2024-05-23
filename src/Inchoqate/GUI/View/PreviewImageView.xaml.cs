using Inchoqate.GUI.ViewModel;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Wpf;
using System.Diagnostics;

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
                    FrameworkPropertyMetadataOptions.AffectsArrange,
                    OnStretchPropertyChanged));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSource",
                typeof(string),
                typeof(PreviewImageView),
                new FrameworkPropertyMetadata(
                    "",
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnImageSourcePropertyChanged));


        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }


        private static void OnStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PreviewImageView control && control.DataContext is PreviewImageViewModel viewModel)
            {
            }
        }

        private static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PreviewImageView control && control.DataContext is PreviewImageViewModel viewModel)
            {
                viewModel.ImageSource = (string)e.NewValue;
            }
        }


        private readonly PreviewImageViewModel _viewModel;


        public PreviewImageView()
        {
            InitializeComponent();

            GLImage.Start(new GLWpfControlSettings
            {
                RenderContinuously = false,
            });

            DataContext = _viewModel = new PreviewImageViewModel();
        }


        private Size GetDesiredImageSize(Size bounds)
        {
            if (_viewModel is null || _viewModel.SourceSize == default)
            {
                return default;
            }

            if (double.IsInfinity(bounds.Width) || double.IsInfinity(bounds.Height))
            {
                return _viewModel.SourceSize;
            }

            double aspectRatio = (double)_viewModel.SourceSize.Height / _viewModel.SourceSize.Width;
            double boundsRatio = bounds.Height / bounds.Width;

            return Stretch switch
            {
                Stretch.None => new Size(_viewModel.SourceSize.Width, _viewModel.SourceSize.Height),
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
            var size = GetDesiredImageSize(arrangeBounds);
            GLImage.Width = size.Width;
            GLImage.Height = size.Height;
            Viewbox.Width = arrangeBounds.Width;
            Viewbox.Height = arrangeBounds.Height;
            return base.ArrangeOverride(size);
        }


        private void OpenTK_OnRender(TimeSpan delta)
        {
            _viewModel?.RenderToImage(GLImage);
        }
    }
}
