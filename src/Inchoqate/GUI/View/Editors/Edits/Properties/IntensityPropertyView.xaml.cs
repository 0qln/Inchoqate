using System.Windows.Controls.Primitives;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.View.Converters;

namespace Inchoqate.GUI.View.Editors.Edits.Properties;

/// <summary>
///     Interaction logic for Intensity.xaml
/// </summary>
public partial class IntensityPropertyView : MonitoredPropertyView<IIntensityProperty, IntensityChangedEvent>
{
    private double _intensityChangeBegin;

    public IntensityPropertyView() : base("Intensity")
    {
        InitializeComponent();
    }

    private void MultiSlider_OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
    {
        (this as IEventDelegate<IntensityChangedEvent>).Delegate(new() { OldValue = _intensityChangeBegin, NewValue = Model.Intensity });
    }

    private void MultiSlider_OnThumbDragStarted(object sender, DragStartedEventArgs e)
    {
        _intensityChangeBegin = Model.Intensity;
    }
}

public class DoubleArrayBoxingConverter : ArrayBoxingConverter<double>;