﻿using System.Windows;
using System.Windows.Controls;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.View.Events;

public partial class EventTreeView : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(EventTreeViewModel),
            typeof(EventTreeView),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                EditorTargetPropertyChangedCallback));


    public EventTreeView()
    {
        InitializeComponent();
    }

    public EventTreeViewModel? ViewModel
    {
        get => (EventTreeViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private static void EditorTargetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // only now we attach an event system as viewmodel to the
        // editor target for realtime updates of the tree.
        // TODO: view model has to be removed later.
        var @this = (EventTreeView)d;
        var tree = (EventTreeViewModel)e.NewValue;
        @this.Head.ViewModel = (EventViewModel)tree.Initial;
        @this.Head.Tree = tree;
    }
}