using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using GpuSpoofer.Services;
using Forms = System.Windows.Forms;

namespace GpuSpoofer;

public partial class App : System.Windows.Application
{
    private static SingleInstance? _single;
    private Forms.NotifyIcon? _trayIcon;

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
                System.Windows.MessageBox.Show($"发生致命错误:\n{ex?.Message}\n\n详情见日志:\n{AppLogger.LogPath}",
                    "GPU 名称切换器", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        DispatcherUnhandledException += (_, args) =>
        {
            AppLogger.Error("未处理异常 (Dispatcher)", args.Exception);
            args.Handled = true;
            if (!Debugger.IsAttached)
                System.Windows.MessageBox.Show($"发生错误:\n{args.Exception.Message}\n\n详情见日志:\n{AppLogger.LogPath}",
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

            // 系统托盘图标
            var iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
            if (!File.Exists(iconPath))
                iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");

            Icon icon;
            try
            {
                icon = new Icon(iconPath);
            }
            catch
            {
                AppLogger.Info("未找到图标文件，使用默认图标");
                icon = SystemIcons.Application;
            }

            _trayIcon = new Forms.NotifyIcon
            {
                Icon = icon,
                Text = "GPU 名称切换器",
                Visible = true
            };

            _trayIcon.DoubleClick += (_, _) => ShowMainWindow();
            _trayIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _trayIcon.ContextMenuStrip.Items.Add("显示主窗口", null, (_, _) => ShowMainWindow());
            _trayIcon.ContextMenuStrip.Items.Add("-");
            _trayIcon.ContextMenuStrip.Items.Add("退出", null, (_, _) =>
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                Shutdown();
            });

            AppLogger.Info("启动完成，打开主窗口");
        }
        catch (Exception ex)
        {
            AppLogger.Error("启动失败", ex);
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
