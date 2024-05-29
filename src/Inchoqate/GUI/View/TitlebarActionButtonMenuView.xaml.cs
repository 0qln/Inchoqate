using Inchoqate.GUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.View
{
    public enum ActionButtonOptionsPosition
    {
        Bottom,
        Right
    }


    public enum GridIndex
    {
        Row,
        Column
    }


    public class TitlebarActioButtonOptionCollection : ObservableCollection<TitlebarActionButtonOptionView>
    {
    }


    public partial class TitlebarActionButtonMenuView : TitlebarActionButtonOptionView
    {
        public static readonly DependencyProperty OptionsProperty = 
            DependencyProperty.Register(
                "Options", 
                typeof(TitlebarActioButtonOptionCollection), 
                typeof(TitlebarActionButtonMenuView),
                new FrameworkPropertyMetadata(
                    new TitlebarActioButtonOptionCollection(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty OptionsPositionProperty = 
            DependencyProperty.Register(
                "OptionsPosition", 
                typeof(ActionButtonOptionsPosition), 
                typeof(TitlebarActionButtonMenuView),
                new FrameworkPropertyMetadata(
                    ActionButtonOptionsPosition.Right,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty OptionsVisibilityProperty = 
            DependencyProperty.Register(
                "OptionsVisibility", 
                typeof(Visibility), 
                typeof(TitlebarActionButtonMenuView),
                new FrameworkPropertyMetadata(
                    Visibility.Collapsed,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ClickModeProperty = 
            DependencyProperty.Register(
                "ClickMode", 
                typeof(ClickMode), 
                typeof(TitlebarActionButtonMenuView), 
                new FrameworkPropertyMetadata(
                    ClickMode.Release,
                    OnClickModeChanged));

        public static readonly DependencyProperty ButtonPaddingProperty = 
            DependencyProperty.Register(
                "ButtonPadding", 
                typeof(Thickness), 
                typeof(TitlebarActionButtonMenuView),
                new FrameworkPropertyMetadata(
                    new Thickness(0),
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public Thickness ButtonPadding
        {
            get => (Thickness)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }

        public ClickMode ClickMode
        {
            get => (ClickMode)GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        public TitlebarActioButtonOptionCollection Options
        {
            get => (TitlebarActioButtonOptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }

        public ActionButtonOptionsPosition OptionsPosition
        {
            get => (ActionButtonOptionsPosition)GetValue(OptionsPositionProperty);
            set => SetValue(OptionsPositionProperty, value);
        }

        public Visibility OptionsVisibility
        {
            get => (Visibility)GetValue(OptionsVisibilityProperty);
            set => SetValue(OptionsVisibilityProperty, value);
        }


        static void OnClickModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TitlebarActionButtonMenuView b)
            {
                if ((ClickMode)e.NewValue == ClickMode.Hover)
                {
                    b.ActionButton.Button.Click -= b.Click;
                    b.ActionButton.MouseEnter += b.HoverMouseEnter;
                    b.MainGrid.MouseLeave += b.HoverMouseLeave;
                }
                else
                {
                    b.ActionButton.Button.Click += b.Click;
                    b.ActionButton.MouseEnter -= b.HoverMouseEnter;
                    b.MainGrid.MouseLeave -= b.HoverMouseLeave;
                }
            } 
        }


        public TitlebarActionButtonMenuView()
        {
            InitializeComponent();

            ActionButton.Button.Click += Click; 
        }


        private void Click(object sender, RoutedEventArgs e)
        {
            Toggle();
        }


        // TODO: clean up this structure
        private void Toggle()
        {
            if (OptionsVisibility == Visibility.Visible)
            {
                OptionsVisibility = Visibility.Collapsed;
            }
            else
            {
                OptionsVisibility = Visibility.Visible;
            }
        }

        private void HoverMouseLeave(object sender, MouseEventArgs e)
        {
            OptionsVisibility = Visibility.Collapsed;
        }

        private void HoverMouseEnter(object sender, MouseEventArgs e)
        {
            OptionsVisibility = Visibility.Visible;
        }


        private class ToggleCommand(TitlebarActionButtonMenuView menu) : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            bool ICommand.CanExecute(object? parameter)
            {
                return true;
            }

            void ICommand.Execute(object? parameter)
            {
                if (menu.OptionsVisibility == Visibility.Visible)
                {
                    menu.OptionsVisibility = Visibility.Collapsed;
                }
                else
                {
                    menu.OptionsVisibility = Visibility.Visible;
                }
            }
        }
    }

}
