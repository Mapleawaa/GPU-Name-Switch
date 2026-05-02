using Microsoft.Win32;

namespace GpuSpoofer.Services;

public static class ThemeService
{
    private static bool _initialized;

    public static bool IsDarkMode { get; private set; }

    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        try
        {
            DetectTheme();
            SystemEvents.UserPreferenceChanged += (_, _) => DetectTheme();
            AppLogger.Info($"主题检测完成: {(IsDarkMode ? "深色" : "浅色")}");
        }
        catch (Exception ex)
        {
            AppLogger.Error("主题初始化失败", ex);
            IsDarkMode = false;
        }
    }

    private static void DetectTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            IsDarkMode = value is 0;
        }
        catch (Exception ex)
        {
            AppLogger.Error("主题检测失败", ex);
            IsDarkMode = false;
        }
    }

    public static System.Windows.Media.SolidColorBrush CardBg => IsDarkMode
        ? new(FromHex("#2D2D2D"))
        : new(FromHex("#FAFAFA"));

    public static System.Windows.Media.SolidColorBrush CardBorder => IsDarkMode
        ? new(FromHex("#444444"))
        : new(FromHex("#E0E0E0"));

    public static System.Windows.Media.SolidColorBrush TitleText => IsDarkMode
        ? new(FromHex("#E8E8E8"))
        : new(FromHex("#1A1A1A"));

    public static System.Windows.Media.SolidColorBrush BodyText => IsDarkMode
        ? new(FromHex("#BBBBBB"))
        : new(FromHex("#666666"));

    public static System.Windows.Media.SolidColorBrush StatusBarBg => IsDarkMode
        ? new(FromHex("#333333"))
        : new(FromHex("#F0F0F0"));

    public static System.Windows.Media.SolidColorBrush StatusText => IsDarkMode
        ? new(FromHex("#AAAAAA"))
        : new(FromHex("#555555"));

    public static System.Windows.Media.SolidColorBrush ListItemBg => IsDarkMode
        ? new(FromHex("#333333"))
        : new(FromHex("#FFFFFF"));

    public static System.Windows.Media.SolidColorBrush WindowBg => IsDarkMode
        ? new(FromHex("#1E1E1E"))
        : new(FromHex("#FFFFFF"));

    public static System.Windows.Media.SolidColorBrush SubtleText => IsDarkMode
        ? new(FromHex("#777777"))
        : new(FromHex("#888888"));

    public static System.Windows.Media.SolidColorBrush FaintText => IsDarkMode
        ? new(FromHex("#666666"))
        : new(FromHex("#AAAAAA"));

    public static System.Windows.Media.SolidColorBrush AccentBlue => IsDarkMode
        ? new(FromHex("#60CDFF"))
        : new(FromHex("#0078D4"));

    private static System.Windows.Media.Color FromHex(string hex) =>
        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
}
