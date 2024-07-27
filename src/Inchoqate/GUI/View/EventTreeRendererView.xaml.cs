using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.ViewModel;
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
        // make readonly
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(EventViewModel),
                typeof(EventTreeRendererView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnEventChanged));

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (EventTreeRendererView)d;
            @this.UpdateNextNodes();
        }

        // make readonly
        public static readonly DependencyProperty EventTreeProperty =
            DependencyProperty.Register(
                nameof(EventTree),
                typeof(EventTreeViewModel),
                typeof(EventTreeRendererView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    OnEventTreeChanged));

        private static void OnEventTreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (EventTreeRendererView)d;
            @this.UpdateNextNodes();
            @this.EventTree.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(@this.EventTree.Current):
                        Debug.Assert(@this.EventTree.Current is EventViewModel);
                        if (@this.ViewModel.EqualsInner((EventViewModel)@this.EventTree.Current.Previous))
                        {
                            @this.UpdateNextNodes();
                        }
                        break;
                }
            };
        }


        public static readonly DependencyProperty NextNodesSourceProperty = 
            DependencyProperty.Register(
                nameof(NextNodesSource),
                typeof(ObservableCollectionBase<EventTreeRendererView>),
                typeof(EventTreeRendererView),
                new PropertyMetadata(null));


        private void UpdateNextNodes()
        {
            // Lazily update next nodes

            if (ViewModel is null || EventTree is null)
                return;

            NextNodesSource ??= [];

            foreach (var view in NextNodesSource)
            {
                foreach (var viewModel in ViewModel.Next.Values)
                {
                    Debug.Assert(viewModel is EventViewModel);

                    if (((EventViewModel)viewModel).EqualsInner(view.ViewModel)) 
                    {
                        NextNodesSource.Remove(view);
                    }
                }
            }

            foreach (var viewModel in ViewModel.Next.Values.Except(NextNodesSource.Select(x => x.ViewModel)))
            {
                NextNodesSource.Add(new EventTreeRendererView
                {
                    EventTree = EventTree,
                    ViewModel = new EventViewModel((EventModel)viewModel, viewModel.ToString()!),
                });
            }

            _adorner?.InvalidateVisual();
        }



        public EventViewModel ViewModel
        {
            get => (EventViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public EventTreeViewModel EventTree
        {
            get => (EventTreeViewModel)GetValue(EventTreeProperty);
            set => SetValue(EventTreeProperty, value);
        }

        public ObservableCollectionBase<EventTreeRendererView> NextNodesSource
        {
            get => (ObservableCollectionBase<EventTreeRendererView>)GetValue(NextNodesSourceProperty);
            set => SetValue(NextNodesSourceProperty, value);
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


    public class EventArgsInfoConverter : IValueConverter
    {
        object IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var result = new ObservableCollection<string>();
            if (value is null) { return result; }
            var vm = (EventViewModel)value;
            foreach (var arg in typeof(EventModel)
                .GetProperties()
                .Where(prop => prop
                    .GetCustomAttributes(true)
                    .OfType<ViewProperty>()
                    .Any()))
            {
                 var argN = arg.Name;
                 var argV = vm.GetModelPropertyValue(arg)?.ToString() ?? "";
                 result.Add($"{argN}: {argV}");
            }
            return result;
        }

        object IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class NodeConnectorAdorner(EventTreeRendererView adorned) : Adorner(adorned.EventInfo)
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
            {
                return;
            }

            double adjust = 1, linewidth = 2;
            double x, y, width, height;
            var stackpanel = Utils.FindVisualChildOfType<StackPanel>(adorned.NextNodes);
            var next = stackpanel.Children.FirstOrDefault<EventTreeRendererView>(e => e.ViewModel.State == EventState.Executed);

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
                var top = (EventTreeRendererView)stackpanel.Children[0];
                var bottom = (EventTreeRendererView)stackpanel.Children[^1];
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
}
