using Inchoqate.GUI.ViewModel;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.View
{
    public partial class PreviewImageView : GLWpfControl
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageView>();


        public static readonly DependencyProperty StretchProperty =
            Viewbox.StretchProperty.AddOwner(typeof(PreviewImageView));


        static PreviewImageView()
        {
            StretchProperty.OverrideMetadata(
                typeof(PreviewImageView),
                new FrameworkPropertyMetadata(
                    Stretch.Uniform,
                    FrameworkPropertyMetadataOptions.AffectsMeasure
                )
            );
        }


        public PreviewImageView()
        {
            DataContext = new PreviewImageViewModel();
        }


        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var viewModel = DataContext as PreviewImageViewModel;

            if (viewModel?.Texture is null)
            {
                return Size.Empty;
            }

            double aspectRatio = (double)viewModel.Texture.Height / viewModel.Texture.Width;
            double boundsRatio = availableSize.Height / availableSize.Width;

            // TODO: test case Stretch.UniformToFill

            switch (Stretch)
            {
                default:
                    throw new ArgumentException(nameof(Stretch));

                case Stretch.None:
                    return new Size(viewModel.Texture.Width, viewModel.Texture.Height);

                case Stretch.Fill:
                    return availableSize;

                case Stretch.Uniform:
                    if (boundsRatio > aspectRatio)
                        return new Size(
                            width: availableSize.Width,
                            height: availableSize.Width * aspectRatio);
                    else
                        return new Size(
                            width: availableSize.Height / aspectRatio,
                            height: availableSize.Height);

                case Stretch.UniformToFill:
                    if (boundsRatio > aspectRatio)
                        return new Size(
                            width: availableSize.Height / aspectRatio,
                            height: availableSize.Height);
                    else
                        return new Size(
                            width: availableSize.Width,
                            height: availableSize.Width * aspectRatio);

            }
        }
    }
}
