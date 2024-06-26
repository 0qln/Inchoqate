﻿using GUI.Main.Editor.FlowChart;
using Inchoqate.GUI.Titlebar;
using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
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
using System.Windows.Shapes;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    /// <summary>
    /// Interaction logic for FlowChartWindow.xaml
    /// </summary>
    public partial class FlowChartWindow : BorderlessWindow
    {
        private readonly ILogger<MainWindow> _logger = FileLoggerFactory.CreateLogger<MainWindow>();


        public static readonly DependencyProperty AddGrayscaleNodeProperty = DependencyProperty.Register(
            "DP_AddGrayscaleNode", typeof(ICommand), typeof(FlowChartWindow));

        // Example of how to make a shortcut
        public static readonly RoutedCommand AddGrayscaleNodeCommand = new(
            "RC_AddGrayscaleNode", typeof(FlowChartWindow), [new KeyGesture(Key.N, ModifierKeys.Control)]);


        public static readonly DependencyProperty AddNoRedChannelNodeProperty = DependencyProperty.Register(
            "DP_AddNoRedChannelNode", typeof(ICommand), typeof(FlowChartWindow));


        public FlowChartWindow()
        {
            InitializeComponent();

            SetValue(AddGrayscaleNodeProperty, new ActionButtonCommand(AddNode<N_GrayScale>));
            SetValue(AddNoRedChannelNodeProperty, new ActionButtonCommand(AddNode<N_NoRedChannel>));

            _logger.LogInformation("Flowchart window initiated.");
        }


        public void AddNode<TNode>()
            where TNode : NodeModel, new()
        {
            var newNode = new TNode();
            E_FlowChartEditor.E_MainContainer.Items.Add(newNode);
        }
    }
}
