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

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    /// <summary>
    /// The flow chart editor can combine multiple complex edits in a node
    /// like structure.
    /// 
    /// +---------------------------+
    /// |                           | 
    /// |                           |
    /// |       Preview Image       | 
    /// |                           |
    /// |          +---Flow chart editor---+
    /// |          |                       |                
    /// +----------|    In-----+           |
    ///            |           |           |
    ///            | Preset1--->-----Out   |
    ///            |                       |
    ///            +-----------------------+
    /// 
    /// </summary>
    public partial class FlowChartEditor : Page, IEditor
    {
        Texture IEditor.Source
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        Texture IEditor.Result
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public bool SelectionMode { get; set; }


        public FlowChartEditor()
        {
            InitializeComponent();
        }


        // TODO: Lazy hotreload.
        // TODO: Compile non-multipass editnodes into a single shader.
        // TODO: output pipeline
        public void Compile() { }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            E_InputNode.SetNext(E_OutputNode);

            Canvas.SetTop(E_InputNode, 50);
            Canvas.SetLeft(E_InputNode, 50);

            // `Canvas.SetRight`/`Canvas.SetBottom` introduces glitches with the connection
            // adorner for some reason. Can't bother to figure out why...
            Canvas.SetTop(E_OutputNode, E_MainCanvas.ActualHeight - 50 - E_OutputNode.ActualHeight);
            Canvas.SetLeft(E_OutputNode, E_MainCanvas.ActualWidth - 50 - E_OutputNode.ActualWidth);
        }


        #region Disposing stuff

        private bool _disposedValue;

        public event EventHandler? Disposing;


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Disposing?.Invoke(this, EventArgs.Empty);
                }

                _disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
