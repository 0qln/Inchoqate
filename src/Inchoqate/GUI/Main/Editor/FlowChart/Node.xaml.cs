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
    public partial class OptionCollection : ObservableCollection<Control>
    {
    }


    public class NodeCollection : ObservableCollection<Control>
    {
    }


    /// <summary>
    /// Makes a connection from the output of a node to the input of another node.
    /// </summary>
    public class NodeConnectionAdorner : Adorner
    {
        public NodeConnectionAdorner(Node adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Node node = (Node)AdornedElement;

            double yMin = Margin.Top;
            double xOut = node.ActualWidth;
            double yMax = node.ActualHeight - Margin.Bottom - yMin;

            var brush = Brushes.White;
            var pen = new Pen(brush, 0);
            var r = 5.0;

            // Draw outputs
            var count = node.Outputs.Count;
            for (int i = 0; i < count; i++)
            {
                var y = Utils.Lerp(yMin, yMax, (i + 0.5) / count);
                var x = xOut;
                drawingContext.DrawEllipse(brush, pen, new Point(x, y), r, r);
            }
        }
    }


    /// <summary>
    /// Interaction logic for FlowChartNode.xaml
    /// </summary>
    public partial class Node : UserControl
    {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Node));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(OptionCollection), typeof(Node));

        public OptionCollection Options
        {
            get => (OptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }


        public static readonly DependencyProperty InputsProperty = DependencyProperty.Register(
            "Inputs", typeof(NodeCollection), typeof(Node));

        public NodeCollection Inputs
        {
            get => (NodeCollection)GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }


        public static readonly DependencyProperty OutputsProperty = DependencyProperty.Register(
            "Outputs", typeof(NodeCollection), typeof(Node));

        public NodeCollection Outputs
        {
            get => (NodeCollection)GetValue(OutputsProperty);
            set => SetValue(OutputsProperty, value);
        }


        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register(
            "IsDragging", typeof(bool), typeof(Node));

        public bool IsDragging
        {
            get => (bool)GetValue(IsDraggingProperty);
            set => SetValue(IsDraggingProperty, value);
        }


        private Point _dragBegin;


        public Node()
        {
            InitializeComponent();

            Outputs = [];
            Inputs = [];

            Loaded += Node_Loaded;
        }

        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this).Add(
                new NodeConnectionAdorner(this)
                {
                    Margin = new Thickness
                    {
                        Top = 15,
                        Bottom = 15
                    }
                }
            );

            var parent = (FrameworkElement)Parent;
            parent.MouseMove += DragApply;
            parent.MouseUp += DragEnd;
        }

        private void DragEnd(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
        }

        private void DragApply(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                var pos = e.GetPosition(Parent as FrameworkElement);
                // TODO: check bounds
                {
                    Canvas.SetTop(this, pos.Y - _dragBegin.Y);
                    Canvas.SetLeft(this, pos.X - _dragBegin.X);
                }
            }
        }

        private void DragBegin(object sender, MouseButtonEventArgs e)
        {
            _dragBegin = e.GetPosition(this);
            IsDragging = true;
        }
    }
}
