using System.Drawing;
using System.Runtime.InteropServices;

namespace GpuSpoofer.Services;

public sealed class TrayIcon : IDisposable
{
    private readonly nint _hwnd;
    private readonly uint _id;
    private bool _visible;

    public event Action? ShowClicked;
    public event Action? ExitClicked;

    public TrayIcon(string tooltip, Icon icon)
    {
        _id = 1;
        _hwnd = CreateMessageWindow();

        var data = new NOTIFYICONDATA
        {
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
            hWnd = _hwnd,
            uID = _id,
            uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
            uCallbackMessage = WM_TRAYICON,
            hIcon = icon.Handle,
            szTip = tooltip
        };

        Shell_NotifyIcon(NIM_ADD, ref data);
        _visible = true;
    }

    public void SetTooltip(string text)
    {
        var data = new NOTIFYICONDATA
        {
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
            hWnd = _hwnd,
            uID = _id,
            uFlags = NIF_TIP,
            szTip = text
        };
        Shell_NotifyIcon(NIM_MODIFY, ref data);
    }

    public void Dispose()
    {
        if (_visible)
        {
            var data = new NOTIFYICONDATA
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = _hwnd,
                uID = _id
            };
            Shell_NotifyIcon(NIM_DELETE, ref data);
            _visible = false;
        }
        if (_hwnd != nint.Zero)
            DestroyWindow(_hwnd);
    }

    private nint CreateMessageWindow()
    {
        var hInstance = GetModuleHandle(nint.Zero);
        var wc = new WNDCLASS
        {
            lpfnWndProc = WndProc,
            hInstance = hInstance,
            lpszClassName = "GpuSpooferTray"
        };
        RegisterClass(ref wc);
        return CreateWindowEx(0, "GpuSpooferTray", "", 0, 0, 0, 0, 0,
            nint.Zero, nint.Zero, hInstance, nint.Zero);
    }

    private nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WM_TRAYICON && (uint)lParam == WM_RBUTTONUP)
        {
            ShowContextMenu();
        }
        else if (msg == WM_TRAYICON && (uint)lParam == WM_LBUTTONDBLCLK)
        {
            ShowClicked?.Invoke();
        }
        else if (msg == WM_COMMAND && (uint)wParam == 1)
        {
            ShowClicked?.Invoke();
        }
        else if (msg == WM_COMMAND && (uint)wParam == 2)
        {
            ExitClicked?.Invoke();
        }
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private void ShowContextMenu()
    {
        var menu = CreatePopupMenu();
        AppendMenu(menu, MF_STRING, 1, "显示主窗口");
        AppendMenu(menu, MF_SEPARATOR, 0, "");
        AppendMenu(menu, MF_STRING, 2, "退出");

        GetCursorPos(out var pt);
        SetForegroundWindow(_hwnd);
        TrackPopupMenu(menu, TPM_RIGHTBUTTON, pt.X, pt.Y, 0, _hwnd, nint.Zero);
        PostMessage(_hwnd, 0, nint.Zero, nint.Zero);
        DestroyMenu(menu);
    }

    // Win32 API
    private const uint WM_TRAYICON = 0x8000;
    private const uint WM_RBUTTONUP = 0x0205;
    private const uint WM_LBUTTONDBLCLK = 0x0203;
    private const uint WM_COMMAND = 0x0111;
    private const uint NIF_MESSAGE = 0x1;
    private const uint NIF_ICON = 0x2;
    private const uint NIF_TIP = 0x4;
    private const uint NIM_ADD = 0x0;
    private const uint NIM_MODIFY = 0x1;
    private const uint NIM_DELETE = 0x2;
    private const uint MF_STRING = 0x0;
    private const uint MF_SEPARATOR = 0x800;
    private const uint TPM_RIGHTBUTTON = 0x2;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATA
    {
        public uint cbSize;
        public nint hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public nint hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WNDCLASS
    {
        public uint style;
        public WndProcDelegate lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
    }

    private delegate nint WndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

    [DllImport("user32.dll")]
    private static extern nint CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName,
        uint dwStyle, int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu,
        nint hInstance, nint lpParam);

    [DllImport("user32.dll")]
    private static extern bool DestroyWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport("kernel32.dll")]
    private static extern nint GetModuleHandle(nint lpModuleName);

    [DllImport("user32.dll")]
    private static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

    [DllImport("user32.dll")]
    private static extern nint CreatePopupMenu();

    [DllImport("user32.dll")]
    private static extern bool AppendMenu(nint hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

    [DllImport("user32.dll")]
    private static extern bool DestroyMenu(nint hMenu);

    [DllImport("user32.dll")]
    private static extern bool TrackPopupMenu(nint hMenu, uint uFlags, int x, int y,
        int nReserved, nint hWnd, nint prcRect);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }
}
