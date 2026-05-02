using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace GpuSpoofer.Services;

public static class AppLogger
{
    private static readonly string LogDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GpuSpoofer", "logs");

    private static readonly object _lock = new();

    static AppLogger()
    {
        try { Directory.CreateDirectory(LogDir); } catch { }
    }

    public static void Info(string msg, [CallerMemberName] string caller = "")
    {
        Write("INFO", msg, caller);
    }

    public static void Error(string msg, Exception? ex = null,
        [CallerMemberName] string caller = "")
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(msg);
        var current = ex;
        var depth = 0;
        while (current is not null)
        {
            sb.Append($"\n  [{depth}] {current.GetType().Name}: {current.Message}");
            if (current.StackTrace is not null)
                sb.Append($"\n  {current.StackTrace}");
            current = current.InnerException;
            depth++;
        }
        Write("ERROR", sb.ToString(), caller);
    }

    public static string LogPath => Path.Combine(LogDir, "app.log");

    private static void Write(string level, string msg, string caller)
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] [{caller}] {msg}";
        lock (_lock)
        {
            try { File.AppendAllText(LogPath, line + Environment.NewLine); } catch { }
            Debug.WriteLine(line);
            try { Console.WriteLine(line); } catch { }
        }
    }
}
