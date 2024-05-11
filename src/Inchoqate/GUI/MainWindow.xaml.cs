﻿using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using OpenTK.Wpf;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using OpenTK.Compute.OpenCL;
using Inchoqate.Miscellaneous;
using Utillities.Wpf;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Shell;
using System.Globalization;
using Inchoqate.GUI.Titlebar;
using System.Collections.ObjectModel;
using Inchoqate.GUI.Main.Editor.FlowChart;

namespace Inchoqate.GUI
{
    public partial class MainWindow : BorderlessWindow
    {
        private readonly ILogger<MainWindow> _logger = FileLoggerFactory.CreateLogger<MainWindow>();


        public static readonly DependencyProperty IsFullscreenProperty = DependencyProperty.Register(
            "IsFullscreen", typeof(bool), typeof(MainWindow));

        public bool IsFullscreen
        {
            get => (bool)GetValue(IsFullscreenProperty);
            set => SetValue(IsFullscreenProperty, value);
        }


        public MainWindow()
        {
            InitializeComponent();

            RegisterCommand("TE_OpenFlowChartEditor_Command", OpenFlowChartEditor);
            RegisterCommand("TE_ToggleFullscreen_Command", ToggleFullscreen);

            E_Titlebar.Closed += (_, _) => Application.Current.Shutdown();

            _logger.LogInformation("Main window initiated.");
        }


        #region Seperator logic

        public bool Seperator_IsDragging { get; private set; }
        private Point Mouse_Position;

        private void Seperator_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Seperator_IsDragging = true;
        }

        private void Seperator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Seperator_IsDragging = false;
        }

        private void Seperator_Dragging(object sender, MouseEventArgs e)
        {
            var nextPosition = e.GetPosition(this);

            if (Seperator_IsDragging)
            {
                var mouseDelta = nextPosition - Mouse_Position;
                if (EditorInputs.Width >= mouseDelta.X)
                    EditorInputs.Width -= mouseDelta.X;
            }
        }

        #endregion


        private void This_MouseMove(object sender, MouseEventArgs e)
        {
            Seperator_Dragging(sender, e);

            Mouse_Position = e.GetPosition(this);
        }

        private void This_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Seperator_MouseUp(sender, e);
        }


        #region Action button logic

        public void OpenFlowChartEditor()
        {
            var window = new FlowChartWindow();
            window.Show();
        }

        public void ToggleFullscreen()
        {
            // TODO: full implementation.
            if (IsFullscreen)
            {
                E_Titlebar.E_ApplicationButtonsStack.Visibility = Visibility.Visible;
                IsFullscreen = false;
            }
            else
            {
                E_Titlebar.E_ApplicationButtonsStack.Visibility = Visibility.Collapsed;
                IsFullscreen = true;
            }
        }

        // Helper method to ease the syntax.
        private static DependencyProperty RegisterCommand(string name, Action action)
        {
            return DependencyProperty.Register(
                name, 
                typeof(ICommand), 
                typeof(MainWindow), 
                new(new ActionButtonCommand(action)));
        }

        #endregion
    }
}