using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using System.Windows.Media;
using Inchoqate.GUI.Model;
using System.Reflection.Metadata;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel
{
    public class PreviewImageViewModel : BaseViewModel, IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageViewModel>();


        private TextureModel? _texture;
        private Stretch _stretch;

        public TextureModel? Texture
        {
            get => _texture;
            set => SetProperty(ref _texture, value);
        }

        public Stretch Stretch
        {
            get => _stretch;
            set => SetProperty(ref _stretch, value);
        }


        #region Clean up

        void IDisposable.Dispose()
        {
            _texture?.Dispose();
        }

        #endregion
    }
}
