// TODO: some nicer UI than the context menu
#undef ImprovedUI

using Inchoqate.GUI.Main.Editor.Panel;
using Inchoqate.GUI.Titlebar;
using Inchoqate.Miscellaneous;
using Microsoft.Extensions.Logging;
using Miscellaneous;
using Miscellaneous.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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


        public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register(
            "IconColor", typeof(Brush), typeof(NodeAdapterAdorner), new(Brushes.Gray));

        public Brush IconColor
        {
            get => (Brush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }


        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(NodeAdapterAdorner), new(5.0));

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }


        public static readonly DependencyProperty OffTopProperty = DependencyProperty.Register(
            "OffTop", typeof(double), typeof(NodeAdapterAdorner), new(0.0));

        public double OffTop
        {
            get => (double)GetValue(OffTopProperty);
            set => SetValue(OffTopProperty, value);
        }


        public static readonly DependencyProperty OffBottomProperty = DependencyProperty.Register(
            "OffBottom", typeof(double), typeof(NodeAdapterAdorner), new(0.0));

        public double OffBottom
        {
            get => (double)GetValue(OffBottomProperty);
            set => SetValue(OffBottomProperty, value);
        }

#if ImprovedUI
        public static readonly DependencyProperty DrawAddIconProperty = DependencyProperty.Register(
            "DrawAddIcon", typeof(bool), typeof(NodeAdapterAdorner), new(true));

        public bool DrawAddIcon
        {
            get => (bool)GetValue(DrawAddIconProperty);
            set => SetValue(DrawAddIconProperty, value);
        }
