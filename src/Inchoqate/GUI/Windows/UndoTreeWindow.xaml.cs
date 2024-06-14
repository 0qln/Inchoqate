using Inchoqate.GUI.Model.Events;
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
using System.Windows.Shapes;

namespace Inchoqate.GUI.Windows
{
    public partial class UndoTreeWindow : BorderlessWindowBase
    {
        public static readonly DependencyProperty EventTreesProperty = 
            DependencyProperty.Register(
                "EventTrees",
                typeof(ObservableCollection<EventTreeModel>),
                typeof(UndoTreeWindow),
                new FrameworkPropertyMetadata(
                    EventTreeModel.RegisteredTrees,
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public UndoTreeWindow()
        {
            InitializeComponent();

            EventTreeModel.RegisteredTrees.CollectionChanged += (s, e) =>
            {
                InvalidateProperty(EventTreesProperty);
            };
        }
    }
}
