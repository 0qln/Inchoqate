﻿using Inchoqate.GUI.Converters;
using Inchoqate.GUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.View
{
    // TODO: implement remaining features
    /// <summary>
    /// Extends the std Slider with the following functionality:
    ///     - Mutliple thumbs
    ///     - Keyboard navigation
    ///     - Naming the thumbs with title and value bindings
    /// </summary>
    public partial class ExtSliderView : UserControl
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(double),
            typeof(ExtSliderView),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(double),
            typeof(ExtSliderView),
            new FrameworkPropertyMetadata(
                1.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));

        // TODO: use `DoubleCollection` ?

        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.RegisterAttached(
                "Values",
                typeof(double[]),
                typeof(ExtSliderView),
                new FrameworkPropertyMetadata(
                    propertyChangedCallback: ValuespropertyChangedCallback,
                    coerceValueCallback: ValuespropertyCoerceValueCallback));

        public static readonly DependencyProperty RangesProperty =
            DependencyProperty.RegisterAttached(
                "Ranges",
                typeof(double[]),
                typeof(ExtSliderView),
                new FrameworkPropertyMetadata(
                    propertyChangedCallback: RangespropertyChangedCallback,
                    coerceValueCallback: RangespropertyCoerceValueCallback));

        public static readonly DependencyProperty ValueCountProperty = DependencyProperty.Register(
            "ValueCount",
            typeof(int),
            typeof(ExtSliderView),
            new FrameworkPropertyMetadata(
                1,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RangeCountProperty = DependencyProperty.Register(
            "RangeCount",
            typeof(int),
            typeof(ExtSliderView),
            new FrameworkPropertyMetadata(
                2,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ShowValuesProperty = 
            DependencyProperty.Register(
                "ShowValues",
                typeof(bool[]),
                typeof(ExtSliderView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        // TOOD
        public static readonly DependencyProperty ShowRangesProperty = 
            DependencyProperty.Register(
                "ShowRanges",
                typeof(bool[]),
                typeof(ExtSliderView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundGradientBrushesProperty =
            DependencyProperty.Register(
                "BackgroundGradientBrushes",
                typeof(Color[]),
                typeof(ExtSliderView),
                new FrameworkPropertyMetadata(
                    new Color[] { 
                        (Color)((App)Application.Current).ThemeDictionary["Element_Idle_3"]
                    },
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double[] Values
        {
            get { return (double[])GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public double[] Ranges
        {
            get { return (double[])GetValue(RangesProperty); }
            set { SetValue(RangesProperty, value); }
        }

        public int ValueCount
        {
            get { return (int)GetValue(ValueCountProperty); }
            set { SetValue(ValueCountProperty, value); }
        }

        public int RangeCount
        {
            get { return (int)GetValue(RangeCountProperty); }
            set { SetValue(RangeCountProperty, value); }
        }

        public bool[] ShowValues
        {
            get { return (bool[])GetValue(ShowValuesProperty); }
            set { SetValue(ShowValuesProperty, value); }
        }

        public bool[] ShowRanges
        {
            get { return (bool[])GetValue(ShowRangesProperty); }
            set { SetValue(ShowRangesProperty, value); }
        }

        public Color[] BackgroundGradientBrushes
        {
            get { return (Color[])GetValue(BackgroundGradientBrushesProperty); }
            set { SetValue(BackgroundGradientBrushesProperty, value); }
        }


        public ExtSliderView()
        {
            InitializeComponent();

            SetBinding(RangeCountProperty, new Binding("ValueCount") { Source = this, Converter = new OffsetConverter<int>(1), Mode = BindingMode.TwoWay });
        }


        private static void RangespropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (ExtSliderView)d;
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
            var extSlider = (ExtSliderView)d;

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
            var slider = (ExtSliderView)d;
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
            var extSlider = (ExtSliderView)d;

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


    public class GradientStopsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: create a property for this.
            if (/*smooth interpolation*/ false)
            {
                var result = new GradientStopCollection();
                var colors = (Color[])values[0];
                var thumbValues = (double[])values[1];
                var minimum = (double)values[2];
                var maximum = (double)values[3];
                var offsets = new double[thumbValues.Length + 1];
                offsets[0] = 0.0;
                var range = Math.Abs(minimum) + Math.Abs(maximum);
                for (int i = 1; i < offsets.Length; i++)
                {
                    var valNorm = (thumbValues[i - 1] - minimum) / range;
                    offsets[i] = valNorm;
                }
                if (colors.Length != offsets.Length)
                {
                    if (colors.Length == 1)
                        return new SolidColorBrush(colors[0]);

                    else throw new ArgumentException("The number of colors must be equal the number of offsets.");
                }
                for (int i = 0; i < colors.Length; i++)
                {
                    result.Add(new GradientStop
                    {
                        Color = colors[i],
                        Offset = offsets[i]
                    });
                }
                
                return new LinearGradientBrush(result);
            }
            else
            {
                var result = new GradientStopCollection();
                var colors = (Color[])values[0];
                var thumbValues = (double[])values[1];
                var minimum = (double)values[2];
                var maximum = (double)values[3];
                var offsets = new double[thumbValues.Length + 1 /*ranges count*/ + 1 /*maximum*/];
                offsets[0] = 0.0;
                var range = Math.Abs(minimum) + Math.Abs(maximum);
                for (int i = 1; i < colors.Length; i++)
                {
                    var valNorm = (thumbValues[i - 1] - minimum) / range;
                    offsets[i] = valNorm;
                }
                offsets[^1] = 1.0;
                if (colors.Length != offsets.Length - 1)
                {
                    if (colors.Length == 1)
                        return new SolidColorBrush(colors[0]);

                    else throw new ArgumentException("The number of colors must be equal the number of offsets.");
                }
                for (int i = 0; i < colors.Length; i++)
                {
                    result.Add(new GradientStop
                    {
                        Color = colors[i],
                        Offset = offsets[i]
                    });
                    result.Add(new GradientStop
                    {
                        Color = colors[i],
                        Offset = offsets[i + 1]
                    });
                }
                return new LinearGradientBrush(result);
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SliderInfoAdorner : Adorner
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(double),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(double),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowPrevRangeProperty =
            DependencyProperty.Register(
                "ShowPrevRange",
                typeof(bool),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowNextRangeProperty =
            DependencyProperty.Register(
                "ShowNextRange",
                typeof(bool),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowValueProperty =
            DependencyProperty.Register(
                "ShowValue",
                typeof(bool),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RangesProperty =
            DependencyProperty.Register(
                "Ranges",
                typeof(double[]),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register(
                "Index",
                typeof(int),
                typeof(SliderInfoAdorner),
                new FrameworkPropertyMetadata(
                    -1,
                    FrameworkPropertyMetadataOptions.AffectsRender));


        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool ShowNextRange
        {
            get { return (bool)GetValue(ShowNextRangeProperty); }
            set { SetValue(ShowNextRangeProperty, value); }
        }

        public bool ShowPrevRange
        {
            get { return (bool)GetValue(ShowPrevRangeProperty); }
            set { SetValue(ShowPrevRangeProperty, value); }
        }

        public bool ShowValue
        {
            get { return (bool)GetValue(ShowValueProperty); }
            set { SetValue(ShowValueProperty, value); }
        }

        public double[] Ranges
        {
            get { return (double[])GetValue(RangesProperty); }
            set { SetValue(RangesProperty, value); }
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public Brush TextColor
        {
            get => new SolidColorBrush((Color)((App)Application.Current).ThemeDictionary["Text_2"]);
        }

        public Brush BackgroundColor
        {
            get => new SolidColorBrush((Color)((App)Application.Current).ThemeDictionary["Popup_Idle_1"]);
        }


        public SliderInfoAdorner(SliderPart adornedElement) : base(adornedElement)
        {
            SetBinding(ValueProperty, new Binding("Value") { Source = adornedElement, });
            SetBinding(MinimumProperty, new Binding("Minimum") { Source = adornedElement, });
            SetBinding(MaximumProperty, new Binding("Maximum") { Source = adornedElement, });
            SetBinding(IndexProperty, new Binding("Index") { Source = adornedElement, });
            SetBinding(ShowValueProperty, new Binding("ShowValue") { Source = adornedElement, });
            SetBinding(ShowPrevRangeProperty, new Binding("ShowPrevRange") { Source = adornedElement, });
            SetBinding(ShowNextRangeProperty, new Binding("ShowNextRange") { Source = adornedElement, });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (AdornedElement is SliderPart slider)
            {
                // TODO: maxtextwidth, typeface, textsize as property

                var track = (Track)slider.Template.FindName("PART_Track", slider);
                var trackSpace = Math.Abs(Minimum) + Math.Abs(Maximum);
                double norm(double value) => (value - Minimum) / trackSpace;
                double toScreen(double value) => value * (slider.ActualWidth - track.Thumb.ActualWidth);
                var thumbX = toScreen(norm(Value));
                double textSize = 12;
                double maxTextWidth = 40;
                var typeFace = new Typeface("Segoe UI");

                void DrawText(string text, double x, double y, bool top)
                {
                    // TODO: use non-obsolete function.
#pragma warning disable CS0618 // Type or member is obsolete
                    var formattedText = new FormattedText(
                        text,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        typeFace,
                        textSize,
                        TextColor)
                    {
                        MaxTextWidth = maxTextWidth,
                    };
#pragma warning restore CS0618 // Type or member is obsolete

                    // transform 
                    y += top
                        ? -formattedText.Height + (slider.ActualHeight - track.Thumb.ActualHeight)
                        : (slider.ActualHeight - track.Thumb.ActualHeight) + track.Thumb.ActualHeight;
                    // draw
                    if (top) drawingContext.DrawRectangle(BackgroundColor, null, new Rect(x, y, formattedText.Width, formattedText.Height));
                    drawingContext.DrawText(formattedText, new Point(x, y));
                }

                if (ShowValue)
                {
                    DrawText(Value.ToString(), thumbX, 0, true);
                }

                if (ShowNextRange 
                    && Index + 1 < Ranges.Length 
                    && Index + 1 >= 0)
                {
                    var range = Ranges[Index + 1];
                    DrawText(range.ToString(), thumbX + toScreen(norm(range)) / 2, 0, false);
                }

                if (ShowPrevRange
                    && Index >= 0
                    && Index < Ranges.Length)
                {
                    var range = Ranges[Index];
                    DrawText(range.ToString(), thumbX - toScreen(norm(range)) / 2, 0, false);
                }
            }
        }
    }


    public class CountToSliderConverter() 
        : CountToControlsConverter<SliderPart>((_, container, index) =>
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


    public class SliderValueIndexer() 
        : ElementAtConverter<double>()
    {
    }


    public class SliderShowValueIndexer()
        : ElementAtConverter<bool>(@default: false)
    {
    }


    public class SliderPart : Slider
    {
        public static readonly DependencyProperty ValueMinProperty = 
            DependencyProperty.Register(
                "ValueMin",
                typeof(double),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(
                    double.NegativeInfinity,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ValueMaxProperty = 
            DependencyProperty.Register(
                "ValueMax",
                typeof(double),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(
                    double.PositiveInfinity,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IndexProperty = 
            DependencyProperty.Register(
                "Index",
                typeof(int),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(-1));

        public static readonly DependencyProperty RangeProperty = 
            DependencyProperty.Register(
                "Range",
                typeof(double),
                typeof(SliderPart));

        public static readonly DependencyProperty ExtSliderProperty =
            DependencyProperty.Register(
                "ExtSlider",
                typeof(ExtSliderView),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TrackVisibilityProperty =
            DependencyProperty.Register(
                "TrackVisibility",
                typeof(Visibility),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(
                    Visibility.Collapsed,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowValueProperty =
            DependencyProperty.Register(
                "ShowValue",
                typeof(bool),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(
                    true,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowPrevRangeProperty =
            DependencyProperty.Register(
                "ShowPrevRange",
                typeof(bool),
                typeof(SliderPart),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowNextRangeProperty =
            DependencyProperty.Register(
                "ShowNextRange",
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

        public ExtSliderView ExtSlider
        {
            get => (ExtSliderView)GetValue(ExtSliderProperty);
            set => SetValue(ExtSliderProperty, value);
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


        public SliderPart(SliderPart? previousPart)
        {
            SetBinding(RangeProperty, new Binding("Value") { Source = this, Converter = new ValueToRangeConverter(previousPart), Mode = BindingMode.TwoWay });
            SetBinding(TrackVisibilityProperty, new Binding("Index") { Source = this, Converter = new IndexToTrackVisibilityConverter(), Mode = BindingMode.OneWay });
            Loaded += LoadedHandler;
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            var adorner = new SliderInfoAdorner(this);
            AdornerLayer.GetAdornerLayer(this).Add(adorner);
            var prBinding = new MultiBinding() { Converter = new ElementAtConverter<bool>(fallbackPredicateIndex: index => index != 0) };
            prBinding.Bindings.Add(new Binding("ShowRanges") { Source = ExtSlider });
            prBinding.Bindings.Add(new Binding("Index") { Source = this });
            adorner.SetBinding(SliderInfoAdorner.ShowPrevRangeProperty, prBinding);
            var nrBinding = new MultiBinding() { Converter = new ElementAtConverter<bool>(indexTransform: index => index + 1) };
            nrBinding.Bindings.Add(new Binding("ShowRanges") { Source = ExtSlider });
            nrBinding.Bindings.Add(new Binding("Index") { Source = this });
            adorner.SetBinding(SliderInfoAdorner.ShowNextRangeProperty, nrBinding);
            adorner.SetBinding(SliderInfoAdorner.RangesProperty, new Binding("Ranges") { Source = ExtSlider });

            var bgBinding = new MultiBinding() { Converter = new GradientStopsConverter() };
            bgBinding.Bindings.Add(new Binding("BackgroundGradientBrushes") { Source = ExtSlider });
            bgBinding.Bindings.Add(new Binding("Values") { Source = ExtSlider });
            bgBinding.Bindings.Add(new Binding("Minimum") { Source = ExtSlider });
            bgBinding.Bindings.Add(new Binding("Maximum") { Source = ExtSlider });
            SetBinding(BackgroundProperty, bgBinding);
        }

        private static void ValuepropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (SliderPart)d;
            var arr = @this.ExtSlider.Values.ToArray();
            arr[@this.Index] = (double)e.NewValue;
            @this.ExtSlider.Values = arr;
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

}
