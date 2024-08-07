﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Inchoqate.GUI.View;

public class TitlebarActionButtonOptionView : UserControl
{
    private const FrameworkPropertyMetadataOptions ContentPropertyMetadata =
        // Affects the measurements of this control
        FrameworkPropertyMetadataOptions.AffectsMeasure |
        // The action button menu aligns its options wrt. its contents
        FrameworkPropertyMetadataOptions.AffectsParentMeasure;

    public static readonly DependencyProperty IconProperty = 
        DependencyProperty.Register(
            nameof(Icon), 
            typeof(ImageSource), 
            typeof(TitlebarActionButtonOptionView),
            new FrameworkPropertyMetadata(
                null,
                ContentPropertyMetadata));

    public static readonly DependencyProperty TitleProperty = 
        DependencyProperty.Register(
            nameof(Title), 
            typeof(string), 
            typeof(TitlebarActionButtonOptionView),
            new FrameworkPropertyMetadata(
                "",
                ContentPropertyMetadata));

    public static readonly DependencyProperty IndicatorVisibilityProperty = 
        DependencyProperty.Register(
            nameof(IndicatorVisibility), 
            typeof(Visibility), 
            typeof(TitlebarActionButtonOptionView), 
            new FrameworkPropertyMetadata(
                Visibility.Collapsed,
                ContentPropertyMetadata));

    public static readonly DependencyProperty CommandBindingProperty = 
        DependencyProperty.Register(
            nameof(CommandBinding), 
            typeof(CommandBinding), 
            typeof(TitlebarActionButtonOptionView), 
            new FrameworkPropertyMetadata(
                null, 
                ContentPropertyMetadata));

    public static readonly DependencyProperty IndicatorContentProperty =
        DependencyProperty.Register(
            nameof(IndicatorContent),
            typeof(Visibility),
            typeof(TitlebarActionButtonOptionView),
            new FrameworkPropertyMetadata(
                Visibility.Collapsed,
                FrameworkPropertyMetadataOptions.AffectsRender));

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public CommandBinding CommandBinding
    {
        get => (CommandBinding)GetValue(CommandBindingProperty);
        set => SetValue(CommandBindingProperty, value);
    }

    public Visibility IndicatorVisibility
    {
        get => (Visibility)GetValue(IndicatorVisibilityProperty);
        set => SetValue(IndicatorVisibilityProperty, value);
    }

    public Visibility IndicatorContent
    {
        get => (Visibility)GetValue(IndicatorContentProperty);
        set => SetValue(IndicatorContentProperty, value);
    }
}