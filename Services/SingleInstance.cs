using System.Diagnostics;

namespace GpuSpoofer.Services;

public sealed class SingleInstance : IDisposable
{
    private readonly Mutex _mutex;
    public bool IsFirst { get; }

    public SingleInstance(string appName)
    {
        var name = $"Global\\{appName}_" + (Environment.UserName ?? "");
        try
        {
            _mutex = new Mutex(true, name, out var createdNew);
            IsFirst = createdNew;
        }
        catch
        {
            _mutex = new Mutex();
            IsFirst = true;
        }
    }

    public static void ShowExistingWindow()
    {
        var current = Process.GetCurrentProcess();
        foreach (var p in Process.GetProcessesByName(current.ProcessName))
        {
            if (p.Id != current.Id)
            {
                NativeMethods.SetForegroundWindow(p.MainWindowHandle);
                NativeMethods.ShowWindowAsync(p.MainWindowHandle, 9); // SW_RESTORE
                break;
            }
        }
    }

    public void Dispose() => _mutex.Dispose();
}

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
}
