using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using Inchoqate.GUI.View.Converters;

namespace Inchoqate.GUI.View.MultiSlider;

public class SliderPart : Slider
{
    public static readonly DependencyProperty ValueMinProperty = 
        DependencyProperty.Register(
            nameof(ValueMin),
            typeof(double),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                double.NegativeInfinity,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ValueMaxProperty = 
        DependencyProperty.Register(
            nameof(ValueMax),
            typeof(double),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                double.PositiveInfinity,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IndexProperty = 
        DependencyProperty.Register(
            nameof(Index),
            typeof(int),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(-1));

    public static readonly DependencyProperty RangeProperty = 
        DependencyProperty.Register(
            nameof(Range),
            typeof(double),
            typeof(SliderPart));

    public static readonly DependencyProperty MultiSliderProperty =
        DependencyProperty.Register(
            nameof(MultiSlider),
            typeof(MultiSlider),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                propertyChangedCallback: ExtSliderChangedCallback));

    private static void ExtSliderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var @this = (SliderPart)d;
        var bgBinding = new MultiBinding() { Converter = new GradientStopsConverter() };
        bgBinding.Bindings.Add(new Binding("BackgroundGradientBrushes") { Source = e.NewValue });
        bgBinding.Bindings.Add(new Binding("Values") { Source = e.NewValue });
        bgBinding.Bindings.Add(new Binding("Minimum") { Source = e.NewValue });
        bgBinding.Bindings.Add(new Binding("Maximum") { Source = e.NewValue });
        @this.SetBinding(BackgroundProperty, bgBinding);

        @this._infoAdorner = new SliderInfoAdorner(@this);
        AdornerLayer.GetAdornerLayer(@this).Add(@this._infoAdorner);
        var prBinding = new MultiBinding() { Converter = new ElementAtConverter<bool>(fallbackPredicateIndex: index => index != 0) };
        prBinding.Bindings.Add(new Binding("ShowRanges") { Source = e.NewValue });
        prBinding.Bindings.Add(new Binding("Index") { Source = @this });
        @this._infoAdorner.SetBinding(SliderInfoAdorner.ShowPrevRangeProperty, prBinding);
        var nrBinding = new MultiBinding() { Converter = new ElementAtConverter<bool>(indexTransform: index => index + 1) };
        nrBinding.Bindings.Add(new Binding("ShowRanges") { Source = e.NewValue });
        nrBinding.Bindings.Add(new Binding("Index") { Source = @this });
        @this._infoAdorner.SetBinding(SliderInfoAdorner.ShowNextRangeProperty, nrBinding);
        @this._infoAdorner.SetBinding(SliderInfoAdorner.RangesProperty, new Binding("Ranges") { Source = e.NewValue });
    }

    public static readonly DependencyProperty TrackVisibilityProperty =
        DependencyProperty.Register(
            nameof(TrackVisibility),
            typeof(Visibility),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                Visibility.Collapsed,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowValueProperty =
        DependencyProperty.Register(
            nameof(ShowValue),
            typeof(bool),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                true,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowPrevRangeProperty =
        DependencyProperty.Register(
            nameof(ShowPrevRange),
            typeof(bool),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowNextRangeProperty =
        DependencyProperty.Register(
            nameof(ShowNextRange),
            typeof(bool),
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));


    public double ValueMin
    {
        get => (double)GetValue(ValueMinProperty);
        set => SetValue(ValueMinProperty, value);
    }

    public double ValueMax
    {
        get => (double)GetValue(ValueMaxProperty);
        set => SetValue(ValueMaxProperty, value);
    }

    public int Index
    {
        get => (int)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    /// <summary> The range left of the thumb. </summary>
    public double Range
    {
        get => (double)GetValue(RangeProperty);
        set => SetValue(RangeProperty, value);
    }

    public MultiSlider MultiSlider
    {
        get => (MultiSlider)GetValue(MultiSliderProperty);
        set => SetValue(MultiSliderProperty, value);
    }

    public Visibility TrackVisibility
    {
        get => (Visibility)GetValue(TrackVisibilityProperty);
        set => SetValue(TrackVisibilityProperty, value);
    }

    public bool ShowValue
    {
        get => (bool)GetValue(ShowValueProperty);
        set => SetValue(ShowValueProperty, value);
    }

    public bool ShowPrevRange
    {
        get => (bool)GetValue(ShowPrevRangeProperty);
        set => SetValue(ShowPrevRangeProperty, value);
    }

    public bool ShowNextRange
    {
        get => (bool)GetValue(ShowNextRangeProperty);
        set => SetValue(ShowNextRangeProperty, value);
    }

    public event DragStartedEventHandler? ThumbDragStarted;

    public event DragCompletedEventHandler? ThumbDragCompleted;


    private Adorner? _infoAdorner;


    static SliderPart()
    {
        ValueProperty.OverrideMetadata(
            typeof(SliderPart),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender,
                propertyChangedCallback: ValuepropertyChangedCallback,
                coerceValueCallback: ValuepropertyCoerceValueCallback));
    }


    protected override void OnThumbDragStarted(DragStartedEventArgs e)
    {
        base.OnThumbDragStarted(e);
        ThumbDragStarted?.Invoke(this, e);
    }

    protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
    {
        base.OnThumbDragCompleted(e);
        ThumbDragCompleted?.Invoke(this, e);
    }


    public SliderPart(SliderPart? previousPart)
    {
        SetBinding(RangeProperty, new Binding("Value") { Source = this, Converter = new ValueToRangeConverter(previousPart), Mode = BindingMode.TwoWay });
        SetBinding(TrackVisibilityProperty, new Binding("Index") { Source = this, Converter = new IndexToTrackVisibilityConverter(), Mode = BindingMode.OneWay });
    }

    private static void ValuepropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var @this = (SliderPart)d;
        var arr = @this.MultiSlider.Values.ToArray();
        arr[@this.Index] = (double)e.NewValue;
        @this.MultiSlider.Values = arr;
    }

    private static object ValuepropertyCoerceValueCallback(DependencyObject d, object baseValue)
    {
        var slider = (SliderPart)d;
        var value = (double)baseValue;
        return Math.Clamp(value, slider.ValueMin, slider.ValueMax);
    }


    private class ValueToRangeConverter(SliderPart? previousPart) : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var thisVal = (double)value;
            var prevVal = previousPart?.Value ?? 0.0;
            return thisVal - prevVal;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var newRange = (double)value;
            var prevVal = previousPart?.Value ?? 0.0;
            return newRange + prevVal;
        }
    }

    private class IndexToTrackVisibilityConverter() : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value == 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}