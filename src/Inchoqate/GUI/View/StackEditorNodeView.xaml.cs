using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.Model.Events;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for StackEditorNodeView.xaml
    /// </summary>
    public partial class StackEditorNodeView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
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
                nameof(ContentVisibility),
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

        private Point _dragOffset;

        public EditorNodeCollectionLinear? BackingCollection { get; set; }


        public StackEditorNodeView()
        {
            InitializeComponent();

            SetBinding(DataContextProperty, new Binding("ViewModel") { Source = this });
        }


        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (e.VerticalChange != 0) return;

            ContentVisibility = EditorContent.Visibility == Visibility.Visible 
                ? Visibility.Collapsed 
                : Visibility.Visible;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (BackingCollection is null)
            {
                return;
            }

            var index = BackingCollection.IndexOf(ViewModel);

            var stackPanel = (StackPanel)VisualParent;

            if (index < BackingCollection.Count - 1 && 
                e.VerticalChange + _dragOffset.Y > stackPanel.Children[index + 1].TransformToVisual(this).Transform(new()).Y)
            {
                BackingCollection.Eventuate(new ItemMovedEvent(index, index + 1));
            }   

            if (index > 0 && 
                e.VerticalChange + _dragOffset.Y < stackPanel.Children[index - 1].TransformToVisual(this).Transform(new()).Y)
            {
                BackingCollection.Eventuate(new ItemMovedEvent(index, index - 1));
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _dragOffset = new(e.HorizontalOffset, e.VerticalOffset);
        }
    }
}
