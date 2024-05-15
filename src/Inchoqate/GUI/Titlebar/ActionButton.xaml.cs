using System;
using System.Collections.Generic;
using System.Globalization;
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


    [ValueConversion(sourceType: typeof(ActionButtonOptionsPosition), targetType: typeof(int))]
    public class ActionButtonOptionsPositionToIndexConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var position = (ActionButtonOptionsPosition)value;
            var gridIndex = (GridIndex)parameter;

            return position == ActionButtonOptionsPosition.Bottom
                ? (gridIndex == GridIndex.Column ? 0 : 1) 
                : (gridIndex == GridIndex.Column ? 1 : 0);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class MaximumSelector : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Cast<double>().Max();
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Interaction logic for ActionButton.xaml
    /// </summary>
    public partial class ActionButton : ActionButtonOption
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(ImageSource), typeof(ActionButton));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ActionButton));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty IndicatorVisibilityProperty = DependencyProperty.Register(
            "IndicatorVisibility", typeof(Visibility), typeof(ActionButton), new(Visibility.Collapsed));

        public Visibility IndicatorVisibility
        {
            get => (Visibility)GetValue(IndicatorVisibilityProperty);
            set => SetValue(IndicatorVisibilityProperty, value);
        }


        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(ActionButtonOptionCollection), typeof(ActionButton));

        public ActionButtonOptionCollection Options
        {
            get => (ActionButtonOptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }


        public static readonly DependencyProperty OptionsPositionProperty = DependencyProperty.Register(
            "OptionsPosition", typeof(ActionButtonOptionsPosition), typeof(ActionButton));

        public ActionButtonOptionsPosition OptionsPosition
        {
            get => (ActionButtonOptionsPosition)GetValue(OptionsPositionProperty);
            set => SetValue(OptionsPositionProperty, value);
        }


        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
            "IsCollapsed", typeof(bool), typeof(ActionButton));

        public bool IsCollapsed
        {
            get => (bool)GetValue(IsCollapsedProperty);
            set => SetValue(IsCollapsedProperty, value);
        }


        public static readonly DependencyProperty ClickModeProperty = DependencyProperty.Register(
            "ClickMode", typeof(ClickMode), typeof(ActionButton), new(OnClickModeChanged));

        public ClickMode ClickMode
        {
            get => (ClickMode)GetValue(ClickModeProperty);
            set => SetValue(ClickModeProperty, value);
        }

        static void OnClickModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = (ActionButton)d;

            if ((ClickMode)e.NewValue == ClickMode.Hover)
            {
                b.E_Thumb.E_Button.Click -= b.Click;
                b.E_Thumb.MouseEnter += b.HoverMouseEnter;
                b.E_MainGrid.MouseLeave += b.HoverMouseLeave;
            }
            else
            {
                b.E_Thumb.E_Button.Click += b.Click;
                b.E_Thumb.MouseEnter -= b.HoverMouseEnter;
                b.E_MainGrid.MouseLeave -= b.HoverMouseLeave;
            }
        }


        public static readonly DependencyProperty ButtonPaddingProperty = DependencyProperty.Register(
            "ButtonPadding", typeof(Thickness), typeof(ActionButton));

        public Thickness ButtonPadding
        {
            get => (Thickness)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }


        public static readonly DependencyProperty OptionsIconMaxWidthProperty = DependencyProperty.Register(
            "OptionsIconMaxWidth", typeof(double), typeof(ActionButton));

        public double OptionsIconMaxWidth
        {
            get => (double)GetValue(OptionsIconMaxWidthProperty);
            set => SetValue(OptionsIconMaxWidthProperty, value);
        }
        

        public static readonly DependencyProperty OptionsShortcutMaxWidthProperty = DependencyProperty.Register(
            "OptionsShortcutMaxWidth", typeof(double), typeof(ActionButton));

        public double OptionsShortcutMaxWidth
        {
            get => (double)GetValue(OptionsShortcutMaxWidthProperty);
            set => SetValue(OptionsShortcutMaxWidthProperty, value);
        }


        public event EventHandler? VisibilityChanged;

        int ccount = -1;

        public ActionButton()
        {
            InitializeComponent();

            E_Thumb.E_Button.Click += Click;

            E_Options.LayoutUpdated += delegate
            {
                if (E_Options.Items.Count == ccount)
                {
                    return;
                }

                ccount = E_Options.Items.Count;

                // Icon
                //var iconsBinding = new MultiBinding { Converter = new MaximumSelector() };
                //foreach (ActionButtonOption option in E_Options.Items)
                //{
                //    if (option is ActionButtonAction button1)
                //    {
                //        var icon = button1.E_Icon;
                //        iconsBinding.Bindings.Add(new Binding("ActualWidth") { Source = icon });
                //    }
                //    else if (option is ActionButton button2)
                //    {
                //        var icon = button2.E_Thumb.E_Icon;
                //        iconsBinding.Bindings.Add(new Binding("ActualWidth") { Source = icon });
                //    }
                //}
                //E_Thumb.SetBinding(ActionButtonAction.IconMinWidthProperty, iconsBinding);

                // Shortcut
                var shortcutBinding = new MultiBinding { Converter = new MaximumSelector() };
                foreach (ActionButtonOption option in E_Options.Items)
                {
                    if (option is ActionButtonAction button1)
                    {
                        var shortcut = button1.E_Shortcut;
                        shortcutBinding.Bindings.Add(new Binding("ActualWidth") { Source = shortcut });
                    }
                    else if (option is ActionButton button2)
                    {
                        var shortcut = button2.E_Thumb.E_Shortcut;
                        shortcutBinding.Bindings.Add(new Binding("ActualWidth") { Source = shortcut });
                    }
                }

                this.SetBinding(OptionsShortcutMaxWidthProperty, shortcutBinding);

                //double max = 0;
                //foreach (ActionButtonOption option in E_Options.Items)
                //{
                //    if (option is ActionButtonAction button1)
                //    {
                //        var shortcut = button1.E_Shortcut;
                //        max = Math.Max(shortcut.ActualWidth, max);
                //    }
                //    else if (option is ActionButton button2)
                //    {
                //        var shortcut = button2.E_Thumb.E_Shortcut;
                //        max = Math.Max(shortcut.ActualWidth, max);
                //    }
                //}

                //foreach (ActionButtonOption option in E_Options.Items)
                //{
                //    option.ShortcutMinWidth = max;
                //}
            };

            Loaded += delegate
            {
                Collapse();

            };
        }


        public void Toggle()
        {
            if (IsCollapsed)
            {
                Show();
            }
            else
            {
                Collapse();
            }
        }

        public void Collapse()
        {
            E_OptionsCanvas.Visibility = Visibility.Collapsed;
            IsCollapsed = true;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Show()
        {
            E_OptionsCanvas.Visibility = Visibility.Visible;
            IsCollapsed = false;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }


        private void Click(object sender, RoutedEventArgs e)
        {
            Toggle();
        }

        private void HoverMouseLeave(object sender, MouseEventArgs e)
        {
            Collapse();
        }

        private void HoverMouseEnter(object sender, MouseEventArgs e)
        {
            Show();
        }
    }
}
