using System.Windows.Data;
using Inchoqate.GUI.View.Converters;

namespace Inchoqate.GUI.View.MultiSlider;

public class CountToSliderConverter() 
    : CountToObservableCollectionConverter<SliderPart>((_, container, index) =>
    {
        var prevSlider = index > 0 ? container[index - 1] : null;
        var slider = new SliderPart(prevSlider) { Name = $"Slider{index}", Index = index };

        slider.Loaded += (s, e) => {
            if (index > 0)
                slider.SetBinding(SliderPart.ValueMinProperty, new Binding("Value") { Source = container[index - 1] });
            if (index < container.Count - 1)
                slider.SetBinding(SliderPart.ValueMaxProperty, new Binding("Value") { Source = container[index + 1] });
        };

        return slider;
    })
{
}