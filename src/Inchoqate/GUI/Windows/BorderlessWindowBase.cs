using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Inchoqate.GUI.Windows;

public class BorderlessWindowBase : Window
{
    private readonly ILogger _logger = FileLoggerFactory.CreateLogger<BorderlessWindowBase>();


    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(BorderlessWindowBase),
            new FrameworkPropertyMetadata(
                new CornerRadius(15),
                FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty TitlebarHeightProperty =
        DependencyProperty.Register(
            nameof(TitlebarHeight),
            typeof(double),
            typeof(BorderlessWindowBase),
            new FrameworkPropertyMetadata(
                30.0,
                FrameworkPropertyMetadataOptions.AffectsArrange));


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


    public BorderlessWindowBase()
    {
        Style = (Style)Application.Current.Resources["BorderlessWindowBaseStyle"];
        Loaded += BorderlessWindowBase_Loaded;
    }

    private static void BorderlessWindowBase_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is BorderlessWindowBase win)
        {
            win.FixSizingGlitch();
        }
    }

    private void FixSizingGlitch()
    {
        IntPtr handle = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));

        _logger.LogInformation("Fixed borderless window: {Title}", Title);
    }

    private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case 0x0024:
                WmGetMinMaxInfo(hwnd, lParam);
                handled = true;
                break;
        }
        return (IntPtr)0;
    }

    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO))!;
        int MONITOR_DEFAULTTONEAREST = 0x00000002;
        IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        if (monitor != IntPtr.Zero)
        {
            MONITORINFO monitorInfo = new();
            GetMonitorInfo(monitor, monitorInfo);
            RECT rcWorkArea = monitorInfo.rcWork;
            RECT rcMonitorArea = monitorInfo.rcMonitor;
            mmi.ptMaxPosition.x = System.Math.Abs(rcWorkArea.left - rcMonitorArea.left);
            mmi.ptMaxPosition.y = System.Math.Abs(rcWorkArea.top - rcMonitorArea.top);
            mmi.ptMaxSize.x = System.Math.Abs(rcWorkArea.right - rcWorkArea.left);
            mmi.ptMaxSize.y = System.Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
        }
        Marshal.StructureToPtr(mmi, lParam, true);
    }

    /// <summary>Construct a point of coordinates (x,y).</summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT(int x, int y)
    {
        /// <summary>x coordinate of point.</summary>
        public int x = x;
        /// <summary>y coordinate of point.</summary>
        public int y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor;
        public RECT rcWork;
        public int dwFlags = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public static readonly RECT Empty;
        public readonly int Width { get { return System.Math.Abs(right - left); } }
        public readonly int Height { get { return bottom - top; } }
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        public RECT(RECT rcSrc)
        {
            left = rcSrc.left;
            top = rcSrc.top;
            right = rcSrc.right;
            bottom = rcSrc.bottom;
        }
        public readonly bool IsEmpty { get { return left >= right || top >= bottom; } }
        public override readonly string ToString()
        {
            if (this == Empty) { return "RECT {Empty}"; }
            return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
        }
        public override readonly bool Equals(object? obj)
        {
            if (obj is not Rect) return false;
            return this == (RECT)obj;
        }
        /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
        public override readonly int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
        /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
        public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
        /// <summary> Determine if 2 RECT are different(deep compare)</summary>
        public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
    }

#pragma warning disable SYSLIB1054 

    [DllImport("user32")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    [DllImport("User32")]
    private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

#pragma warning restore SYSLIB1054

}