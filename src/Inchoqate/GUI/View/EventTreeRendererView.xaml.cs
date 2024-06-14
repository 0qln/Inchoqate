using Inchoqate.GUI.Model.Events;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for EventTreeNodeView.xaml
    /// </summary>
    public partial class EventTreeRendererView : UserControl
    {
        public static readonly DependencyProperty InitialEventProperty =
            DependencyProperty.Register(
                "InitialEvent",
                typeof(EventModel),
                typeof(EventTreeRendererView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ConnectorBrushPrevHProperty =
            DependencyProperty.Register(
                "ConnectorBrushPrevH",
                typeof(Brush),
                typeof(EventTreeRendererView),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Colors.White),
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public EventModel InitialEvent
        {
            get => (EventModel)GetValue(InitialEventProperty);
            set => SetValue(InitialEventProperty, value);
        }

        public Brush ConnectorBrushPrevH
        {
            get => (Brush)GetValue(ConnectorBrushPrevHProperty);
            set => SetValue(ConnectorBrushPrevHProperty, value);
        }


        public EventTreeRendererView()
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


    public class EventTreeNextNodesConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new ObservableCollection<EventTreeRendererView>();
            if (value is null) { return result; }
            var @event = (EventModel)value;
            foreach (var nextEvent in @event.Next)
            {
                result.Add(new EventTreeRendererView { InitialEvent = nextEvent.Value });
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EventArgsInfoConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new ObservableCollection<string>();
            if (value is null) { return result; }
            foreach (var arg in value.GetType().GetProperties())
            {
                string argN = arg.Name;
                if (argN == "Parameter")
                    continue;
                string argV = arg.GetValue(value)?.ToString() ?? "";
                result.Add($"{argN}: {argV}");
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class NodeConnectorAdorner(EventTreeRendererView adorned) : Adorner(adorned.EventInfo)
    {
        public static readonly DependencyProperty RevertedBrushProperty =
            DependencyProperty.Register(
                "RevertedBrush",
                typeof(Brush),
                typeof(NodeConnectorAdorner),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ExecutedBrushProperty =
            DependencyProperty.Register(
                "ExecutedBrush",
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
            if (adorned.InitialEvent is null)
            {
                return;
            }

            double adjust = 1, linewidth = 2;
            double x, y, width, height;
            var stackpanel = Utils.FindVisualChildOfType<StackPanel>(adorned.NextNodes);
            var next = stackpanel.Children.FirstOrDefault<EventTreeRendererView>(e => e.InitialEvent.State == EventState.Executed);

            y = adorned.EventInfo.ActualHeight / 2;

            // prev h
            if (adorned.InitialEvent.Previous is not null)
            {
                x = -adorned.EventInfo.Margin.Left;
                width = adorned.EventInfo.Margin.Left;
                drawingContext.DrawRectangle(
                    adorned.InitialEvent.State == EventState.Executed ? ExecutedBrush : RevertedBrush,
                    null,
                    new Rect(x - adjust, y, width + adjust, linewidth));
            }

            // next h
            if (adorned.InitialEvent.Next.Count > 0)
            {
                x = adorned.EventInfo.ActualWidth;
                width = adorned.EventInfo.Margin.Right;
                drawingContext.DrawRectangle(
                    next is not null ? ExecutedBrush : RevertedBrush,
                    null,
                    new Rect(x, y, width + adjust, linewidth));
            }

            // next v
            if (adorned.InitialEvent.Next.Count > 1)
            {
                var top = (EventTreeRendererView)stackpanel.Children[0];
                var bottom = (EventTreeRendererView)stackpanel.Children[^1];
                var span = top.EventInfo.TransformToVisual(bottom.EventInfo).Transform(new()).Y;

                // next reverted
                x = adorned.EventInfo.ActualWidth + adorned.EventInfo.Margin.Left;
                y = -Math.Abs(top.EventInfo.TransformToVisual(adorned.EventInfo).Transform(new()).Y) + top.EventInfo.ActualHeight / 2;
                height = Math.Abs(span) - Math.Abs(top.EventInfo.ActualHeight - bottom.EventInfo.ActualHeight) / 2;
                drawingContext.DrawRectangle(RevertedBrush, null, new Rect(x, y, linewidth, height));

                // next executed
                if (adorned.InitialEvent.State == EventState.Executed
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
}
