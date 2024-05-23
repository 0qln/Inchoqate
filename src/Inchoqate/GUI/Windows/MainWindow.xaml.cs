using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Inchoqate.GUI.Windows
{
    public partial class MainWindow : BorderlessWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();

            PreviewImage.ImageSource = @"C:\Users\User\OneDrive\Bilder\Wallpapers\z\wallhaven-l8rloq.jpg";

        }

        private void SliderThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                PreviewImage.Width += e.HorizontalChange;
            }
        }
    }
}