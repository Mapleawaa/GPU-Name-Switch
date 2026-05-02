using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using GpuSpoofer.Services;

namespace GpuSpoofer;

public partial class App : System.Windows.Application
{
    private static SingleInstance? _single;
    private TrayIcon? _trayIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppLogger.Info("=== GPU 名称切换器 启动 ===");
        AppLogger.Info($"进程: {Process.GetCurrentProcess().ProcessName}  PID: {Environment.ProcessId}");
        AppLogger.Info($"工作目录: {Environment.CurrentDirectory}");
        AppLogger.Info($"系统: {Environment.OSVersion}  .NET: {Environment.Version}");

        // 全局异常处理
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            AppLogger.Error("未处理异常 (AppDomain)", ex);
            if (!Debugger.IsAttached)
                System.Windows.MessageBox.Show(
                    $"发生致命错误:\n{ex?.Message}\n\n详情见日志:\n{AppLogger.LogPath}",
                    "GPU 名称切换器", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        DispatcherUnhandledException += (_, args) =>
        {
            AppLogger.Error("未处理异常 (Dispatcher)", args.Exception);
            args.Handled = true;
            if (!Debugger.IsAttached)
                System.Windows.MessageBox.Show(
                    $"发生错误:\n{args.Exception.Message}\n\n详情见日志:\n{AppLogger.LogPath}",
                    "GPU 名称切换器", MessageBoxButton.OK, MessageBoxImage.Warning);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            AppLogger.Error("未处理异常 (Task)", args.Exception);
            args.SetObserved();
        };

        try
        {
            // 单实例检查
            _single = new SingleInstance("GpuSpoofer");
            if (!_single.IsFirst)
            {
                AppLogger.Info("已有实例在运行，激活已有窗口");
                SingleInstance.ShowExistingWindow();
                Shutdown();
                return;
            }

            // 托盘图标
            var iconPath = Path.Combine(AppContext.BaseDirectory, "icon", "app.ico");
            if (!File.Exists(iconPath))
                iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
            if (!File.Exists(iconPath))
                iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");

            Icon icon;
            try
            {
                icon = new Icon(iconPath);
                AppLogger.Info($"图标加载成功: {iconPath}");
            }
            catch (Exception ex)
            {
                AppLogger.Error("图标加载失败，使用默认", ex);
                icon = SystemIcons.Application;
            }

            _trayIcon = new TrayIcon("GPU 名称切换器", icon);
            _trayIcon.ShowClicked += () =>
            {
                AppLogger.Info("托盘: 显示主窗口");
                ShowMainWindow();
            };
            _trayIcon.ExitClicked += () =>
            {
                AppLogger.Info("托盘: 退出");
                _trayIcon?.Dispose();
                Shutdown();
            };

            AppLogger.Info("启动完成，打开主窗口");

            // 手动创建主窗口 (不用 StartupUri 避免 BAML 加载时序问题)
            CreateMainWindow();
        }
        catch (Exception ex)
        {
            AppLogger.Error("启动失败", ex);
            throw;
        }
    }

    private static void CreateMainWindow()
    {
        try
        {
            AppLogger.Info("开始创建 MainWindow");
            var win = new MainWindow();
            Current.MainWindow = win;

            win.Closing += (_, e) =>
            {
                e.Cancel = true;
                win.Hide();
                AppLogger.Info("窗口最小化到托盘");
            };

            win.Show();
            AppLogger.Info("MainWindow.Show() 完成");
        }
        catch (Exception ex)
        {
            AppLogger.Error("创建 MainWindow 失败", ex);
            throw;
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        AppLogger.Info("程序退出");
        _trayIcon?.Dispose();
        _single?.Dispose();
        base.OnExit(e);
    }

    public static void ShowMainWindow()
    {
        if (Current.MainWindow == null)
        {
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Closed += (_, _) => Current.MainWindow = null;
        }

        Current.MainWindow.Show();
        Current.MainWindow.WindowState = WindowState.Normal;
        Current.MainWindow.Activate();
    }
}
