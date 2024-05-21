using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
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

namespace GUI.Main.Editor
{
    public partial class NodeView : UserControl
    {


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(NodeViewModel));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(NodeOptionCollection), typeof(NodeViewModel));
        public NodeView()
        {
            InitializeComponent();
        }


        public NodeOptionCollection Options
        {
            get => (NodeOptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }


        protected virtual void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
        }

        protected virtual void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
        }

        protected virtual void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
        }
    }
}
