using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Inchoqate.GUI.View
{
    public partial class EventTreeEditorView : UserControl
    {
        public static readonly DependencyProperty EditorTargetProperty = 
            DependencyProperty.Register(
                nameof(EditorTarget),
                typeof(EventTreeModel),
                typeof(EventTreeEditorView),
                new FrameworkPropertyMetadata(
                    null, 
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    EditorTargetPropertyChangedCallback));

        private static void EditorTargetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // only now we attatch an event system as viewmodel to the
            // editor target for realtime updates of the tree.
            // TODO: view model has to be removed later.
            var @this = (EventTreeEditorView)d;
            var tree = (EventTreeViewModel)e.NewValue;
            @this.Renderer.ViewModel = new EventViewModel((EventModel)tree.Initial, "Initial Event");
            @this.Renderer.EventTree = tree;
        }

        public EventTreeModel? EditorTarget
        {
            get => (EventTreeModel?)GetValue(EditorTargetProperty);
            set => SetValue(EditorTargetProperty, value);
        }


        public EventTreeEditorView()
        {
            InitializeComponent();
        }
    }
}
