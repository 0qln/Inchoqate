﻿using CommunityToolkit.Mvvm.ComponentModel;
using Inchoqate.GUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
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
    // TODO: squish the sliders into one lane.

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

        //public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
        //    "Values",
        //    typeof(ObservableItemCollection<ObservableStruct<double>>),
        //    typeof(ExtSliderView),
        //    new FrameworkPropertyMetadata(
        //        propertyChangedCallback: ValuespropertyChangedCallback,
        //        coerceValueCallback: ValuespropertyCoerceValueCallback));

        //public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
        //    "Values",
        //    typeof(ObservableCollection<double>),
        //    typeof(ExtSliderView),
        //    new FrameworkPropertyMetadata(
        //        propertyChangedCallback: ValuespropertyChangedCallback,
        //        coerceValueCallback: ValuespropertyCoerceValueCallback));

        //public static readonly DependencyProperty RangesProperty = DependencyProperty.Register(
        //    "Ranges",
        //    typeof(ObservableItemCollection<ObservableStruct<double>>),
        //    typeof(ExtSliderView),
        //    new FrameworkPropertyMetadata(
        //        propertyChangedCallback: RangespropertyChangedCallback,
        //        coerceValueCallback: RangespropertyCoerceValueCallback));

        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.RegisterAttached(
                "Values",
                typeof(double[]),
                typeof(ExtSliderView),
                new FrameworkPropertyMetadata(
                    propertyChangedCallback: ValuespropertyChangedCallback,
                    coerceValueCallback: ValuespropertyCoerceValueCallback));

        //public static readonly DependencyProperty RangesProperty =
        //    DependencyProperty.RegisterAttached(
        //        "Ranges",
        //        typeof(double[]),
        //        typeof(ExtSliderView),
        //        new FrameworkPropertyMetadata(
        //            propertyChangedCallback: RangespropertyChangedCallback,
        //            coerceValueCallback: RangespropertyCoerceValueCallback));

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

        //public ObservableItemCollection<ObservableStruct<double>> Values
        //{
        //    get => (ObservableItemCollection<ObservableStruct<double>>)GetValue(ValuesProperty);
        //    set => SetValue(ValuesProperty, value);
        //}

        //public ObservableCollection<double> Values
        //{
        //    get => (ObservableCollection<double>)GetValue(ValuesProperty);
        //    set => SetValue(ValuesProperty, value);
        //}

        //public ObservableItemCollection<ObservableStruct<double>> Ranges
        //{
        //    get => (ObservableItemCollection<ObservableStruct<double>>)GetValue(RangesProperty);
        //    set => SetValue(RangesProperty, value);
        //}

        public double[] Values
        {
            get { return (double[])GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        //public double[] Ranges
        //{
        //    get { return (double[])GetValue(RangesProperty); }
        //    set { SetValue(RangesProperty, value); }
        //}

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


        public ExtSliderView()
        {
            InitializeComponent();

            SetBinding(RangeCountProperty, new Binding("ValueCount") { Source = this, Converter = new OffsetConverter<int>(1), Mode = BindingMode.TwoWay });

            
        }

        //private class ValuesToRangesConverter(double min, double max) : IValueConverter
        //{
        //    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var values = (ObservableItemCollection<ObservableStruct<double>>)value;
        //        var ranges = new ObservableItemCollection<ObservableStruct<double>> 
        //        {
        //            new(values[0].Value - min)
        //        };
        //        for (int i = 1; i < values.Count; i++)
        //        {
        //            ranges.Add(new ObservableStruct<double>(values[i].Value - values[i - 1].Value));
        //        }
        //        ranges.Add(new(max - values[^1].Value));
        //        Debug.Assert(ranges.Count - 1 == values.Count);
        //        return ranges;
        //    }

        //    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        var ranges = (ObservableItemCollection<ObservableStruct<double>>)value;
        //        var values = new ObservableItemCollection<ObservableStruct<double>>();
        //        double sum = 0;
        //        for (int i = 0; i < ranges.Count - 1; i++)
        //        {
        //            sum += ranges[i].Value;
        //            values.Add(new ObservableStruct<double>(sum));
        //        }
        //        Debug.Assert(ranges.Count - 1 == values.Count);
        //        return values;
        //    }
        //}


        private static void RangespropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var @this = (ExtSliderView)d;
            //IValueConverter conv = new ValuesToRangesConverter(@this.Minimum, @this.Maximum);
            //@this.Values = (ObservableItemCollection<ObservableStruct<double>>)conv.ConvertBack(e.NewValue, typeof(ObservableItemCollection<ObservableStruct<double>>), null, null);
        }

        private static object RangespropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            var extSlider = (ExtSliderView)d;

            switch (baseValue)
            {
                case ObservableItemCollection<ObservableStruct<double>> arr:
                    if (arr.Count != extSlider.RangeCount)
                        throw new ArgumentException("The number of ranges must be equal the range count.");
                    break;

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
            //var @this = (ExtSliderView)d;
            ////var oldCollection = (ObservableItemCollection<ObservableStruct<double>>)e.OldValue;
            //var oldCollection = (ObservableCollection<double>)e.OldValue;
            //if (oldCollection is not null)
            //{
            //    //oldCollection.ItemsPropertyChanged -= @this.OnValuesChanged;
            //    oldCollection.CollectionChanged -= @this.OnValuesChanged;
            //}
            //var newCollection = (ObservableCollection<double>)e.NewValue;
            //if (newCollection is not null)
            //{
            //    //newCollection.ItemsPropertyChanged += @this.OnValuesChanged;
            //    newCollection.CollectionChanged += @this.OnValuesChanged;
            //}

            //var @this = (ExtSliderView)d;
            //IValueConverter conv = new ValuesToRangesConverter(@this.Minimum, @this.Maximum);
            //@this.Ranges = (ObservableItemCollection<ObservableStruct<double>>)conv.Convert(e.NewValue, typeof(ObservableItemCollection<ObservableStruct<double>>), null, null);

        }

        private void OnValuesChanged(object? sender, EventArgs e)
        {
            //OnPropertyChanged(new DependencyPropertyChangedEventArgs(ValuesProperty, Values, Values));
            //Values = Values;
            //SetValue(ValuesProperty, Values);
        }

        private static object ValuespropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            var extSlider = (ExtSliderView)d;

            switch (baseValue)
            {
                case ObservableCollection<double> arr:
                    if (arr.Count != extSlider.ValueCount)
                        throw new ArgumentException("The number of values must be equal the value count.");
                    break;

                case ObservableItemCollection<ObservableStruct<double>> arr:
                    if (arr.Count != extSlider.ValueCount)
                        throw new ArgumentException("The number of values must be equal the value count.");
                    break;

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


    public class CountToSliderConverter() 
        : CountToControlsConverter<SliderPart>((_, container, index) =>
        {
            var prevSlider = index > 0 ? container[index - 1] : null;
            var slider = new SliderPart(prevSlider) { Name = $"Slider{index}", Index = index };

            slider.Loaded += (s, e) => {
                // min max
                if (index > 0)
                    slider.SetBinding(SliderPart.ValueMinProperty, new Binding("Value") { Source = container[index - 1] });
                if (index < container.Count - 1)
                    slider.SetBinding(SliderPart.ValueMaxProperty, new Binding("Value") { Source = container[index + 1] });
            };

            return slider;
        })
    {
    }


    public class SubtractionConverter<T> : IMultiValueConverter
        where T : ISubtractionOperators<T, T, T>
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var a = (T)values[0];
            var b = (T)values[1];
            return a - b;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }


    public class OffsetConverter<T>(T? offset = default) : IValueConverter
        where T : IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            offset ??= (T)parameter;
            var t = (T)value;
            return t + offset;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            offset ??= (T)parameter;
            var t = (T)value;
            return t - offset;
        }
    }


    public class SelectConverter<TIn, TOut>(Func<TIn, TOut> converter) : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                TIn[] arr => arr.Select(converter).ToArray(),
                ObservableCollection<TIn> col => col.Select(converter).ToArray(),
                _ => throw new NotSupportedException(),
            };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class CountToControlsConverter<T>(CountToControlsConverter<T>.ConstructorHandler constructor) : IValueConverter
        where T : Control
    {
        public delegate T ConstructorHandler(object parameter, ObservableCollection<T> collection, int index);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //ObservableCollection<T> result = new(Enumerable.Range(0, (int)value).Select(i => constructor(parameter, i)));

            var result = new ObservableCollection<T>();

            for (int i = 0; i < (int)value; i++)
            {
                result.Add(constructor(parameter, result, i));
            }

            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ObservableStruct<T>(T value) : ObservableObject
        where T : struct
    {
        private T _value = value;

        public T Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }


    public class SliderValueIndexer : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //var arr = (ObservableItemCollection<ObservableStruct<double>>)values[0];
            //var arr = (ObservableCollection<double>)values[0];
            var arr = (double[])values[0];
            var index = (int)values[1];
            return (double)arr[index];
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ElementAtConverter<T>(int? index = null) : IMultiValueConverter
        where T : notnull
    {
        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            index ??= 
                parameter is int i ? i : 
                values.Length > 1 ? (int)values[1] : 
                null;

            if (index is null)
            {
                throw new ArgumentException("No index specified");
            }

            return (T)values[0] switch
            {
                T[] arr => arr[(Index)index],
                ObservableCollection<T> col => col[(Index)index],
                _ => throw new NotSupportedException(),
            };
        }

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
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

        //public static readonly DependencyProperty WholeValuesProperty =
        //    DependencyProperty.Register(
        //        "WholeValues",
        //        typeof(ObservableItemCollection<ObservableStruct<double>>),
        //        typeof(SliderPart));

        //public static readonly DependencyProperty WholeRangesProperty =
        //    DependencyProperty.Register(
        //        "WholeRanges",
        //        typeof(ObservableItemCollection<ObservableStruct<double>>),
        //        typeof(SliderPart));

        //public static readonly DependencyProperty WholeRangesProperty =
        //    DependencyProperty.Register(
        //        "WholeRanges",
        //        typeof(double[]),
        //        typeof(SliderPart));

        //public static readonly DependencyProperty WholeValuesProperty =
        //    DependencyProperty.Register(
        //        "WholeValues",
        //        typeof(double[]),
        //        typeof(SliderPart));

        public static readonly DependencyProperty ExtSliderProperty =
            DependencyProperty.Register(
                "ExtSlider",
                typeof(ExtSliderView),
                typeof(SliderPart));


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


        //public ObservableItemCollection<ObservableStruct<double>> WholeValues
        //{
        //    get => (ObservableItemCollection<ObservableStruct<double>>)GetValue(WholeValuesProperty);
        //    set => SetValue(WholeValuesProperty, value);
        //}

        //public ObservableItemCollection<ObservableStruct<double>> WholeRanges
        //{
        //    get => (ObservableItemCollection<ObservableStruct<double>>)GetValue(WholeRangesProperty);
        //    set => SetValue(WholeRangesProperty, value);
        //}

        //public double[] WholeValues
        //{
        //    get => (double[])GetValue(WholeValuesProperty);
        //    set => SetValue(WholeValuesProperty, value);
        //}

        //public double[] WholeRanges
        //{
        //    get => (double[])GetValue(WholeRangesProperty);
        //    set => SetValue(WholeRangesProperty, value);
        //}


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
        }


        private static void ValuepropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (SliderPart)d;
            //@this.ExtSlider.Values[@this.Index].Value = (double)e.NewValue;
            //@this.ExtSlider.Values[@this.Index] = (double)e.NewValue;
            var newValues = new double[@this.ExtSlider.ValueCount];
            Array.Copy(@this.ExtSlider.Values, newValues, @this.ExtSlider.ValueCount);
            newValues[@this.Index] = (double)e.NewValue;
            @this.ExtSlider.Values = newValues;
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
    }

}
