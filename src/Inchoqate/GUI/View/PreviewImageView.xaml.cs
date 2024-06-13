using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using OpenTK.Wpf;

namespace Inchoqate.GUI.View
{
    public partial class PreviewImageView : UserControl
    {
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
            if (_viewModel is null
                || _viewModel.RenderEditor is null)
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

            // render size has to be atleast bounds size, otherwise the
            // result will clip outside the GLImage while zooming and panning.
            var renderSize = new Size (
                width: Math.Max(sourceSize.Width, arrangeBounds.Width),
                height: Math.Max(sourceSize.Height, arrangeBounds.Height));
            if (_viewModel.RenderEditor is not null)
                _viewModel.RenderEditor.RenderSize = renderSize;
            _viewModel.BoundsSize = renderSize;
            GLImage.Width = renderSize.Width;
            GLImage.Height = renderSize.Height;

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
