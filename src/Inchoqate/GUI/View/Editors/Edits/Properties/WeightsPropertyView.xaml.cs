using System.Windows.Controls.Primitives;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using OpenTK.Mathematics;

namespace Inchoqate.GUI.View.Editors.Edits.Properties;

/// <summary>
///     Interaction logic for WeightsPropertyView.xaml
/// </summary>
public partial class WeightsPropertyView : MonitoredPropertyView<IWeightsProperty, WeightsChangedEvent>
{
    private Vector3 _weightsChangeBegin;

    public WeightsPropertyView() : base("Weights")
    {
        InitializeComponent();
    }

    private void MultiSlider_OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
    {
        (this as IEventDelegate<WeightsChangedEvent>).Delegate(new() { OldValue = _weightsChangeBegin, NewValue = Model.Weights });
    }

    private void MultiSlider_OnThumbDragStarted(object sender, DragStartedEventArgs e)
    {
        _weightsChangeBegin = Model.Weights;
    }
}