﻿using Inchoqate.GUI.Events;
using Inchoqate.GUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.View
{
    public partial class EventTreeEditorView : UserControl
    {
        public static readonly DependencyProperty EditorTargetProperty = 
            DependencyProperty.Register(
                "EditorTarget",
                typeof(EventTree),
                typeof(EventTreeEditorView),
                new FrameworkPropertyMetadata(
                    null, 
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    EditorTargetPropertyChangedCallback));

        private static void EditorTargetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (EventTreeEditorView)d;
            var tree = (EventTree)e.NewValue;
            @this.Renderer.InitialEvent = tree.InitialEvent;
        }

        public EventTree EditorTarget
        {
            get => (EventTree)GetValue(EditorTargetProperty);
            set => SetValue(EditorTargetProperty, value);
        }


        public EventTreeEditorView()
        {
            InitializeComponent();
        }
    }
}
