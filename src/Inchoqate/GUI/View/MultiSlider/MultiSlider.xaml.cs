using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Inchoqate.GUI.View.Converters;

namespace Inchoqate.GUI.View.MultiSlider;

// TODO: implement remaining features
/// <summary>
/// Extends the std Slider with the following functionality:
///     - Mutliple thumbs
///     - Keyboard navigation
///     - Naming the thumbs with title and value bindings
/// </summary>
public partial class MultiSlider : UserControl
{
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(double),
        typeof(MultiSlider),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(double),
        typeof(MultiSlider),
        new FrameworkPropertyMetadata(
            1.0,
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsRender));

    // TODO: use `DoubleCollection` ?

    public static readonly DependencyProperty ValuesProperty =
        DependencyProperty.RegisterAttached(
            "Values",
            typeof(double[]),
            typeof(MultiSlider),
            new FrameworkPropertyMetadata(
                propertyChangedCallback: ValuespropertyChangedCallback,
                coerceValueCallback: ValuespropertyCoerceValueCallback));

    public static readonly DependencyProperty RangesProperty =
        DependencyProperty.RegisterAttached(
            "Ranges",
            typeof(double[]),
            typeof(MultiSlider),
            new FrameworkPropertyMetadata(
                propertyChangedCallback: RangespropertyChangedCallback,
                coerceValueCallback: RangespropertyCoerceValueCallback));

    public static readonly DependencyProperty ValueCountProperty = DependencyProperty.Register(
        nameof(ValueCount),
        typeof(int),
        typeof(MultiSlider),
        new FrameworkPropertyMetadata(
            1,
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty RangeCountProperty = DependencyProperty.Register(
        nameof(RangeCount),
        typeof(int),
        typeof(MultiSlider),
        new FrameworkPropertyMetadata(
            2,
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty ShowValuesProperty = 
        DependencyProperty.Register(
            nameof(ShowValues),
            typeof(bool[]),
            typeof(MultiSlider),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    // TOOD
    public static readonly DependencyProperty ShowRangesProperty = 
        DependencyProperty.Register(
            nameof(ShowRanges),
            typeof(bool[]),
            typeof(MultiSlider),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty BackgroundGradientBrushesProperty =
        DependencyProperty.Register(
            nameof(BackgroundGradientBrushes),
            typeof(Color[]),
            typeof(MultiSlider),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty UseSmoothGradientsProperty =
        DependencyProperty.Register(
            nameof(UseSmoothGradients),
            typeof(bool),
            typeof(MultiSlider),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public bool UseSmoothGradients
    {
        get => (bool)GetValue(UseSmoothGradientsProperty);
        set => SetValue(UseSmoothGradientsProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double[] Values
    {
        get => (double[])GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public double[] Ranges
    {
        get => (double[])GetValue(RangesProperty);
        set => SetValue(RangesProperty, value);
    }

    public int ValueCount
    {
        get => (int)GetValue(ValueCountProperty);
        set => SetValue(ValueCountProperty, value);
    }

    public int RangeCount
    {
        get => (int)GetValue(RangeCountProperty);
        set => SetValue(RangeCountProperty, value);
    }

    public bool[] ShowValues
    {
        get => (bool[])GetValue(ShowValuesProperty);
        set => SetValue(ShowValuesProperty, value);
    }

    public bool[] ShowRanges
    {
        get => (bool[])GetValue(ShowRangesProperty);
        set => SetValue(ShowRangesProperty, value);
    }

    public Color[]? BackgroundGradientBrushes
    {
        get => (Color[]?)GetValue(BackgroundGradientBrushesProperty);
        set => SetValue(BackgroundGradientBrushesProperty, value);
    }

    public event DragStartedEventHandler? ThumbDragStarted
    {
        add
        {
            foreach (var slider in SlidersControl.Items.Cast<SliderPart>())
            {
                slider.ThumbDragStarted += value;
            }
        }
        remove
        {
            foreach (var slider in SlidersControl.Items.Cast<SliderPart>())
            {
                slider.ThumbDragStarted -= value;
            }
        }
    }

    public event DragCompletedEventHandler? ThumbDragCompleted
    {
        add
        {
            foreach (var slider in SlidersControl.Items.Cast<SliderPart>())
            {
                slider.ThumbDragCompleted += value;
            }
        }
        remove
        {
            foreach (var slider in SlidersControl.Items.Cast<SliderPart>())
            {
                slider.ThumbDragCompleted -= value;
            }
        }
    }


    public MultiSlider()
    {
        InitializeComponent();

        SetBinding(RangeCountProperty, new Binding("ValueCount") { Source = this, Converter = new OffsetConverter<int>(1), Mode = BindingMode.TwoWay });
    }


    private static void RangespropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var slider = (MultiSlider)d;
        var ranges = (double[])e.NewValue;
        var values = new double[ranges.Length - 1];
            
        double current = .0;
        for (int i = 0; i < ranges.Length - 1; i++)
            values[i] = (current += ranges[i]);

        if (slider.Values is null || !values.SequenceEqual(slider.Values))
            slider.Values = values;
    }

    private static object RangespropertyCoerceValueCallback(DependencyObject d, object baseValue)
    {
        var extSlider = (MultiSlider)d;

        switch (baseValue)
        {
            case double[] arr:
                if (arr.Length != extSlider.RangeCount)
                    throw new ArgumentException("The number of ranges must be equal the range count.");
                break;

            default:
                throw new ArgumentException(baseValue.GetType().Name);
        }

        return baseValue;
    }

    private static void ValuespropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var slider = (MultiSlider)d;
        var values = (double[])e.NewValue;
        var ranges = new double[values.Length + 1];
            
        ranges[0] = values[0] - slider.Minimum;
        for (int i = 1; i < values.Length; i++)
            ranges[i] = values[i] - values[i - 1];
        ranges[^1] = slider.Maximum - values[^1];

        if (slider.Ranges is null || !ranges.SequenceEqual(slider.Ranges))
            slider.Ranges = ranges;
    }

    private static object ValuespropertyCoerceValueCallback(DependencyObject d, object baseValue)
    {
        var extSlider = (MultiSlider)d;

        switch (baseValue)
        {
            case double[] arr:
                if (arr.Length != extSlider.ValueCount)
                    throw new ArgumentException("The number of values must be equal the value count.");
                break;

            default:
                throw new ArgumentException(baseValue.GetType().Name);
        }

        return baseValue;
    }
}