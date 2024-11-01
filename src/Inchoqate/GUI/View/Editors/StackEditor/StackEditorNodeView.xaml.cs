using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.View.Editors.Edits;
using Inchoqate.GUI.View.Editors.Edits.Properties;

namespace Inchoqate.GUI.View.Editors.StackEditor;

/// <summary>
/// Interaction logic for StackEditorNodeView.xaml
/// </summary>
public partial class StackEditorNodeView : UserControl
{
    public static readonly DependencyProperty ContentVisibilityProperty = 
        DependencyProperty.Register(
            nameof(ContentVisibility),
            typeof(Visibility),
            typeof(StackEditorNodeView),
            new FrameworkPropertyMetadata(
                Visibility.Visible,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));

    public EditBaseView? Edit
    {
        get => (EditBaseView?)DataContext;
        set => DataContext = value;
    }

    public Visibility ContentVisibility
    {
        get => (Visibility)GetValue(ContentVisibilityProperty);
        set => SetValue(ContentVisibilityProperty, value);
    }

    private Point _dragOffset;

    public StackEditorNodeCollection? SelfContainer { get; set; }


    public StackEditorNodeView()
    {
        InitializeComponent();
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
        if (SelfContainer is null || Edit is null)
            return;

        var index = SelfContainer.IndexOf(this);
        var stackPanel = (StackPanel)VisualParent;
        var moveItems = SelfContainer as IEventDelegate<ItemMovedEvent>;

        if (index < SelfContainer.Count - 1 && 
            e.VerticalChange + _dragOffset.Y > stackPanel.Children[index + 1].TransformToVisual(this).Transform(new()).Y)
        {
            moveItems.Delegate(new() { From = index, To = index + 1});
        }   

        if (index > 0 && 
            e.VerticalChange + _dragOffset.Y < stackPanel.Children[index - 1].TransformToVisual(this).Transform(new()).Y)
        {
            moveItems.Delegate(new() { From = index, To = index - 1});
        }
    }

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        _dragOffset = new(e.HorizontalOffset, e.VerticalOffset);
    }
}