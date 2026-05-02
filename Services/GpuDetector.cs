using System.Management;
using GpuSpoofer.Models;
using Microsoft.Win32;

namespace GpuSpoofer.Services;

public static class GpuDetector
{
    private const string PciEnumPath = @"SYSTEM\CurrentControlSet\Enum\PCI";

    public static List<GpuInfo> DetectGpus()
    {
        var gpus = new List<GpuInfo>();

        try
        {
            AppLogger.Info("WMI 查询开始");
            using var searcher = new ManagementObjectSearcher(
                @"SELECT Name, PNPDeviceID, AdapterCompatibility
                  FROM Win32_VideoController");

            foreach (var obj in searcher.Get())
        {
            var name = obj["Name"]?.ToString() ?? "未知显卡";
            var pnpId = obj["PNPDeviceID"]?.ToString() ?? "";

            var gpu = new GpuInfo
            {
                Name = name,
                PnpDeviceId = pnpId,
                IsHidden = !IsRealGpu(name)
            };

            ParsePnpId(gpu, pnpId);
            ReadRegistryInfo(gpu);

            gpus.Add(gpu);
            }

            AppLogger.Info($"WMI 查询完成，共 {gpus.Count} 个设备 (真实: {gpus.Count(g => !g.IsHidden)}, 隐藏: {gpus.Count(g => g.IsHidden)})");
        }
        catch (Exception ex)
        {
            AppLogger.Error("GPU 检测失败", ex);
        }

        return gpus;
    }

    private static bool IsRealGpu(string name)
    {
        return name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase)
            || name.Contains("AMD", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Radeon", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Intel", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Arc", StringComparison.OrdinalIgnoreCase);
    }

    private static void ParsePnpId(GpuInfo gpu, string pnpId)
    {
        if (string.IsNullOrEmpty(pnpId))
            return;

        var parts = pnpId.Split('\\');
        if (parts.Length < 2)
            return;

        var idPart = parts[1];

        foreach (var segment in idPart.Split('&'))
        {
            if (segment.StartsWith("VEN_", StringComparison.OrdinalIgnoreCase))
                gpu.VendorId = segment[4..].ToUpper();
            else if (segment.StartsWith("DEV_", StringComparison.OrdinalIgnoreCase))
                gpu.DeviceId = segment[4..].ToUpper();
        }

        gpu.RegistryPath = $@"{PciEnumPath}\{parts[1]}\{parts[2]}";
    }

    private static void ReadRegistryInfo(GpuInfo gpu)
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(gpu.RegistryPath);
            if (key == null)
                return;

            var deviceDesc = key.GetValue("DeviceDesc")?.ToString() ?? "";
            gpu.OriginalDeviceDesc = deviceDesc;
            gpu.CurrentDeviceDesc = deviceDesc;

            var locationInfo = key.GetValue("LocationInformation")?.ToString() ?? "";
            gpu.PcieAddress = ParsePcieAddress(locationInfo, gpu.RegistryPath);
        }
        catch (Exception)
        {
            gpu.PcieAddress = "无法读取";
        }
    }

    private static string ParsePcieAddress(string locationInfoStr, string registryPath)
    {
        if (!string.IsNullOrEmpty(locationInfoStr))
        {
            var semi = locationInfoStr.LastIndexOf(';');
            var clean = semi >= 0 ? locationInfoStr[(semi + 1)..] : locationInfoStr;

            var match = System.Text.RegularExpressions.Regex.Match(clean,
                @"bus\s+(\d+).*?device\s+(\d+).*?function\s+(\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
                return $"Bus {match.Groups[1].Value}, Dev {match.Groups[2].Value}, Fn {match.Groups[3].Value}";
        }

        var lastSlash = registryPath.LastIndexOf('\\');
        return lastSlash >= 0 ? registryPath[(lastSlash + 1)..] : "未知";
    }
}
