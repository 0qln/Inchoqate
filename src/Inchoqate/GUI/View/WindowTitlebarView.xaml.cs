using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ActionButtonCollection : ObservableCollection<TitlebarActionButtonMenuView>
    {
    }

    public partial class WindowTitlebarView : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                "Icon",
                typeof(ImageSource),
                typeof(WindowTitlebarView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(WindowTitlebarView),
                new FrameworkPropertyMetadata(
                    "Inchoqate",
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ActionButtonsProperty =
            DependencyProperty.Register(
                "ActionButtons",
                typeof(ActionButtonCollection),
                typeof(WindowTitlebarView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ActionButtonCollection ActionButtons
        {
            get => (ActionButtonCollection)GetValue(ActionButtonsProperty);
            set => SetValue(ActionButtonsProperty, value);
        }


        public WindowTitlebarView()
        {
            InitializeComponent();
        }

        private void WindowedButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ActionButtonStack_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