#endif

        private Button? IconButton;


        protected override void OnRender(DrawingContext drawingContext)
        {
            double x = Type == AdapterType.Input ? 0.0 : _thisNode.ActualWidth;
            double yMin = OffTop;
            double yMax = _thisNode.ActualHeight - OffBottom;
            Pen pen = new(Color, 0.0);

            int adapters = Type == AdapterType.Input ? _thisNode.Inputs.Count : _thisNode.Outputs.Count;
#if ImprovedUI
            int icons = DrawAddIcon ? 1 : 0;
            int count = adapters + icons;
#else
            int count = adapters;
#endif

            Points.Clear();
            
            for (int i = 0; i < adapters; i++)
            {
                double t = (i + 0.5) / count;
                double y = Utils.Lerp(yMin, yMax, t);
                Points.Add(new(x, y));
                drawingContext.DrawEllipse(Color, pen, Points[i], Radius, Radius);
                var g = Geometry.Parse("");
            }


#if ImprovedUI
            if (DrawAddIcon)
            {
                double size1 = 2;
                double size2 = Radius + Radius / 2;
                int i = adapters;
                double t = (i + 0.5) / count;
                double y = Utils.Lerp(yMin, yMax, t);
                drawingContext.DrawEllipse(Color, pen, new (x, y), Radius, Radius);
                drawingContext.DrawRoundedRectangle(
                    IconColor, 
                    new(IconColor, 0.0), 
                    new Rect
                    { 
                        X = x - size1 / 2,
                        Y = y - size2 / 2,
                        Width = size1,
                        Height = size2
                    },
                    1, 
                    1);
                drawingContext.DrawRoundedRectangle(
                    IconColor,
                    new(IconColor, 0.0),
                    new Rect
                    {
                        X = x - size2 / 2,
                        Y = y - size1 / 2,
                        Width = size2,
                        Height = size1
                    },
                    1,
                    1);
            }
#endif
        }
    }


    ///// <summary>
    ///// Draws a connection from the output of a node to the input of another node.
    ///// </summary>
    //public class NodeConnectionAdorner(NodeView fromNode) : Adorner(fromNode)
    //{
    //    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<NodeConnectionAdorner>();


    //    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    //        "Color", typeof(Brush), typeof(NodeConnectionAdorner), new (Brushes.Gray));

    //    public Brush Color
    //    {
    //        get => (Brush)GetValue(ColorProperty);
    //        set => SetValue(ColorProperty, value);
    //    }


    //    protected override void OnRender(DrawingContext drawingContext)
    //    {            
    //        Pen penCon = new(Color, 1);

    //        int toAdapt, fromAdapt = 0;
    //        foreach (var toNode in fromNode.Outputs)
    //        {
    //            if (fromNode.OutputAdapters is null || toNode.InputAdapters is null)
    //            {
    //                _logger.LogWarning(
    //                    "Tried to render a connection adorner before " +
    //                    "one of the inputs/outputs adorners were created.");
    //                continue;
    //            }

    //            for (toAdapt = 0; 
    //                fromAdapt < fromNode.OutputAdapters!.Points.Count && toAdapt < toNode.InputAdapters!.Points.Count; fromAdapt++, toAdapt++)
    //            {
    //                Point from  = fromNode.OutputAdapters!.Points[fromAdapt];
    //                Point to    = Point.Add(toNode.InputAdapters!.Points[toAdapt], toNode.CanvasPosition - fromNode.CanvasPosition);
    //                var path = Geometry.Parse($"M {from.X},{from.Y} C {to.X},{from.Y} {from.X},{to.Y} {to.X},{to.Y}");
    //                drawingContext.DrawGeometry(Brushes.Transparent, penCon, path);
    //            }
    //        }
    //    }
    //}


    //public class ConnectionAdorner(UIElement adornedNode) : Adorner(adornedNode)
    //{
    //    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<ConnectionAdorner>();


    //    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    //        "Color", typeof(Brush), typeof(ConnectionAdorner), new(Brushes.Gray));

    //    public Brush Color
    //    {
    //        get => (Brush)GetValue(ColorProperty);
    //        set => SetValue(ColorProperty, value);
    //    }


    //    public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
    //        "From", typeof(NodeView), typeof(ConnectionAdorner));

    //    public NodeView From
    //    {
    //        get => (NodeView)GetValue(FromProperty);
    //        set => SetValue(FromProperty, value);
    //    }


    //    public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
    //        "To", typeof(NodeCollection), typeof(ConnectionAdorner));

    //    public NodeCollection To
    //    {
    //        get => (NodeCollection)GetValue(ToProperty);
    //        set => SetValue(ToProperty, value);
    //    }


    //    public static readonly DependencyProperty CursorConnectionProperty = DependencyProperty.Register(
    //        "CursorConnection", typeof(bool), typeof(ConnectionAdorner));

    //    public bool CursorConnection
    //    {
    //        get => (bool)GetValue(CursorConnectionProperty);
    //        set => SetValue(CursorConnectionProperty, value);
    //    }


    //    public static ConnectionAdorner ConnectNodes(Canvas container, NodeView from, NodeCollection to)
    //    {
    //        var output = new ConnectionAdorner(container)
    //        {
    //            //From = from,
    //            //To = to
    //        };

    //        output.SetBinding(FromProperty, new Binding { Source = from } );
    //        output.SetBinding(ToProperty, new Binding { Source = to });

    //        return output;
    //    }


    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        Pen pen = new(Color, 1);

    //        for (int i = 0; i < From.Outputs.Count; i++)
    //        {
    //            var toNode = From.Outputs[i];
    //            for (int j = 0; j < toNode.Inputs.Count; j++)
    //            {
    //                if (toNode.Inputs[j] != From)
    //                {
    //                    continue;
    //                }

    //                // Hasn't loaded yet
    //                if (From.OutputAdapters is null ||
    //                    From.Outputs.Count != From.OutputAdapters.Points.Count ||
    //                    toNode.InputAdapters is null ||
    //                    toNode.Inputs.Count != toNode.InputAdapters.Points.Count)
    //                {
    //                    continue;
    //                }

    //                Point from = Point.Add(From.OutputAdapters.Points[i], (Vector)From.CanvasPosition);
    //                Point to = Point.Add(toNode.InputAdapters.Points[j], (Vector)toNode.CanvasPosition);
    //                var path = Geometry.Parse($"M {from.X},{from.Y} C {to.X},{from.Y} {from.X},{to.Y} {to.X},{to.Y}");
    //                drawingContext.DrawGeometry(Brushes.Transparent, pen, path);
    //            }
    //        }

    //        if (CursorConnection)
    //        {

    //        }
    //    }
    //}


    //[ValueConversion()]
    //public class ConnectionAdapterSelector : IValueConverter
    //{

    //}


    //public class AdapterConnectorAdorner(Canvas e) : Adorner(e)
    //{
    //    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    //        "Color", typeof(Brush), typeof(AdapterConnectorAdorner), new(Brushes.Gray));

    //    public Brush Color
    //    {
    //        get => (Brush)GetValue(ColorProperty);
    //        set => SetValue(ColorProperty, value);
    //    }


    //    public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
    //        "From", typeof(NodeView), typeof(AdapterConnectorAdorner));

    //    public NodeView From
    //    {
    //        get => (NodeView)GetValue(FromProperty);
    //        set => SetValue(FromProperty, value);
    //    }


    //    public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
    //        "To", typeof(NodeView), typeof(AdapterConnectorAdorner));

    //    public NodeView To
    //    {
    //        get => (NodeView)GetValue(ToProperty);
    //        set => SetValue(ToProperty, value);
    //    }



    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        Point souce, dest;



    //        Pen pen = new(Color, 1);
    //        var path = Geometry.Parse($"M {souce.X},{souce.Y} C {dest.X},{souce.Y} {souce.X},{dest.Y} {dest.X},{dest.Y}");
    //        drawingContext.DrawGeometry(Brushes.Transparent, pen, path);
    //    }
    //}


    public class ObservableItemCollection<T> : ObservableCollection<T> 
        where T : INotifyPropertyChanged
    {
        public ObservableItemCollection() : base()
        {
            CollectionChanged += ObservableItemCollection_CollectionChanged;
        }

        private void ObservableItemCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // The entire collection could be changed, thus we pass the 'Reset' flag.
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }
    }


    //public class NodeViewAdapter : INotifyPropertyChanged
    //{
    //    private NodeView _nodeView;

    //    public NodeView 

    //    private Point _loc;

    //    public Point Location
    //    {

    //    }


    //    public NodeViewAdapter(Point location, NodeView node)
    //    {
    //        _loc = location;

    //    }


    //    public static implicit operator (Point Location, NodeView Node)(NodeViewAdapter value)
    //    {
    //        return (value.Location, value.Node);
    //    }

    //    public static implicit operator NodeViewAdapter((Point Location, NodeView Node) value)
    //    {
    //        return new NodeViewAdapter(value.Location, value.Node);
    //    }

    //    public event PropertyChangedEventHandler? PropertyChanged;

    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}


    public class ObservablePointPair : INotifyPropertyChanged
    {
        private Point _p1, _p2;

        public Point Value1
        {
            get => _p1;
            set
            {
                if (_p1 != value)
                {
                    _p1 = value;
                    OnPropertyChanged(nameof(Value1));
                }
            }
        }

        public Point Value2
        {
            get => _p2;
            set
            {
                if (_p2 != value)
                {
                    _p2 = value;
                    OnPropertyChanged(nameof(Value1));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class ConnectionsAdorner(Canvas adorned) : Adorner(adorned)
    {
        public static readonly DependencyProperty InfoColorProperty =
            DependencyProperty.Register(
                "InfoColor",
                typeof(Brush),
                typeof(ConnectionsAdorner),
                new FrameworkPropertyMetadata(
                    Brushes.Gray,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush InfoColor
        {
            get => (Brush)GetValue(InfoColorProperty);
            set => SetValue(InfoColorProperty, value);
        }


        public static readonly DependencyProperty ErrorColorProperty =
            DependencyProperty.Register(
                "ErrorColor",
                typeof(Brush),
                typeof(ConnectionsAdorner),
                new FrameworkPropertyMetadata(
                    Brushes.Red,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ErrorColor
        {
            get => (Brush)GetValue(ErrorColorProperty);
            set => SetValue(ErrorColorProperty, value);
        }

        public static readonly DependencyProperty OutputAdaptersProperty =
            DependencyProperty.Register(
                "OutputAdapters",
                typeof(ObservableItemCollection<ObservablePointPair>),
                typeof(ConnectionsAdorner),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public ObservableItemCollection<ObservablePointPair> OutputAdapters
        {
            get => (ObservableItemCollection<ObservablePointPair>)GetValue(OutputAdaptersProperty);
            set => SetValue(OutputAdaptersProperty, value);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            foreach (var source in OutputAdapters)
            {
                Point lSource = source.Value1;
                Point lDest = source.Value2;

                Pen pen = new(InfoColor, 1);
                var path = Geometry.Parse($"" +
                    $"M {lSource.X},{lSource.Y} " +
                    $"C {lDest.X},{lSource.Y} {lSource.X},{lDest.Y} {lDest.X},{lDest.Y}");
                drawingContext.DrawGeometry(Brushes.Transparent, pen, path);
            }
        }
    }


    //public class ConnectionAdorner(Canvas adorned) : Adorner(adorned)
    //{
    //    public static readonly DependencyProperty InfoColorProperty =
    //        DependencyProperty.Register(
    //            "InfoColor",
    //            typeof(Brush),
    //            typeof(ConnectionAdorner),
    //            new FrameworkPropertyMetadata(
    //                Brushes.Gray,
    //                FrameworkPropertyMetadataOptions.AffectsRender));

    //    public Brush InfoColor
    //    {
    //        get => (Brush)GetValue(InfoColorProperty);
    //        set => SetValue(InfoColorProperty, value);
    //    }


    //    public static readonly DependencyProperty ErrorColorProperty =
    //        DependencyProperty.Register(
    //            "ErrorColor",
    //            typeof(Brush),
    //            typeof(ConnectionAdorner),
    //            new FrameworkPropertyMetadata(
    //                Brushes.Red,
    //                FrameworkPropertyMetadataOptions.AffectsRender));

    //    public Brush ErrorColor
    //    {
    //        get => (Brush)GetValue(ErrorColorProperty);
    //        set => SetValue(ErrorColorProperty, value);
    //    }


    //    public static readonly DependencyProperty SourceDestinationPairProperty =
    //        DependencyProperty.Register(
    //            "SourceDestinationPair",
    //            typeof(ObservablePointPair),
    //            typeof(ConnectionAdorner),
    //            new FrameworkPropertyMetadata(
    //                default,
    //                FrameworkPropertyMetadataOptions.AffectsRender));

    //    public ObservablePointPair SourceDestinationPair
    //    {
    //        get => (ObservablePointPair)GetValue(SourceDestinationPairProperty);
    //        set => SetValue(SourceDestinationPairProperty, value);
    //    }


    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        Point lSource = SourceDestinationPair.Value1;
    //        Point lDest = SourceDestinationPair.Value2;

    //        Pen pen = new(InfoColor, 1);
    //        var path = Geometry.Parse($"" +
    //            $"M {lSource.X},{lSource.Y} " +
    //            $"C {lDest.X},{lSource.Y} {lSource.X},{lDest.Y} {lDest.X},{lDest.Y}");
    //        drawingContext.DrawGeometry(Brushes.Transparent, pen, path);
    //    }
    //}

    ///// <summary>
    ///// Draws a connection from the output of a node to the cursor.
    ///// </summary>
    //public class CursorConnectionAdorner(NodeView fromNode, int adapterIndex) : Adorner(fromNode)
    //{
    //    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<CursorConnectionAdorner>();


    //    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    //        "Color", typeof(Brush), typeof(CursorConnectionAdorner), new PropertyMetadata(Brushes.Gray));

    //    public Brush Color
    //    {
    //        get => (Brush)GetValue(ColorProperty);
    //        set => SetValue(ColorProperty, value);
    //    }


    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        Pen penCon = new(Color, 1);

    //        if (fromNode.OutputAdapters is null)
    //        {
    //            _logger.LogWarning(
    //                "Tried to render a connection adorner before " + 
    //                "the outputs adorner waas created.");
    //            return;
    //        }

    //        Point from = fromNode.OutputAdapters!.Points[adapterIndex];
    //        Point to = Mouse.GetPosition(this);
    //        to.X -= 1;
    //        var path = Geometry.Parse($"M {from.X},{from.Y} C {to.X},{from.Y} {from.X},{to.Y} {to.X},{to.Y}");
    //        drawingContext.DrawGeometry(Brushes.Transparent, penCon, path);
    //    }
    //}


    //public static class ConnectionSorter
    //{
    //    public static List<(Point, Point)> GenerateConnections(List<(NodeView Out, NodeView In)> connections)
    //    {
    //        foreach ((var Out, var In) in connections)
    //        {
    //            Debug.Assert(Out.CanvasPosition.X + Out.ActualWidth <= In.CanvasPosition.X);
    //        }

    //        static double PairDistance((NodeView Out, NodeView In) pair)
    //        {
    //            Point pOut = pair.Out.CanvasPosition;
    //            pOut.X += pair.Out.ActualWidth;
    //            Point pIn = pair.In.CanvasPosition;
    //            return (pOut - pIn).Length;
    //        }

    //        // Short connections have priority.
    //        connections.Sort((pair1, pair2) =>
    //        {
    //            return PairDistance(pair1).CompareTo(PairDistance(pair2));
    //        });

    //        foreach ((var nOut, var nIn) in connections)
    //        {
    //            var adaptersOut = nOut.OutputAdapters;
    //        }
    //    }
    //}


    /// <summary>
    /// Interaction logic for FlowChartNode.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {
        // Contains the node that is selecting
        private static NodeView? _selectionMode = null;


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
            "Inputs", typeof(NodeCollection), typeof(NodeView), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsRender));

        public NodeCollection Inputs
        {
            get => (NodeCollection)GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }


        public static readonly DependencyProperty FixedInputsProperty = DependencyProperty.Register(
            "FixedInputs", typeof(bool), typeof(NodeView), new FrameworkPropertyMetadata(
                true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool FixedInputs
        {
            get => (bool)GetValue(FixedInputsProperty);
            set => SetValue(FixedInputsProperty, value);
        }


        public static readonly DependencyProperty OutputsProperty = DependencyProperty.Register(
            "Outputs", typeof(NodeCollection), typeof(NodeView), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsRender));

        public NodeCollection Outputs
        {
            get => (NodeCollection)GetValue(OutputsProperty);
            set => SetValue(OutputsProperty, value);
        }


        public static readonly DependencyProperty FixedOutputsProperty = DependencyProperty.Register(
            "FixedOutputs", typeof(bool), typeof(NodeView), new FrameworkPropertyMetadata(
                false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool FixedOutputs
        {
            get => (bool)GetValue(FixedOutputsProperty);
            set => SetValue(FixedOutputsProperty, value);
        }


        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(NodeView));

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }


        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y", typeof(double), typeof(NodeView));

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }


        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register(
            "IsDragging", typeof(bool), typeof(NodeView));

        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            set => SetValue(IsDraggingProperty, value);
        }


        public event EventHandler? Dragged;


        public IEnumerable<(Point Location, NodeView Node)> OutputAdapters
        {
            get
            {
                // Hyper parameters
                double topOffset = 15, bottomOffset = 15;

                double x = ActualWidth;
                double yMin = topOffset;
                double yMax = ActualHeight - bottomOffset;
                int count = Outputs.Count;
                for (int i = 0; i < count; i++)
                {
                    double t = (i + 0.5) / count;
                    double y = Utils.Lerp(yMin, yMax, t);
                    yield return (Point.Add(CanvasPosition, new(x, y)), Outputs[i]);
                }
            }
        }

        /// <summary>
        /// Returns a list of the input adapters, sorted wrt. height in descending order.
        /// </summary>
        public IEnumerable<Point> InputAdapters
        {
            get
            {
                // Hyper parameters
                double topOffset = 15, bottomOffset = 15;

                double x = 0;
                double yMin = topOffset;
                double yMax = ActualHeight - bottomOffset;
                int count = Inputs.Count;
                for (int i = 0; i < count; i++)
                {
                    double t = (i + 0.5) / count;
                    double y = Utils.Lerp(yMin, yMax, t);
                    yield return Point.Add(CanvasPosition, new(x, y));
                }
            }
        }

        /// <summary>
        /// Searches the input adapters for this node for the highest possible 
        /// </summary>
        /// <param name="sourceLocation"></param>
        /// <returns></returns>
        public Point PickAdapter(Point sourceLocation)
        {
            var result = Inputs
                .SelectMany(node => node.OutputAdapters)
                .Select(source => source.Location.Y)
                .Count(y => y > sourceLocation.Y);

            return InputAdapters.ElementAt(result);
        }


        //public static readonly DependencyProperty CanvasPositionProperty =
        //    DependencyProperty.Register(
        //        "CanvasPosition",
        //        typeof(Point),
        //        typeof(NodeView),
        //        new FrameworkPropertyMetadata(
        //            FrameworkPropertyMetadataOptions.AffectsRender));

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

            //this.SetBinding(
            //    ConnectionPointsProperty,
            //    new Binding("Outputs")
            //    {
            //        Source = this,
            //        Converter = new ConnectionPointsGenerator(this)
            //    });

            Loaded += Node_Loaded;

            SetBinding(Canvas.LeftProperty, new Binding("X") { Source = this, Mode = BindingMode.TwoWay, });
            SetBinding(Canvas.TopProperty, new Binding("Y") { Source = this, Mode = BindingMode.TwoWay });
            X = 10;
            Y = 10;
        }


        //[ValueConversion(typeof(NodeCollection), typeof(ObservableItemCollection<ObservablePointPair>))]
        //public class ConnectionPointsGenerator(NodeView source) : IValueConverter
        //{
        //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        ObservableItemCollection<ObservablePointPair> result = [];
        //        var outputAdapters = source.OutputAdapters;
        //        var outputsNodes = (NodeCollection)value;
        //        foreach (var adapter in outputAdapters)
        //        {
        //            result.Add(new ObservablePointPair
        //            {
        //                Value1 = adapter.Location,
        //                Value2 = adapter.Node.PickAdapter(adapter.Location)
        //            });
        //        }
        //        return result;
        //    }

        //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public class ConnectionPointsGenerator(NodeView source) : IMultiValueConverter
        {
            object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                ObservableItemCollection<ObservablePointPair> result = [];
                var outputAdapters = source.OutputAdapters;
                foreach (var adapter in outputAdapters)
                {
                    result.Add(new ObservablePointPair
                    {
                        Value1 = adapter.Location,
                        Value2 = adapter.Node.PickAdapter(adapter.Location)
                    });
                }
                return result;
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        private ConnectionsAdorner? _connections;


        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            //            AdornerLayer.GetAdornerLayer(this).Add(
            //                _inputsAdorner = new(this, AdapterType.Input)
            //                {
            //                    // TODO: Bind to corner radius
            //                    OffTop = 15,
            //                    OffBottom = 15,
            //                    // TODO: create a binding instead of just init
            //#if ImprovedUI
            //                    DrawAddIcon = !FixedInputs
            //#endif
            //                }
            //            );
            //            AdornerLayer.GetAdornerLayer(this).Add(
            //                _outputsAdorner = new(this, AdapterType.Output)
            //                {
            //                    // TODO: Bind to corner radius
            //                    OffTop = 15,
            //                    OffBottom = 15,
            //                    // TODO: create a binding instead of just init
            //#if ImprovedUI
            //                    DrawAddIcon = !FixedOutputs
            //#endif
            //                }
            //            );

            mbinding = new MultiBinding
            {
                Converter = new ConnectionPointsGenerator(this),
            };

            mbinding.Bindings.Add(new Binding("Outputs") { Source = this, });
            mbinding.Bindings.Add(new Binding("Y") { Source = this, });
            mbinding.Bindings.Add(new Binding("X") { Source = this, });

            AdornerLayer.GetAdornerLayer(this).Add(
                _connections = new ConnectionsAdorner((Canvas)Parent));
            _connections.SetBinding(
                ConnectionsAdorner.OutputAdaptersProperty, 
                mbinding
            );
        }

        private MultiBinding? mbinding;

        // debugging
        class NoopConverter : IValueConverter
        {
            object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }

            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        //[ValueConversion(typeof(NodeCollection), typeof(ObservableItemCollection<ObservablePointPair>))]
        //class OutputAdaptersConverter : IValueConverter
        //{
        //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var outputs = (NodeCollection)value;
        //        var node = (NodeView)parameter;
        //        var result = new ObservableItemCollection<ObservablePointPair>();

        //        // Hyper parameters
        //        double topOffset = 15, bottomOffset = 15;

        //        double x =  node.ActualWidth + Canvas.GetLeft(node);
        //        double yMin = topOffset;
        //        double yMax = node.ActualHeight - bottomOffset;
        //        int count = outputs.Count;
        //        for (int i = 0; i < count; i++)
        //        {
        //            double t = (i + 0.5) / count;
        //            double y = Utils.Lerp(yMin, yMax, t) + Canvas.GetTop(node);
        //            Point point = new (x, y);
        //            result.Add(
        //                new ObservablePointPair 
        //                {
        //                    Value1 = point, 
        //                    Value2 = outputs[i].PickAdapter(point) 
        //                }
        //            );
        //        }
        //        return result;
        //    }

        //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    { throw new NotImplementedException(); }
        //}



        //[ValueConversion(typeof(NodeCollection), typeof(ObservablePointPair))]
        //class OutputAdaptersConverter : IValueConverter
        //{
        //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var outputs = (NodeCollection)value;
        //        var node = (NodeView)parameter;
        //        var result = new ObservablePointPair();

        //        // Hyper parameters
        //        double topOffset = 15, bottomOffset = 15;

        //        double x = node.ActualWidth + Canvas.GetLeft(node);
        //        double yMin = topOffset;
        //        double yMax = node.ActualHeight - bottomOffset;
        //        int count = outputs.Count;
        //        for (int i = 0; i < count; i++)
        //        {
        //            double t = (i + 0.5) / count;
        //            double y = Utils.Lerp(yMin, yMax, t) + Canvas.GetTop(node);
        //            Point point = new(x, y);
        //            result.Add(new ObservablePointPair
        //                {
        //                    Value1 = point,
        //                    Value2 = outputs[i].PickAdapter(point)
        //                });
        //        }
        //        return result;
        //    }

        //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    { throw new NotImplementedException(); }
        //}



        public static readonly DependencyProperty ConnectionPointsProperty = DependencyProperty.Register(
            "ConnectionPoints", 
            typeof(ObservableItemCollection<ObservablePointPair>), 
            typeof(NodeView),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

        public ObservableItemCollection<ObservablePointPair> ConnectionPoints
        {
            get => (ObservableItemCollection<ObservablePointPair>)GetValue(ConnectionPointsProperty);
            private set => SetValue(ConnectionPointsProperty, value);
        }


        public virtual void AddNext(NodeView next)
        {
            next.Inputs.Add(this);
            this.Outputs.Add(next);

            if (mbinding is null)
            {
                Loaded += delegate
                {
                    mbinding?.Bindings.Add(new Binding("X") { Source = next });
                    mbinding?.Bindings.Add(new Binding("Y") { Source = next });
                };
            }
            else
            {
                mbinding?.Bindings.Add(new Binding("X") { Source = next });
                mbinding?.Bindings.Add(new Binding("Y") { Source = next });
            }
        }


        public virtual void RemoveNext(NodeView next)
        {
            next.Inputs.Remove(this);
            this.Outputs.Remove(next);


        }


        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
            //Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);

            X += e.HorizontalChange;
            Y += e.VerticalChange;

            Dragged?.Invoke(this, EventArgs.Empty);
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectionMode is null)
            {
                return;
            }

            //(var node, var connection) = _selectionMode.Value;
            //_selectionMode = null;
            //node.Outputs.Remove(null!);
            //AdornerLayer.GetAdornerLayer(node).Remove(connection);
            //Window.GetWindow(this).MouseMove -= (_, _) => connection.InvalidateVisual();
            //node.AddNext(this);

            //_inputsAdorner?.Points.Clear();
            //node._outputsAdorner?.Points.Clear();
            //_inputsAdorner?.InvalidateVisual();
            //node._outputsAdorner?.InvalidateVisual();
            //node._connectionAdorners?.InvalidateVisual();
        }

        private void MenuItem_AddOutput_Click(object sender, RoutedEventArgs e)
        {
            if (_selectionMode is not null)
            {
                return;
            }



            //int outputIndex = Outputs.Count;
            //Outputs.Add(null!);
            //_outputsAdorner?.InvalidateVisual();
            //CursorConnectionAdorner connection = new(this, outputIndex);
            //AdornerLayer.GetAdornerLayer(this).Add(connection);
            //Window.GetWindow(this).MouseMove += (_, _) => connection.InvalidateVisual();
            //_selectionMode = (this, connection);
        }
    }
}
