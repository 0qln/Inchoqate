using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Inchoqate.GUI.Windows
{
    public abstract class BorderlessWindowBase : Window
    {
        private readonly ILogger _logger = FileLoggerFactory.CreateLogger<BorderlessWindowBase>();


        public static readonly DependencyProperty CornerRadiusProperty = 
            DependencyProperty.Register(
                "CornerRadius", 
                typeof(CornerRadius), 
                typeof(BorderlessWindowBase));

        public static readonly DependencyProperty TitlebarHeightProperty =
            DependencyProperty.Register(
                "TitlebarHeight",
                typeof(double),
                typeof(BorderlessWindowBase));


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


        public BorderlessWindowBase()
        {
            DataContext = this;
        }
    }
}
