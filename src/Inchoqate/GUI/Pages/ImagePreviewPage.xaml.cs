﻿using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
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

namespace Inchoqate.GUI.Pages
{
    public partial class ImagePreviewPage : Page
    {
        private ILogger _logger = FileLoggerFactory.CreateLogger<ImagePreviewPage>();

        public ImagePreviewPage()
        {
            InitializeComponent();
        }

        private void ImageControl_Render(TimeSpan obj)
        {

        }
    }
}