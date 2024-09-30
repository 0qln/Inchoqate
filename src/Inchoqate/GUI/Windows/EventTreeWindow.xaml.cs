using System.Collections.ObjectModel;
using System.Windows;
using Inchoqate.CustomControls;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.Windows;

public partial class EventTreeWindow : BorderlessWindow
{
    // TODO: make readonly
    public static readonly DependencyProperty EventTreesProperty = 
        DependencyProperty.Register(
            nameof(EventTrees),
            typeof(ObservableCollection<EventTreeViewModel>),
            typeof(EventTreeWindow),
            new FrameworkPropertyMetadata(
                EventTreeViewModel.RegisteredTrees,
                FrameworkPropertyMetadataOptions.AffectsRender));


    public ObservableCollection<EventTreeViewModel> EventTrees
    {
        get => (ObservableCollection<EventTreeViewModel>)GetValue(EventTreesProperty);
        set => SetValue(EventTreesProperty, value);
    }


    public EventTreeWindow()
    {
        InitializeComponent();

        EventTreeViewModel.RegisteredTrees.CollectionChanged += (s, e) =>
        {
            InvalidateProperty(EventTreesProperty);
        };
    }
}