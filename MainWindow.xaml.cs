using System.Windows;
using System.Windows.Controls;
using GpuSpoofer.Data;
using GpuSpoofer.Models;
using GpuSpoofer.Services;
using SWM = System.Windows.Media;

namespace GpuSpoofer;

public partial class MainWindow : Window
{
    private List<GpuInfo> _detectedGpus = [];
    private List<GpuInfo> _realGpus = [];
    private List<GpuInfo> _hiddenGpus = [];
    private GpuInfo? _selectedGpu;
    private GpuPresets.GpuPreset? _selectedPreset;
    private bool _hiddenPanelVisible;

    public MainWindow()
    {
        InitializeComponent();
        AppLogger.Info("MainWindow 初始化完成");

        Closing += (_, e) =>
        {
            e.Cancel = true;
            Hide();
            AppLogger.Info("窗口最小化到托盘");
        };
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        AppLogger.Info("Window_Loaded 开始");
        try
        {
            ApplyTheme();
            AppLogger.Info("主题应用完成");
            RefreshGpuList();
            AppLogger.Info("GPU 列表刷新完成");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Window_Loaded 异常", ex);
            throw;
        }
    }

    private void ApplyTheme()
    {
        ThemeService.Initialize();
        var r = Resources;

        r["WinBg"]       = ThemeService.WindowBg;
        r["CardBg"]      = ThemeService.CardBg;
        r["CardBorderC"] = ThemeService.CardBorder;
        r["TitleText"]   = ThemeService.TitleText;
        r["BodyText"]    = ThemeService.BodyText;
        r["SubtleText"]  = ThemeService.SubtleText;
        r["FaintText"]   = ThemeService.FaintText;
        r["ListItemBg"]  = ThemeService.ListItemBg;
        r["StatusBg"]    = ThemeService.StatusBarBg;
        r["StatusText"]  = ThemeService.StatusText;
        r["Accent"]      = ThemeService.AccentBlue;
    }

    private void RefreshGpuList()
    {
        try
        {
            SetNormalStatus("正在检测显卡...");
            _detectedGpus = GpuDetector.DetectGpus();

            _realGpus = _detectedGpus.Where(g => !g.IsHidden).ToList();
            _hiddenGpus = _detectedGpus.Where(g => g.IsHidden).ToList();

            GpuListBox.ItemsSource = null;
            GpuListBox.ItemsSource = _realGpus;

            HiddenGpuListBox.ItemsSource = null;
            HiddenGpuListBox.ItemsSource = _hiddenGpus;

            UpdateHiddenButtonText();

            if (_realGpus.Count == 0)
            {
                if (_hiddenGpus.Count > 0)
                {
                    SetWarningStatus($"未检测到物理显卡，发现 {_hiddenGpus.Count} 个隐藏设备。请点「显示隐藏设备」查看。");
                }
                else
                {
                    SetWarningStatus("未检测到任何显示适配器。如果确有显卡，请确认以管理员身份运行。");
                }
            }
            else
            {
                SetNormalStatus($"检测到 {_realGpus.Count} 个显卡 — 请选择左侧显卡。");
                if (_realGpus.Count == 1 && _hiddenGpus.Count == 0)
                {
                    GpuListBox.SelectedIndex = 0;
                }
            }
        }
        catch (Exception ex)
        {
            SetWarningStatus($"检测失败: {ex.Message}");
        }
    }

    private void UpdateHiddenButtonText()
    {
        ToggleHiddenBtn.Content = _hiddenGpus.Count > 0
            ? $"显示隐藏设备 ({_hiddenGpus.Count})"
            : "显示隐藏设备";
    }

    private void ToggleHidden_Click(object sender, RoutedEventArgs e)
    {
        _hiddenPanelVisible = !_hiddenPanelVisible;
        HiddenPanel.Visibility = _hiddenPanelVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void GpuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedGpu = GpuListBox.SelectedItem as GpuInfo;
        if (_selectedGpu != null)
        {
            HiddenGpuListBox.SelectedIndex = -1;
        }
        UpdateApplyButtonState();

        if (_selectedGpu != null)
        {
            SetNormalStatus($"已选择: [{_selectedGpu.VendorName}] {_selectedGpu.Name}  —  PCIe {_selectedGpu.PcieAddress}");
            FilterPresets();
        }
    }

    private void HiddenGpuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedGpu = HiddenGpuListBox.SelectedItem as GpuInfo;
        if (_selectedGpu != null)
        {
            GpuListBox.SelectedIndex = -1;
        }
        UpdateApplyButtonState();

