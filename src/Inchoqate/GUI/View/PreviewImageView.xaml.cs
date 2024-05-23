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
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
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


        public PreviewImageView()
        {
            InitializeComponent();

            GLImage.Start(new GLWpfControlSettings
            {
                RenderContinuously = false,
            });

            var viewModel = new PreviewImageViewModel();
            DataContext = viewModel;
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var viewModel = DataContext as PreviewImageViewModel;

            if (viewModel is null || viewModel.SourceSize == Size.Empty)
            {
                return new Size();
            }

            double aspectRatio = (double)viewModel.SourceSize.Height / viewModel.SourceSize.Width;
            double boundsRatio = availableSize.Height / availableSize.Width;

            // TODO: test case Stretch.UniformToFill

            var result = Stretch switch
            {
                Stretch.None => new Size(viewModel.SourceSize.Width, viewModel.SourceSize.Height),
                Stretch.Fill => availableSize,
                Stretch.Uniform => boundsRatio > aspectRatio 
                    ? new (availableSize.Width, availableSize.Width * aspectRatio)
                    : new (availableSize.Height / aspectRatio, availableSize.Height),
                Stretch.UniformToFill => boundsRatio > aspectRatio
                    ? new (availableSize.Height / aspectRatio, availableSize.Height)
                    : new (availableSize.Width, availableSize.Width * aspectRatio),
                _ => throw new ArgumentException(nameof(Stretch)),
            };

            viewModel.RenderSize = result;

            return result;
        }


        private void OpenTK_OnRender(TimeSpan delta)
        {
            var viewModel = DataContext as PreviewImageViewModel;
            viewModel?.RenderToImage(GLImage);
        }
    }
}
