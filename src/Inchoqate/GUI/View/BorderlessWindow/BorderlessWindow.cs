using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using MvvmHelpers.Commands;

namespace Inchoqate.GUI.View.BorderlessWindow;

public class BorderlessWindow : Window
{
    static BorderlessWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(BorderlessWindow),
            new FrameworkPropertyMetadata(typeof(BorderlessWindow)));
    }

    public new static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(object),
            typeof(BorderlessWindow),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(BorderlessWindow),
            new FrameworkPropertyMetadata(
                new CornerRadius(10),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty TitlebarHeightProperty =
        DependencyProperty.Register(
            nameof(TitlebarHeight),
            typeof(double),
            typeof(BorderlessWindow),
            new FrameworkPropertyMetadata(
                30.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty ContentHeightProperty =
        DependencyProperty.Register(
            nameof(ContentHeight),
            typeof(double),
            typeof(BorderlessWindow),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty TitlebarContentProperty =
        DependencyProperty.Register(
            nameof(TitlebarContent),
            typeof(object),
            typeof(BorderlessWindow),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty MinimizeCommandProperty =
        DependencyProperty.Register(
            nameof(MinimizeCommand),
            typeof(ICommand),
            typeof(BorderlessWindow));

    public static readonly DependencyProperty RestoreCommandProperty =
        DependencyProperty.Register(
            nameof(RestoreCommand),
            typeof(ICommand),
            typeof(BorderlessWindow));

    public static readonly DependencyProperty ExitCommandProperty =
        DependencyProperty.Register(
            nameof(ExitCommand),
            typeof(ICommand),
            typeof(BorderlessWindow));

    public BorderlessWindow()
    {
        var heightBinding = new MultiBinding
        {
            Converter = new HeightBindingConverter(this),
            Mode = BindingMode.TwoWay
        };
        heightBinding.Bindings.Add(new Binding(nameof(ActualHeight)) { Source = this });
        heightBinding.Bindings.Add(new Binding(nameof(TitlebarHeight)) { Source = this });
        SetBinding(ContentHeightProperty, heightBinding);

        Loaded += OnLoaded;
    }

    public new object? Icon
    {
        get => (object?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public double TitlebarHeight
    {
        get => (double)GetValue(TitlebarHeightProperty);
        set => SetValue(TitlebarHeightProperty, value);
    }

    public double ContentHeight
    {
        get => (double)GetValue(ContentHeightProperty);
        set => SetValue(ContentHeightProperty, value);
    }

    public object TitlebarContent
    {
        get => GetValue(TitlebarContentProperty);
        set => SetValue(TitlebarContentProperty, value);
    }

    public ICommand MinimizeCommand
    {
        get => (ICommand)GetValue(MinimizeCommandProperty);
        set => SetValue(MinimizeCommandProperty, value);
    }

    public ICommand RestoreCommand
    {
        get => (ICommand)GetValue(RestoreCommandProperty);
        set => SetValue(RestoreCommandProperty, value);
    }

    public ICommand ExitCommand
    {
        get => (ICommand)GetValue(ExitCommandProperty);
        set => SetValue(ExitCommandProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var handle = new WindowInteropHelper(this).Handle;
        var hook = new HwndSourceHook(WindowProc);
        HwndSource.FromHwnd(handle)?.AddHook(hook);

        MinimizeCommand = new Command(
            () => WindowState = WindowState.Minimized
        );

        RestoreCommand = new Command(
            () => WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal
        );

        ExitCommand = new Command(Close);
    }

    private static IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case 0x0024:
                WmGetMinMaxInfo(hWnd, lParam);
                handled = true;
                break;
        }

        return IntPtr.Zero;
    }

    private static void WmGetMinMaxInfo(IntPtr hWnd, IntPtr lParam)
    {
        var mmi = (MinMaxInfo)Marshal.PtrToStructure(lParam, typeof(MinMaxInfo))!;
        var monitor = MonitorFromWindow(hWnd, 2);
        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MonitorInfo();
            GetMonitorInfo(monitor, monitorInfo);
            var rcWorkArea = monitorInfo.rcWork;
            var rcMonitorArea = monitorInfo.rcMonitor;
            mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
            mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
            mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
            mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
        }

        Marshal.StructureToPtr(mmi, lParam, true);
    }

    [DllImport("user32")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, MonitorInfo lpMi);

    [DllImport("User32")]
    private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int x, y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public Point ptReserved, ptMaxSize, ptMaxPosition, ptMinTrackSize, ptMaxTrackSize;
    }

#pragma warning disable CS0649, CS0414
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MonitorInfo
    {
        // ReSharper disable once UnusedMember.Local
        public int cbSize = Marshal.SizeOf(typeof(MonitorInfo));
        public Rect rcMonitor, rcWork;
        public int dwFlags = 0;
    }
#pragma warning restore CS0649, CS0414

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    private struct Rect
    {
        public int left, top, right, bottom;
    }
}