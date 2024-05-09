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

namespace Inchoqate.GUI.Titlebar;


public class ActionButtonOptionCollection : ObservableCollection<Control>
{
    public ActionButtonOptionCollection()
    {
    }
}

/// <summary>
/// Interaction logic for ActionButton.xaml
/// </summary>
public partial class ActionButton : UserControl
{
    public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
        "Options", typeof(ActionButtonOptionCollection), typeof(ActionButton));

    public ActionButtonOptionCollection Options
    {
        get => (ActionButtonOptionCollection)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }


    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title", typeof(string), typeof(ActionButton));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }


    public event EventHandler? VisibilityChanged;

    public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
        "IsCollapsed", typeof(bool), typeof(ActionButton));

    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }


    public ActionButton()
    {
        InitializeComponent();

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
        E_ActionsCanvas.Visibility = Visibility.Collapsed;
        IsCollapsed = true;
        VisibilityChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Show()
    {
        E_ActionsCanvas.Visibility = Visibility.Visible;
        IsCollapsed = false;
        VisibilityChanged?.Invoke(this, EventArgs.Empty);
    }

    private void E_Title_Click(object sender, RoutedEventArgs e)
    {
        Toggle();
    }
}
