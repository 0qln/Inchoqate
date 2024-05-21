// TODO: some nicer UI than the context menu
#undef ImprovedUI

using GUI.Main.Editor;
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


    public class NodeCollection : ObservableCollection<NodeViewModel>
    {
    }


    public enum AdapterType
    {
        Input,
        Output
    }


    public class NodeAdapterAdorner(NodeViewModel adornedElement, AdapterType type) : Adorner(adornedElement)
    {
        private readonly NodeViewModel _thisNode = adornedElement;

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


    public class ConnectionAdorner(ItemsControl adorned) : Adorner(adorned)
    {
        public static readonly DependencyProperty InfoColorProperty =
            DependencyProperty.Register(
                "InfoColor",
                typeof(Brush),
                typeof(ConnectionAdorner),
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
                typeof(ConnectionAdorner),
                new FrameworkPropertyMetadata(
                    Brushes.Red,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ErrorColor
        {
            get => (Brush)GetValue(ErrorColorProperty);
            set => SetValue(ErrorColorProperty, value);
        }


        public static readonly DependencyProperty SourceDestinationPairProperty =
            DependencyProperty.Register(
                "SourceDestinationPair",
                typeof(ObservablePointPair),
                typeof(ConnectionAdorner),
                new FrameworkPropertyMetadata(
                    default,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public ObservablePointPair SourceDestinationPair
        {
            get => (ObservablePointPair)GetValue(SourceDestinationPairProperty);
            set => SetValue(SourceDestinationPairProperty, value);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            Point lSource = SourceDestinationPair.Value1;
            Point lDest = SourceDestinationPair.Value2;

            Pen pen = new(InfoColor, 1);
            var path = Geometry.Parse($"" +
                $"M {lSource.X},{lSource.Y} " +
                $"C {lDest.X},{lSource.Y} {lSource.X},{lDest.Y} {lDest.X},{lDest.Y}");
            drawingContext.DrawGeometry(Brushes.Transparent, pen, path);
        }
    }


    /// <summary>
    /// Interaction logic for FlowChartNode.xaml
    /// </summary>
    public partial class NodeViewModel : NodeView
    {
        private static readonly ILogger<NodeViewModel> _logger = FileLoggerFactory.CreateLogger<NodeViewModel>();


        // Contains the node that is selecting
        private static NodeViewModel? _selectionMode = null;


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(NodeViewModel));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(NodeOptionCollection), typeof(NodeViewModel));

        public NodeOptionCollection Options
        {
            get => (NodeOptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }


        public static readonly DependencyProperty InputsProperty = DependencyProperty.Register(
            "Inputs", typeof(NodeCollection), typeof(NodeViewModel), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsRender));

        public NodeCollection Inputs
        {
            get => (NodeCollection)GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }


        public static readonly DependencyProperty FixedInputsProperty = DependencyProperty.Register(
            "FixedInputs", typeof(bool), typeof(NodeViewModel), new FrameworkPropertyMetadata(
                true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool FixedInputs
        {
            get => (bool)GetValue(FixedInputsProperty);
            set => SetValue(FixedInputsProperty, value);
        }


        public static readonly DependencyProperty OutputsProperty = DependencyProperty.Register(
            "Outputs", typeof(NodeCollection), typeof(NodeViewModel), new FrameworkPropertyMetadata(
                null, FrameworkPropertyMetadataOptions.AffectsRender));

        public NodeCollection Outputs
        {
            get => (NodeCollection)GetValue(OutputsProperty);
            set => SetValue(OutputsProperty, value);
        }


        public static readonly DependencyProperty FixedOutputsProperty = DependencyProperty.Register(
            "FixedOutputs", typeof(bool), typeof(NodeViewModel), new FrameworkPropertyMetadata(
                false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool FixedOutputs
        {
            get => (bool)GetValue(FixedOutputsProperty);
            set => SetValue(FixedOutputsProperty, value);
        }


        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(NodeViewModel));

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }


        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y", typeof(double), typeof(NodeViewModel));

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }


        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register(
            "IsDragging", typeof(bool), typeof(NodeViewModel));

        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            set => SetValue(IsDraggingProperty, value);
        }


        public event EventHandler? Dragged;


        /// <summary>
        /// Generates the outputs adapter positions with their destination nodes, 
        /// sorted in descending order wrt. height; highest nodes being
        /// paired with highest adapters, where high ~ low Y value
        /// </summary>
        public IEnumerable<(Point Location, NodeViewModel Target)> OutputAdapters
        {
            get
            {
                // Hyper parameters
                double topOffset = 15, bottomOffset = 15;

                double yMin = topOffset;
                double yMax = ActualHeight - bottomOffset;
                Debug.Assert(yMin <= yMax);

                List<NodeViewModel> outputs = Outputs.ToList();
                outputs.Sort((n1, n2) => n1.Y.CompareTo(n2.Y));

                for (int i = 0; i < outputs.Count; i++)
                {
                    double t = (i + 0.5) / outputs.Count;
                    double x = ActualWidth;
                    double y = Utils.Lerp(yMin, yMax, t);
                    yield return (new(X + x, Y + y), outputs[i]);
                }
            }
        }

        /// <summary>
        /// Generates the input adapter positions with their source nodes, 
        /// sorted in descending order wrt. height; highest nodes being
        /// paired with highest adapters, where high ~ low Y value ~ top most
        /// </summary>
        public IEnumerable<(Point Locaiton, NodeViewModel Target)> InputAdapters
        {
            get
            {
                // Hyper parameters
                double topOffset = 15, bottomOffset = 15;

                double yMin = topOffset;
                double yMax = ActualHeight - bottomOffset;
                Debug.Assert(yMin <= yMax);

                List<NodeViewModel> inputs = Inputs.ToList();
                inputs.Sort((n1, n2) => n1.Y.CompareTo(n2.Y));
                // (Lowest Y value / highest node) should be at index 0 now.
                Debug.Assert(inputs[0] == Enumerable.MinBy(inputs.ToArray(), n => n.Y));

                for (int i = 0; i < inputs.Count; i++)
                {
                    double t = (i + 0.5) / inputs.Count;
                    double x = 0;
                    double y = Utils.Lerp(yMin, yMax, t);
                    yield return (new(X + x, Y + y), inputs[i]);
                }
            }
        }


        #region Adapter Selection system

        /*
         * 
         * The two functions will ping pong upwards until no higher adapter can be found, 
         * at which point that part of the tree is solved, and the system continues to search
         * for the next highest connection until that connection matches the requested 
         * input/output adapter. The match is returned and the search can be aborted.
         * 
         */

        ///// <summary>
        ///// Returns a fitting output adapter for the <paramref name="inputAdapter"/>.
        ///// </summary>
        ///// <param name="inputAdapter"></param>
        ///// <returns></returns>
        //public Point PickOutputAdapter(Point inputAdapter, List<(NodeView Node, int Index, bool IsOutputAdapterElseInputAdapter)>? cache = null)
        //{
        //    cache ??= [];


        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="outputAdapter"></param>
        ///// <returns></returns>
        //public Point PickInputAdapter(
        //    Point outputAdapter, 
        //    Dictionary<
        //        (NodeView Node, int Index, bool IsOutputAdapterElseInputAdapter), 
        //        (NodeView Node, int Index, bool IsOutputAdapterElseInputAdapter)>? cache = null)
        //{
        //    cache ??= [];


        //}

        private List<ObservablePointPair> inputConnections = [];

        public void RegisterInputAdapter(Point connectionOutputLocation, out ObservablePointPair result)
        {
            result = new ObservablePointPair
            {
                Value1 = connectionOutputLocation,
                Value2 = InputAdapters.First(adapter =>
                {
                    // TODO
                    return true;
                }).Locaiton,
            };

            inputConnections.Add(result);
        }

        #endregion


        public NodeViewModel()
        {
            Outputs = [];
            Inputs = [];

            Loaded += Node_Loaded;

            SetBinding(
                Canvas.LeftProperty, 
                new Binding("X") 
                { 
                    Source = this, 
                    Mode = BindingMode.TwoWay, 
                }
            );
            SetBinding(
                Canvas.TopProperty, 
                new Binding("Y") 
                { 
                    Source = this, 
                    Mode = BindingMode.TwoWay 
                }
            );
            X = 10;
            Y = 10;
        }


        public class SingleConnectionPointsGenerator(NodeViewModel source, Func<int> nextNodeIndex) : IMultiValueConverter
        {
            object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var outputAdapters = source.OutputAdapters;
                var destinationIndex = nextNodeIndex.Invoke();
                if (destinationIndex == -1)
                {
                    _logger.LogWarning(
                        "An attempt was made to get the connection point locations " +
                        "of an unconnected node.");
                    return new ObservablePointPair();
                }
                var adapter = outputAdapters.ElementAt(nextNodeIndex.Invoke());
                adapter.Target.RegisterInputAdapter(adapter.Location, out var result);
                return result;
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        private List<ConnectionAdorner> _connections = [];
        private NodeAdapterAdorner? _inputsAdorner, _outputsAdorner;


        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this).Add(
                _inputsAdorner = new(this, AdapterType.Input)
                {
                    // TODO: Bind to corner radius
                    OffTop = 15,
                    OffBottom = 15,
                    // TODO: create a binding instead of just init
#if ImprovedUI
                                DrawAddIcon = !FixedInputs
#endif
                }
            );
            AdornerLayer.GetAdornerLayer(this).Add(
                _outputsAdorner = new(this, AdapterType.Output)
                {
                    // TODO: Bind to corner radius
                    OffTop = 15,
                    OffBottom = 15,
                    // TODO: create a binding instead of just init
#if ImprovedUI
                                DrawAddIcon = !FixedOutputs
#endif
                }
            );
        }


        public virtual void AddNext(NodeViewModel next)
        {
            next.Inputs.Add(this);
            this.Outputs.Add(next);

            var connection = new ConnectionAdorner((ItemsControl)Parent);
            _connections.Add(connection);

            var mbinding = new MultiBinding
            {
                Converter = new SingleConnectionPointsGenerator(
                    this, () => _connections.IndexOf(connection)),
            };

            mbinding.Bindings.Add(new Binding("Outputs") { Source = this, });
            mbinding.Bindings.Add(new Binding("Inputs") { Source = next });
            mbinding.Bindings.Add(new Binding("Y") { Source = this, });
            mbinding.Bindings.Add(new Binding("X") { Source = this, });
            mbinding.Bindings.Add(new Binding("Y") { Source = next, });
            mbinding.Bindings.Add(new Binding("X") { Source = next, });

            AdornerLayer.GetAdornerLayer(this).Add(connection);

            connection.SetBinding(
                ConnectionAdorner.SourceDestinationPairProperty,
                mbinding
            );
        }


        public virtual void RemoveNext(NodeViewModel next)
        {
            next.Inputs.Remove(this);
            this.Outputs.Remove(next);
        }


        protected override void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
        }

        protected override void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            X += e.HorizontalChange;
            Y += e.VerticalChange;

            Dragged?.Invoke(this, EventArgs.Empty);
        }

        protected override void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
        }


        // TODO:

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectionMode is null)
            {
                return;
            }

            _selectionMode.AddNext(this);

            _selectionMode = null;
        }

        private void MenuItem_AddOutput_Click(object sender, RoutedEventArgs e)
        {
            if (_selectionMode is not null)
            {
                return;
            }

            _selectionMode = this;
        }
    }
}
