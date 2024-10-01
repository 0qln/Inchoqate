using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.Input;

namespace Inchoqate.UserControls.MenuButton;

/// <summary>
///     Interaction logic for MenuButton.xaml
/// </summary>
[ContentProperty(nameof(MenuItems))]
// ReSharper disable once RedundantExtendsListEntry
public partial class MenuButton : UserControl
{
    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register(
            nameof(ButtonContent),
            typeof(object),
            typeof(MenuButton),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MenuPositionProperty =
        DependencyProperty.Register(
            nameof(MenuPosition),
            typeof(MenuPosition),
            typeof(MenuButton),
            new FrameworkPropertyMetadata(
                MenuPosition.Bottom,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MenuProperty =
        DependencyProperty.Register(
            nameof(Menu),
            typeof(Canvas),
            typeof(MenuButton));

    public static readonly Action<DependencyObject> HideMenu = @this =>
    {
        if (@this is MenuButton menuButton) menuButton.CollapseDownwards();
    };

    public static readonly Action<DependencyObject> ShowMenu = @this =>
    {
        if (@this is MenuButton menuButton) menuButton.Show();
    };


    public MenuButton()
    {
        MenuItems = new(this);
        InitializeComponent();
        SetValue(MenuProperty, Menu);
    }

    public object? ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    /// <summary>
    ///     This is used in the xaml code to add items to the menu.
    ///     Don't turn this into a Dependency Property, or it will break things.
    /// </summary>
    public MenuButtonItemCollection MenuItems { get; }

    public MenuPosition MenuPosition
    {
        get => (MenuPosition)GetValue(MenuPositionProperty);
        set => SetValue(MenuPositionProperty, value);
    }

    public ICommand ToggleMenuCommand => new RelayCommand(
        () => (Menu.Visibility == Visibility.Visible ? HideMenu : ShowMenu)(this)
    );

    internal MenuButtonItem? NestingParent { get; set; }

    public void Collapse()
    {
        Menu.Visibility = Visibility.Collapsed;
    }

    public void Show()
    {
        Menu.Visibility = Visibility.Visible;
    }

    public void CollapseDownwards()
    {
        foreach (var item in MenuItems)
            if (item.FirstOrDefault() is MenuButton menuButton)
                menuButton.CollapseDownwards();

        Collapse();
    }

    public void CollapseAll()
    {
        var root = this;
        FrameworkElement? element = root;

        do
        {
            // It's either some control or a menu button that is a child of a control.
            element = element.Parent as FrameworkElement ??
                      // Or it's a menu button that is nested within another menu button.
                      // In this case, the menu button gets fucked up by the content
                      // converter and is not a part of the visual tree anymore.
                      (element as MenuButton)?.NestingParent?.Parent ??
                      // Any other case is undefined behaviour and we can break.
                      null;
            if (element is MenuButton menuButton)
                root = menuButton;
        } while (element is not null);

        root.CollapseDownwards();
    }
}