using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.View.Events;

public partial class EventView : UserControl
{
    // make readonly
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(EventViewModelBase),
            typeof(EventView),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnEventChanged));

    private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var @this = (EventView)d;
        @this.UpdateNextNodes();
    }

    // make readonly
    public static readonly DependencyProperty TreeProperty =
        DependencyProperty.Register(
            nameof(Tree),
            typeof(EventTreeViewModel),
            typeof(EventView),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                OnTreeChanged));

    private static void OnTreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var @this = (EventView)d;
        @this.UpdateNextNodes();
        @this.Tree.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                // TODO
                case nameof(@this.Tree.Current) /*when @this.ViewModel == @this.Tree.Current.Previous*/:
                    @this.UpdateNextNodes();
                    break;
            }
        };
    }


    public static readonly DependencyProperty NextNodesProperty = 
        DependencyProperty.Register(
            nameof(NextNodes),
            typeof(ObservableCollectionBase<EventView>),
            typeof(EventView),
            new PropertyMetadata(null));

    private void UpdateNextNodes()
    {
        if (ViewModel is null || Tree is null)
            return;

        NextNodes ??= [];

        foreach (var viewModel in NextNodes.Select(x => x.ViewModel).Except(ViewModel.Next.Values))
            NextNodes.Remove(NextNodes.First(x => x.ViewModel == viewModel));

        foreach (var viewModel in ViewModel.Next.Values.Except(NextNodes.Select(x => x.ViewModel)))
            NextNodes.Add(new()
            {
                Tree = Tree,
                ViewModel = viewModel,
            });
                
        _adorner?.InvalidateVisual();
    }



    public EventViewModelBase ViewModel
    {
        get => (EventViewModelBase)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public EventTreeViewModel Tree
    {
        get => (EventTreeViewModel)GetValue(TreeProperty);
        set => SetValue(TreeProperty, value);
    }

    public ObservableCollectionBase<EventView> NextNodes
    {
        get => (ObservableCollectionBase<EventView>)GetValue(NextNodesProperty);
        set => SetValue(NextNodesProperty, value);
    }


    public EventView()
    {
        InitializeComponent();

        Loaded += EventTreeRendererView_Loaded;
    }


    private NodeConnectorAdorner? _adorner;

    private void EventTreeRendererView_Loaded(object sender, RoutedEventArgs e)
    {
        if (_adorner is null)
        {
            _adorner = new NodeConnectorAdorner(this);
            AdornerLayer.GetAdornerLayer(this).Add(_adorner);
            _adorner.ExecutedBrush = new SolidColorBrush(Colors.LightGreen);
            _adorner.RevertedBrush = new SolidColorBrush(Colors.IndianRed);
        }
    }
}