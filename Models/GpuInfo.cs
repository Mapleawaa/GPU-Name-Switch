namespace GpuSpoofer.Models;

public class GpuInfo
{
    public string Name { get; set; } = "";
    public string PnpDeviceId { get; set; } = "";
    public string PcieAddress { get; set; } = "";
    public string VendorId { get; set; } = "";
    public string DeviceId { get; set; } = "";
    public string RegistryPath { get; set; } = "";
    public string OriginalDeviceDesc { get; set; } = "";
    public string CurrentDeviceDesc { get; set; } = "";
    public bool IsHidden { get; set; }

    public string VendorName => VendorId switch
    {
        "10DE" => "NVIDIA",
        "1002" => "AMD",
        "8086" => "Intel",
        _ => "未知"
    };
}
