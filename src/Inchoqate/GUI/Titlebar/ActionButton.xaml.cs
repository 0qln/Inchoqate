using Inchoqate.GUI.Titlebar;
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

namespace Inchoqate.GUI.Titlebar
{
    /// <summary>
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    public partial class ActionButton : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(ObservableCollection<Control>), typeof(ActionButton));

        public ObservableCollection<Control> ItemsSource
        {
            get => (ObservableCollection<Control>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ActionButton));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
            "IsCollapsed", typeof(bool), typeof(ActionButton));

        public bool IsCollapsed
        {
            get => (bool)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public ActionButton()
        {
            InitializeComponent();
        }


        private void E_This_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsCollapsed)
            {
                E_ActionsCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                E_ActionsCanvas.Visibility= Visibility.Collapsed;
            }
        }
    }
}
