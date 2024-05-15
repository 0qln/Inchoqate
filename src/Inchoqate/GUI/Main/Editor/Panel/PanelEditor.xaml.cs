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

namespace Inchoqate.GUI.Main.Editor.Panel
{
    // TODO: Panel editor as input|node of flow chart editor?

    /// <summary>
    /// The Panel Editor is a linear, squential pipeline of simple, singlepass edits or shaders.
    /// 
    ///                    Side Panel
    /// +-----------------+---------+
    /// |                 | =edit6= | Pipeline Index 1 (collapsed)
    /// |                 |         |
    /// |  Preview Image  | =edit3= | Pipeline Index 2 (visible)
    /// |                 |  opt1   |
    /// |                 |  opt2   |
    /// |                 |         |
    /// +-----------------+---------+
    /// 
    /// </summary>
    public partial class PanelEditor : UserControl, IEditor
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


        public PanelEditor()
        {
            InitializeComponent();
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
