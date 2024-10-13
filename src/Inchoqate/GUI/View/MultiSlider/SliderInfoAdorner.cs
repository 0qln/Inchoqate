using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Inchoqate.GUI.View.MultiSlider;

public class SliderInfoAdorner : Adorner
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(double),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(double),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowPrevRangeProperty =
        DependencyProperty.Register(
            nameof(ShowPrevRange),
            typeof(bool),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowNextRangeProperty =
        DependencyProperty.Register(
            nameof(ShowNextRange),
            typeof(bool),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowValueProperty =
        DependencyProperty.Register(
            nameof(ShowValue),
            typeof(bool),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty RangesProperty =
        DependencyProperty.Register(
            nameof(Ranges),
            typeof(double[]),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IndexProperty =
        DependencyProperty.Register(
            nameof(Index),
            typeof(int),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                -1,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty TextBrushProperty =
        DependencyProperty.Register(
            nameof(TextBrush),
            typeof(Brush),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty BackgroundBrushProperty =
        DependencyProperty.Register(
            nameof(BackgroundBrush),
            typeof(Brush),
            typeof(SliderInfoAdorner),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender));


    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool ShowNextRange
    {
        get => (bool)GetValue(ShowNextRangeProperty);
        set => SetValue(ShowNextRangeProperty, value);
    }

    public bool ShowPrevRange
    {
        get => (bool)GetValue(ShowPrevRangeProperty);
        set => SetValue(ShowPrevRangeProperty, value);
    }

    public bool ShowValue
    {
        get => (bool)GetValue(ShowValueProperty);
        set => SetValue(ShowValueProperty, value);
    }

    public double[] Ranges
    {
        get => (double[])GetValue(RangesProperty);
        set => SetValue(RangesProperty, value);
    }

    public int Index
    {
        get => (int)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public Brush TextBrush
    {
        get => (Brush)GetValue(TextBrushProperty);
        set => SetValue(TextBrushProperty, value);
    }

    public Brush BackgroundBrush
    {
        get => (Brush)GetValue(BackgroundBrushProperty);
        set => SetValue(BackgroundBrushProperty, value);
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
                    TextBrush)
                {
                    MaxTextWidth = maxTextWidth,
                };
#pragma warning restore CS0618 // Type or member is obsolete

                // transform 
                y += top
                    ? -formattedText.Height + (slider.ActualHeight - track.Thumb.ActualHeight)
                    : (slider.ActualHeight - track.Thumb.ActualHeight) + track.Thumb.ActualHeight;
                // draw
                if (top) drawingContext.DrawRectangle(BackgroundBrush, null, new Rect(x, y, formattedText.Width, formattedText.Height));
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