        if (_selectedGpu != null)
        {
            SetNormalStatus($"已选择: {_selectedGpu.Name}  —  PCIe {_selectedGpu.PcieAddress}");
            FilterPresets();
        }
    }

    private void VendorCheck_Changed(object sender, RoutedEventArgs e)
    {
        FilterPresets();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterPresets();
    }

    private void PresetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedPreset = PresetListBox.SelectedItem as GpuPresets.GpuPreset;
        if (_selectedPreset != null)
        {
            CustomNameBox.Text = _selectedPreset.Name;
        }
        UpdateApplyButtonState();
    }

    private void CustomNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateApplyButtonState();
    }

    private void FilterPresets()
    {
        var presets = GpuPresets.All.AsEnumerable();

        var vendors = new List<string>();
        if (ChkNvidia.IsChecked == true) vendors.Add("NVIDIA");
        if (ChkAmd.IsChecked == true) vendors.Add("AMD");
        if (ChkIntel.IsChecked == true) vendors.Add("Intel");
        presets = presets.Where(p => vendors.Contains(p.Vendor));

        var search = SearchBox.Text?.Trim();
        if (!string.IsNullOrEmpty(search))
        {
            presets = presets.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Vendor.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        PresetListBox.ItemsSource = null;
        PresetListBox.ItemsSource = presets.ToList();
    }

    private void UpdateApplyButtonState()
    {
        var hasCustomName = !string.IsNullOrWhiteSpace(CustomNameBox.Text);
        ApplyButton.IsEnabled = _selectedGpu != null && hasCustomName;
    }

    private void Apply_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedGpu == null)
        {
            SetWarningStatus("请先在左侧选择要修改的显卡。");
            return;
        }

        var newName = CustomNameBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(newName))
        {
            SetWarningStatus("请先选择或输入伪装名称。");
            return;
        }

        try
        {
            var oldName = RegistryModifier.ApplySpoof(_selectedGpu.RegistryPath, newName);
            SetSuccessStatus($"修改成功: {oldName} → {newName}  |  备份已保存");

            RebootButton.Visibility = Visibility.Visible;
            NeedRestartText.Visibility = Visibility.Visible;
            RefreshGpuList();
        }
        catch (UnauthorizedAccessException)
        {
            SetWarningStatus("权限不足 — 请右键以管理员身份运行此程序。");
        }
        catch (Exception ex)
        {
            SetWarningStatus($"修改失败: {ex.Message}");
        }
    }

    private void Restore_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedGpu == null)
        {
            SetWarningStatus("请先在左侧选择要恢复的显卡。");
            return;
        }

        try
        {
            RegistryModifier.Restore(_selectedGpu.RegistryPath);
            SetSuccessStatus("已恢复原始名称 — 重启后生效。");

            RebootButton.Visibility = Visibility.Visible;
            NeedRestartText.Visibility = Visibility.Visible;
            RefreshGpuList();
        }
        catch (UnauthorizedAccessException)
        {
            SetWarningStatus("权限不足 — 请右键以管理员身份运行此程序。");
        }
        catch (Exception ex)
        {
            SetWarningStatus($"恢复失败: {ex.Message}");
        }
    }

    private void Reboot_Click(object sender, RoutedEventArgs e)
    {
        var result = System.Windows.MessageBox.Show(
            "确定要立即重启系统吗？\n\n请确保已保存所有工作。",
            "确认重启",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                System.Diagnostics.Process.Start("shutdown", "/r /t 10 /c \"GPU 名称已修改，系统将在 10 秒后重启。\"");
                SetNormalStatus("系统将在 10 秒后重启...");
            }
            catch (Exception ex)
            {
                SetWarningStatus($"无法启动重启: {ex.Message}");
            }
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        RefreshGpuList();
    }

    private void WhyLink_Click(object sender, RoutedEventArgs e)
    {
        new WhyDialog().ShowDialog();
    }

    private void SetNormalStatus(string msg)
    {
        StatusText.Text = msg;
        StatusText.Foreground = ThemeService.StatusText;
    }

    private void SetWarningStatus(string msg)
    {
        StatusText.Text = msg;
        StatusText.Foreground = new SWM.SolidColorBrush(SWM.Color.FromRgb(0xD8, 0x3B, 0x01));
    }

    private void SetSuccessStatus(string msg)
    {
        StatusText.Text = msg;
        StatusText.Foreground = new SWM.SolidColorBrush(SWM.Color.FromRgb(0x10, 0x7C, 0x10));
    }
}
