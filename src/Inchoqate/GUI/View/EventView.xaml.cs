using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Inchoqate.GUI.View;

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
            NextNodes.Add(new EventView
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


public class EventArgsInfoConverter : IValueConverter
{
    object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = new ObservableCollection<string>();
        if (value is null) { return result; }
        var vm = (EventViewModelBase)value;
        foreach (var arg in vm.GetType()
                     .GetProperties()
                     .Where(prop => prop
                         .GetCustomAttributes(true)
                         .OfType<ViewProperty>()
                         .Any()))
        {
            var argN = arg.Name;
            var argV = arg.GetValue(vm)?.ToString() ?? "";
            result.Add($"{argN}: {argV}");
        }
        return result;
    }

    object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}



public class NodeConnectorAdorner(EventView adorned) : Adorner(adorned.EventInfo)
{
    public static readonly DependencyProperty RevertedBrushProperty =
        DependencyProperty.Register(
            nameof(RevertedBrush),
            typeof(Brush),
            typeof(NodeConnectorAdorner),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ExecutedBrushProperty =
        DependencyProperty.Register(
            nameof(ExecutedBrush),
            typeof(Brush),
            typeof(NodeConnectorAdorner),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));


    public Brush RevertedBrush
    {
        get => (Brush)GetValue(RevertedBrushProperty);
        set => SetValue(RevertedBrushProperty, value);
    }

    public Brush ExecutedBrush
    {
        get => (Brush)GetValue(ExecutedBrushProperty);
        set => SetValue(ExecutedBrushProperty, value);
    }


    protected override void OnRender(DrawingContext drawingContext)
    {
        if (adorned.ViewModel is null)
            return;

        double adjust = 1, linewidth = 2;
        double x, y, width, height;
        var stackpanel = Utils.FindVisualChildOfType<StackPanel>(adorned.NextNodesContainer);
        var next = stackpanel.Children.FirstOrDefault<EventView>(e => e.ViewModel.State == EventState.Executed);

        y = adorned.EventInfo.ActualHeight / 2;

        // prev h
        if (adorned.ViewModel.Previous is not null)
        {
            x = -adorned.EventInfo.Margin.Left;
            width = adorned.EventInfo.Margin.Left;
            drawingContext.DrawRectangle(
                adorned.ViewModel.State == EventState.Executed ? ExecutedBrush : RevertedBrush,
                null,
                new Rect(x - adjust, y, width + adjust, linewidth));
        }

        // next h
        if (adorned.ViewModel.Next.Count > 0)
        {
            x = adorned.EventInfo.ActualWidth;
            width = adorned.EventInfo.Margin.Right;
            drawingContext.DrawRectangle(
                next is not null ? ExecutedBrush : RevertedBrush,
                null,
                new Rect(x, y, width + adjust, linewidth));
        }

        // next v
        if (adorned.ViewModel.Next.Count > 1)
        {
            var top = (EventView)stackpanel.Children[0];
            var bottom = (EventView)stackpanel.Children[^1];
            var span = top.EventInfo.TransformToVisual(bottom.EventInfo).Transform(new()).Y;

            // next reverted
            x = adorned.EventInfo.ActualWidth + adorned.EventInfo.Margin.Left;
            y = -Math.Abs(top.EventInfo.TransformToVisual(adorned.EventInfo).Transform(new()).Y) + top.EventInfo.ActualHeight / 2;
            height = Math.Abs(span) - Math.Abs(top.EventInfo.ActualHeight - bottom.EventInfo.ActualHeight) / 2;
            drawingContext.DrawRectangle(RevertedBrush, null, new Rect(x, y, linewidth, height));

            // next executed
            if (adorned.ViewModel.State == EventState.Executed
                && next is not null)
            {
                var diff = next.EventInfo.TransformToVisual(adorned.EventInfo).Transform(new()).Y;
                height = Math.Abs(diff) - Math.Abs(next.EventInfo.ActualHeight - adorned.EventInfo.ActualHeight) / 2;
                y = diff < 0 ? diff + next.EventInfo.ActualHeight / 2 : adorned.EventInfo.ActualHeight / 2;
                drawingContext.DrawRectangle(ExecutedBrush, null, new Rect(x, y, linewidth, height));
            }
        }
    }
}