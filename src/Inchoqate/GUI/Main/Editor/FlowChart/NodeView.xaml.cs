using Inchoqate.GUI.Titlebar;
using Miscellaneous;
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

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    public partial class NodeOptionCollection : ObservableCollection<Control>
    {
    }


    public class NodeCollection : ObservableCollection<NodeView>
    {
    }


    /// <summary>
    /// Makes a connection from the output of a node to the input of another node.
    /// </summary>
    public class NodeConnectionAdorner : Adorner
    {
        public NodeConnectionAdorner(NodeView adornedElement)
            : base(adornedElement)
        {
        }

        // TODO: Bind colors to color theme
        protected override void OnRender(DrawingContext drawingContext)
        {
            NodeView @this = (NodeView)AdornedElement;

            static double yMin(NodeView node) => node.Margin.Top;
            static double yMax(NodeView node) => node.ActualHeight - node.Margin.Bottom - yMin(node);
            static double yPos(NodeView node, int i, int iMax) => Utils.Lerp(yMin(node), yMax(node), (i + 0.5) / iMax);

            // Adapters
            var brush = Brushes.White;
            var pen = new Pen(brush, 0);
            var r = 5.0;

            // Connectors
            var brushCon = Brushes.Transparent;
            var penCon = new Pen(Brushes.Gray, 1);

            // Draw outputs
            var count = @this.Outputs.Count;
            for (int i = 0; i < count; i++)
            {
                var xFrom = (double)@this.ActualWidth;
                var yFrom = yPos(@this, i, count);
                var next = @this.Outputs[i] as NodeView;
                var xTo = Canvas.GetLeft(next) - Canvas.GetLeft(@this);
                var yTo = Canvas.GetTop(next) - Canvas.GetTop(@this) + yPos(next!, next!.Inputs.IndexOf(@this), next!.Inputs.Count);
                var path = Geometry.Parse($"M {xFrom},{yFrom} C {xTo},{yFrom} {xFrom},{yTo} {xTo},{yTo}");
                drawingContext.DrawGeometry(brushCon, penCon, path);
                drawingContext.DrawEllipse(brush, pen, new Point(xFrom, yFrom), r, r);
                drawingContext.DrawEllipse(brush, pen, new Point(xTo, yTo), r, r);
            }
        }
    }


    /// <summary>
    /// Interaction logic for FlowChartNode.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(NodeView));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(NodeOptionCollection), typeof(NodeView));

        public NodeOptionCollection Options
        {
            get => (NodeOptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }


        public static readonly DependencyProperty InputsProperty = DependencyProperty.Register(
            "Inputs", typeof(NodeCollection), typeof(NodeView));

        public NodeCollection Inputs
        {
            get => (NodeCollection)GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }


        public static readonly DependencyProperty OutputsProperty = DependencyProperty.Register(
            "Outputs", typeof(NodeCollection), typeof(NodeView));

        public NodeCollection Outputs
        {
            get => (NodeCollection)GetValue(OutputsProperty);
            set => SetValue(OutputsProperty, value);
        }


        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register(
            "IsDragging", typeof(bool), typeof(NodeView));

        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            set => SetValue(IsDraggingProperty, value);
        }


        public event EventHandler? Dragged;


        private Point _dragBegin;

        private Adorner? _adorner;


        public NodeView()
        {
            InitializeComponent();

            Outputs = [];
            Inputs = [];

            Loaded += Node_Loaded;

            Canvas.SetLeft(this, 10);
            Canvas.SetTop(this, 10);
        }

        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this).Add(
                _adorner = new NodeConnectionAdorner(this)
                {
                    Margin = new Thickness
                    {
                        Top = 15,
                        Bottom = 15
                    }
                }
            );
        }


        public virtual void SetNext(NodeView next)
        {
            this.Outputs.Add(next);
            next.Inputs.Add(this);
            next.Dragged += delegate
            {
                this._adorner?.InvalidateVisual();
                next._adorner?.InvalidateVisual();
            };
            this.Dragged += delegate
            {
                this._adorner?.InvalidateVisual();
                next._adorner?.InvalidateVisual();
            };
            this._adorner?.InvalidateVisual();
        }


        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);

            Dragged?.Invoke(this, EventArgs.Empty);
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

        }
    }
}
