using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Inchoqate.GUI.View
{
    public class TitlebarActionButtonOptionView : UserControl
    {
        private const FrameworkPropertyMetadataOptions ContentPropertyMetadata =
            // Affects the measurements of this control
            FrameworkPropertyMetadataOptions.AffectsMeasure |
            // The action button menu aligns it's options wrt. it's contents
            FrameworkPropertyMetadataOptions.AffectsParentMeasure;

        public static readonly DependencyProperty IconProperty = 
            DependencyProperty.Register(
                "Icon", 
                typeof(ImageSource), 
                typeof(TitlebarActionButtonOptionView),
                new FrameworkPropertyMetadata(
                    null,
                    ContentPropertyMetadata));

        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register(
                "Title", 
                typeof(string), 
                typeof(TitlebarActionButtonOptionView),
                new FrameworkPropertyMetadata(
                    "",
                    ContentPropertyMetadata));

        public static readonly DependencyProperty IndicatorVisibilityProperty = 
            DependencyProperty.Register(
                "IndicatorVisibility", 
                typeof(Visibility), 
                typeof(TitlebarActionButtonOptionView), 
                new FrameworkPropertyMetadata(
                    Visibility.Collapsed,
                    ContentPropertyMetadata));

        public static readonly DependencyProperty ShortcutProperty = 
            DependencyProperty.Register(
                "Shortcut", 
                typeof(CommandBinding), 
                typeof(TitlebarActionButtonOptionView), 
                new FrameworkPropertyMetadata(
                    null, 
                    ContentPropertyMetadata));

        public static readonly DependencyProperty IndicatorContentProperty =
            DependencyProperty.Register(
                "IndicatorContent",
                typeof(Visibility),
                typeof(TitlebarActionButtonOptionView),
                new FrameworkPropertyMetadata(
                    Visibility.Collapsed,
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

        public CommandBinding Shortcut
        {
            get => (CommandBinding)GetValue(ShortcutProperty);
            set => SetValue(ShortcutProperty, value);
        }

        public Visibility IndicatorVisibility
        {
            get => (Visibility)GetValue(IndicatorVisibilityProperty);
            set => SetValue(IndicatorVisibilityProperty, value);
        }

        public Visibility IndicatorContent
        {
            get => (Visibility)GetValue(IndicatorContentProperty);
            set => SetValue(IndicatorContentProperty, value);
        }

        public TitlebarActionButtonOptionView()
        {
        }
    }
}
