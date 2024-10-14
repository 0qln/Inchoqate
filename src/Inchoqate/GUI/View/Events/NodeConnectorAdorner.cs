using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.View.Events;

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
            drawingContext.DrawRectangle(RevertedBrush, null, new Rect(x, y, linewidth, Math.Abs(height)));

            // next executed
            if (adorned.ViewModel.State == EventState.Executed
                && next is not null)
            {
                var diff = next.EventInfo.TransformToVisual(adorned.EventInfo).Transform(new()).Y;
                height = Math.Abs(diff) - Math.Abs(next.EventInfo.ActualHeight - adorned.EventInfo.ActualHeight) / 2;
                y = diff < 0 ? diff + next.EventInfo.ActualHeight / 2 : adorned.EventInfo.ActualHeight / 2;
                drawingContext.DrawRectangle(RevertedBrush, null, new Rect(x, y, linewidth, Math.Abs(height)));
            }
        }
    }
}