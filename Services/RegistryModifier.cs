using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace GpuSpoofer.Services;

public static class RegistryModifier
{
    private static readonly string BackupDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GpuSpoofer", "backups");

    public record BackupEntry(string RegistryPath, string OriginalDeviceDesc, string NewDeviceDesc,
        DateTime Timestamp);

    public static string ApplySpoof(string registryPath, string newGpuName)
    {
        using var key = Registry.LocalMachine.OpenSubKey(registryPath, writable: true)
            ?? throw new InvalidOperationException("无法打开 GPU 注册表项，请确认以管理员身份运行。");

        var original = key.GetValue("DeviceDesc")?.ToString() ?? "";

        // 备份原始值
        var backup = new BackupEntry(registryPath, original, newGpuName, DateTime.Now);
        SaveBackup(backup);

        // 直接写入纯文本名称 (覆盖 INF 引用格式)
        key.SetValue("DeviceDesc", newGpuName, RegistryValueKind.String);

        return original;
    }

    public static void Restore(string registryPath)
    {
        using var key = Registry.LocalMachine.OpenSubKey(registryPath, writable: true)
            ?? throw new InvalidOperationException("无法打开 GPU 注册表项。");

        // 查找最新备份
        var backups = LoadBackups()
            .Where(b => b.RegistryPath == registryPath)
            .OrderByDescending(b => b.Timestamp)
            .ToList();

        if (backups.Count == 0)
            throw new InvalidOperationException("未找到此 GPU 的备份记录。");

        var last = backups[0];
        key.SetValue("DeviceDesc", last.OriginalDeviceDesc, RegistryValueKind.String);
    }

    public static void WriteValue(string registryPath, string valueName, string value)
    {
        using var key = Registry.LocalMachine.OpenSubKey(registryPath, writable: true)
            ?? throw new InvalidOperationException("无法打开 GPU 注册表项。");
        key.SetValue(valueName, value, RegistryValueKind.String);
    }

    public static List<BackupEntry> LoadBackups()
    {
        if (!Directory.Exists(BackupDir))
            return [];

        var backups = new List<BackupEntry>();
        foreach (var file in Directory.GetFiles(BackupDir, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var entry = JsonSerializer.Deserialize<BackupEntry>(json);
                if (entry != null)
                    backups.Add(entry);
            }
            catch
            {
                // 跳过损坏的备份文件
            }
        }
        return backups;
    }

    public static BackupEntry? GetLatestBackup(string registryPath)
    {
        return LoadBackups()
            .Where(b => b.RegistryPath == registryPath)
            .MaxBy(b => b.Timestamp);
    }

    private static void SaveBackup(BackupEntry entry)
    {
        Directory.CreateDirectory(BackupDir);
        var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}"[..40] + ".json";
        var path = Path.Combine(BackupDir, fileName);
        File.WriteAllText(path, JsonSerializer.Serialize(entry, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}
