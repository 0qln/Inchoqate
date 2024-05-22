using Inchoqate.GUI.Windows;
using Inchoqate.Logging;
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
using System.Windows.Shapes;

namespace Inchoqate.GUI.View
{
    public partial class BorderlessWindowView : Window
    {
        private static ILogger _logger = FileLoggerFactory.CreateLogger<BorderlessWindowView>();


        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(BorderlessWindowView),
                new (new CornerRadius(15)));

        public static readonly DependencyProperty TitlebarHeightProperty =
            DependencyProperty.Register(
                "TitlebarHeight",
                typeof(double),
                typeof(BorderlessWindowView),
                new (28.0));

        public static readonly DependencyProperty ContentPageProperty =
            DependencyProperty.Register(
                "ContentPage",
                typeof(Page),
                typeof(BorderlessWindowView));


        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public double TitlebarHeight
        {
            get => (double)GetValue(TitlebarHeightProperty);
            set => SetValue(TitlebarHeightProperty, value);
        }

        public Page ContentPage
        {
            get => (Page)GetValue(ContentPageProperty);
            set => SetValue(ContentPageProperty, value);
        }


        public BorderlessWindowView()
        {
            DataContext = this;

            InitializeComponent();
        }
    }
}
