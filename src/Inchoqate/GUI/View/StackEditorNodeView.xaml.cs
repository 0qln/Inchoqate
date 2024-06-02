using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel;
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

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for StackEditorNodeView.xaml
    /// </summary>
    public partial class StackEditorNodeView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(EditBaseLinear),
                typeof(StackEditorNodeView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender | 
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty ContentVisibilityProperty = 
            DependencyProperty.Register(
                "ContentVisibility",
                typeof(Visibility),
                typeof(StackEditorNodeView),
                new FrameworkPropertyMetadata(
                    Visibility.Visible,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public EditBaseLinear ViewModel
        {
            get => (EditBaseLinear)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public Visibility ContentVisibility
        {
            get => (Visibility)GetValue(ContentVisibilityProperty);
            set => SetValue(ContentVisibilityProperty, value);
        }


        public StackEditorNodeView()
        {
            InitializeComponent();

            SetBinding(DataContextProperty, new Binding("ViewModel") { Source = this });
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // TODO: only if the drag distance was 0.
            if (Content.Visibility == Visibility.Visible)
            {
                ContentVisibility = Visibility.Collapsed;
            }
            else
            {
                ContentVisibility = Visibility.Visible;
            }
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {

        }
    }
}
