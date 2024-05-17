using Inchoqate.GUI.Main.Editor.Panel;
using Inchoqate.GUI.Titlebar;
using Microsoft.Extensions.Logging;
using Miscellaneous;
using Miscellaneous.Logging;
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


    public enum AdapterType
    {
        Input,
        Output
    }


    public class NodeAdapterAdorner(NodeView adornedElement, AdapterType type) : Adorner(adornedElement)
    {
        private readonly NodeView _thisNode = adornedElement;

        public readonly List<Point> Points = [];

        public AdapterType Type = type;


        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Brush), typeof(NodeAdapterAdorner), new(Brushes.White));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty); 
            set => SetValue(ColorProperty, value);
        }


        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(NodeAdapterAdorner), new(5.0));

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }


        public static readonly DependencyProperty OffTopProperty = DependencyProperty.Register(
            "OffTop", typeof(double), typeof(NodeAdapterAdorner), new(5.0));

        public double OffTop
        {
            get => (double)GetValue(OffTopProperty);
            set => SetValue(OffTopProperty, value);
        }


        public static readonly DependencyProperty OffBottomProperty = DependencyProperty.Register(
            "OffBottom", typeof(double), typeof(NodeAdapterAdorner), new(5.0));

        public double OffBottom
        {
            get => (double)GetValue(OffBottomProperty);
            set => SetValue(OffBottomProperty, value);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            double x = Type == AdapterType.Input ? 0.0 : _thisNode.ActualWidth;
            double yMin = OffBottom;
            double yMax = _thisNode.ActualHeight - OffBottom - OffTop;
            Pen pen = new(Color, 0.0);

            Points.Clear();
            var count = Type == AdapterType.Input ? _thisNode.Inputs.Count : _thisNode.Outputs.Count;
            for (int i = 0; i < count; i++)
            {
                double t = (i + 0.5) / count;
                double y = Utils.Lerp(yMin, yMax, t);
                Points.Add(new Point(x, y));
                drawingContext.DrawEllipse(Color, pen, Points[i], Radius, Radius);
            }
        }
    }


    /// <summary>
    /// Draws a connection from the output of a node to the input of another node.
    /// </summary>
    public class NodeConnectionAdorner(NodeView fromNode, NodeView toNode) : Adorner(fromNode)
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<NodeConnectionAdorner>();

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Brush), typeof(NodeConnectionAdorner), new PropertyMetadata(Brushes.Gray));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {            
            Pen penCon = new(Color, 1);
            

            if (fromNode.OutputAdapters is null || toNode.InputAdapters is null)
            {
                _logger.LogWarning(
                    "Tried to render a connection adorner before " +
                    "one of the inputs/outputs adorners were created.");
                return;
            }

            for (int i = 0, j = 0; i < fromNode.OutputAdapters!.Points.Count && j < toNode.InputAdapters!.Points.Count; i++, j++)
            {
                Point from  = fromNode.OutputAdapters!.Points[i];
                Point to    = Point.Add(toNode.InputAdapters!.Points[j], toNode.CanvasPosition - fromNode.CanvasPosition);
                var path = Geometry.Parse($"M {from.X},{from.Y} C {to.X},{from.Y} {from.X},{to.Y} {to.X},{to.Y}");
                drawingContext.DrawGeometry(Brushes.Transparent, penCon, path);
            }
        }
    }


    /// <summary>
    /// Interaction logic for FlowChartNode.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {
        public bool SelectionMode { get; private set; }

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


        private NodeAdapterAdorner? _inputsAdorner, _outputsAdorner;
        private List<NodeConnectionAdorner> _connectionAdorners = [];

        public NodeAdapterAdorner? InputAdapters => _inputsAdorner;
        public NodeAdapterAdorner? OutputAdapters => _outputsAdorner;

        public Point CanvasPosition
        {
            get
            {
                return new Point
                {
                    X = Canvas.GetLeft(this),
                    Y = Canvas.GetTop(this)
                };
            }
        }


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
                _inputsAdorner = new(this, AdapterType.Input)
                {
                    OffTop = 15,
                    OffBottom = 15
                }
            );
            AdornerLayer.GetAdornerLayer(this).Add(
                _outputsAdorner = new(this, AdapterType.Output)
                {
                    OffTop = 15,
                    OffBottom = 15
                }
            );
        }


        public virtual void SetNext(NodeView next)
        {
            next.Inputs.Add(this);
            this.Outputs.Add(next);

            NodeConnectionAdorner connection = new(this, next);
            _connectionAdorners.Add(connection);
            AdornerLayer.GetAdornerLayer(this).Add(connection);
            next.Dragged += (_, _) => connection.InvalidateVisual();
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

        private void MenuItem_AddOutput_Click(object sender, RoutedEventArgs e)
        {
            SelectionMode = true;
            this._inputsAdorner?.InvalidateVisual();
        }
    }
}
