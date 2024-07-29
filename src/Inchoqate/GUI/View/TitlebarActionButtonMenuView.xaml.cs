using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Inchoqate.GUI.View;

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


public class TitlebarActionButtonOptionCollection : ObservableCollection<TitlebarActionButtonOptionView>
{
}


public partial class TitlebarActionButtonMenuView : TitlebarActionButtonOptionView
{
    public static readonly DependencyProperty OptionsProperty = 
        DependencyProperty.Register(
            nameof(Options), 
            typeof(TitlebarActionButtonOptionCollection), 
            typeof(TitlebarActionButtonMenuView),
            new FrameworkPropertyMetadata(
                new TitlebarActionButtonOptionCollection(),
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty OptionsPositionProperty = 
        DependencyProperty.Register(
            nameof(OptionsPosition), 
            typeof(ActionButtonOptionsPosition), 
            typeof(TitlebarActionButtonMenuView),
            new FrameworkPropertyMetadata(
                ActionButtonOptionsPosition.Right,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty OptionsVisibilityProperty = 
        DependencyProperty.Register(
            nameof(OptionsVisibility), 
            typeof(Visibility), 
            typeof(TitlebarActionButtonMenuView),
            new FrameworkPropertyMetadata(
                Visibility.Hidden,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty ClickModeProperty = 
        DependencyProperty.Register(
            nameof(ClickMode), 
            typeof(ClickMode), 
            typeof(TitlebarActionButtonMenuView), 
            new FrameworkPropertyMetadata(
                ClickMode.Release,
                OnClickModeChanged));

    public static readonly DependencyProperty ButtonPaddingProperty = 
        DependencyProperty.Register(
            nameof(ButtonPadding), 
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

    public TitlebarActionButtonOptionCollection Options
    {
        get => (TitlebarActionButtonOptionCollection)GetValue(OptionsProperty);
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


    private static void OnClickModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TitlebarActionButtonMenuView b) 
            return;

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


    public TitlebarActionButtonMenuView()
    {
        InitializeComponent();

        ActionButton.Button.Click += Click; 
    }


    private void Click(object sender, RoutedEventArgs e)
    {
        OptionsVisibility = OptionsVisibility == Visibility.Visible
            ? Visibility.Collapsed 
            : Visibility.Visible;
    }

    private void HoverMouseLeave(object sender, MouseEventArgs e)
    {
        OptionsVisibility = Visibility.Collapsed;
    }

    private void HoverMouseEnter(object sender, MouseEventArgs e)
    {
        OptionsVisibility = Visibility.Visible;
    }
}