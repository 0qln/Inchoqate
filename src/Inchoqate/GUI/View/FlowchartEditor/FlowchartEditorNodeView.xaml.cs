using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Inchoqate.GUI.ViewModel;

namespace Inchoqate.GUI.View.FlowchartEditor;

/// <summary>
/// Interaction logic for FlowchartEditorNodeView.xaml
/// </summary>
public partial class FlowchartEditorNodeView : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            "ViewModel",
            typeof(EditBaseDynamic),
            typeof(FlowchartEditorNodeView),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender | 
                FrameworkPropertyMetadataOptions.AffectsParentArrange));

    public FlowchartEditorNodeView()
    {
        InitializeComponent();

        SetBinding(DataContextProperty, new Binding("ViewModel") { Source = this });
    }

    private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
    {

    }

    private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {

    }

    private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {

    }
}