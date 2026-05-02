namespace GpuSpoofer.Data;

public static class GpuPresets
{
    public record GpuPreset(string Name, string Vendor);

    public static List<GpuPreset> All { get; } =
    [
        // === NVIDIA GeForce RTX 50 系列 ===
        new("NVIDIA GeForce RTX 5090",           "NVIDIA"),
        new("NVIDIA GeForce RTX 5080",           "NVIDIA"),
        new("NVIDIA GeForce RTX 5070 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 5070",           "NVIDIA"),
        new("NVIDIA GeForce RTX 5060 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 5060",           "NVIDIA"),
        // === NVIDIA GeForce RTX 40 系列 ===
        new("NVIDIA GeForce RTX 4090",           "NVIDIA"),
        new("NVIDIA GeForce RTX 4080 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce RTX 4080",           "NVIDIA"),
        new("NVIDIA GeForce RTX 4070 Ti SUPER",  "NVIDIA"),
        new("NVIDIA GeForce RTX 4070 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 4070 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce RTX 4070",           "NVIDIA"),
        new("NVIDIA GeForce RTX 4060 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 4060",           "NVIDIA"),
        // === NVIDIA GeForce RTX 30 系列 ===
        new("NVIDIA GeForce RTX 3090 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 3090",           "NVIDIA"),
        new("NVIDIA GeForce RTX 3080 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 3080",           "NVIDIA"),
        new("NVIDIA GeForce RTX 3070 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 3070",           "NVIDIA"),
        new("NVIDIA GeForce RTX 3060 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 3060",           "NVIDIA"),
        new("NVIDIA GeForce RTX 3050",           "NVIDIA"),
        // === NVIDIA GeForce RTX 20 系列 ===
        new("NVIDIA GeForce RTX 2080 Ti",        "NVIDIA"),
        new("NVIDIA GeForce RTX 2080 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce RTX 2080",           "NVIDIA"),
        new("NVIDIA GeForce RTX 2070 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce RTX 2070",           "NVIDIA"),
        new("NVIDIA GeForce RTX 2060 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce RTX 2060",           "NVIDIA"),
        // === NVIDIA GeForce GTX 16 系列 ===
        new("NVIDIA GeForce GTX 1660 Ti",        "NVIDIA"),
        new("NVIDIA GeForce GTX 1660 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce GTX 1660",           "NVIDIA"),
        new("NVIDIA GeForce GTX 1650 SUPER",     "NVIDIA"),
        new("NVIDIA GeForce GTX 1650",           "NVIDIA"),
        // === NVIDIA GeForce GTX 10 系列 ===
        new("NVIDIA GeForce GTX 1080 Ti",        "NVIDIA"),
        new("NVIDIA GeForce GTX 1080",           "NVIDIA"),
        new("NVIDIA GeForce GTX 1070 Ti",        "NVIDIA"),
        new("NVIDIA GeForce GTX 1070",           "NVIDIA"),
        new("NVIDIA GeForce GTX 1060",           "NVIDIA"),
        new("NVIDIA GeForce GTX 1050 Ti",        "NVIDIA"),
        new("NVIDIA GeForce GTX 1050",           "NVIDIA"),
        // === NVIDIA Quadro / RTX 专业卡 ===
        new("NVIDIA RTX A6000",                  "NVIDIA"),
        new("NVIDIA RTX A5000",                  "NVIDIA"),
        new("NVIDIA RTX A4000",                  "NVIDIA"),
        new("NVIDIA Quadro RTX 8000",            "NVIDIA"),
        new("NVIDIA Quadro RTX 6000",            "NVIDIA"),
        new("NVIDIA Quadro RTX 4000",            "NVIDIA"),
        // === AMD Radeon RX 9000 系列 ===
        new("AMD Radeon RX 9070 XT",             "AMD"),
        new("AMD Radeon RX 9070",                "AMD"),
        new("AMD Radeon RX 9060 XT",             "AMD"),
        new("AMD Radeon RX 9060",                "AMD"),
        // === AMD Radeon RX 7000 系列 ===
        new("AMD Radeon RX 7900 XTX",            "AMD"),
        new("AMD Radeon RX 7900 XT",             "AMD"),
        new("AMD Radeon RX 7800 XT",             "AMD"),
        new("AMD Radeon RX 7700 XT",             "AMD"),
        new("AMD Radeon RX 7600 XT",             "AMD"),
        new("AMD Radeon RX 7600",                "AMD"),
        // === AMD Radeon RX 6000 系列 ===
        new("AMD Radeon RX 6950 XT",             "AMD"),
        new("AMD Radeon RX 6900 XT",             "AMD"),
        new("AMD Radeon RX 6800 XT",             "AMD"),
        new("AMD Radeon RX 6800",                "AMD"),
        new("AMD Radeon RX 6750 XT",             "AMD"),
        new("AMD Radeon RX 6700 XT",             "AMD"),
        new("AMD Radeon RX 6650 XT",             "AMD"),
        new("AMD Radeon RX 6600 XT",             "AMD"),
        new("AMD Radeon RX 6600",                "AMD"),
        new("AMD Radeon RX 6500 XT",             "AMD"),
        new("AMD Radeon RX 6400",                "AMD"),
        // === AMD Radeon RX 5000 系列 ===
        new("AMD Radeon RX 5700 XT",             "AMD"),
        new("AMD Radeon RX 5700",                "AMD"),
        new("AMD Radeon RX 5600 XT",             "AMD"),
        new("AMD Radeon RX 5500 XT",             "AMD"),
        // === AMD Radeon VII / Vega ===
        new("AMD Radeon VII",                    "AMD"),
        new("AMD Radeon RX Vega 64",             "AMD"),
        new("AMD Radeon RX Vega 56",             "AMD"),
        // === Intel Arc 系列 ===
        new("Intel Arc B580",                    "Intel"),
        new("Intel Arc B570",                    "Intel"),
        new("Intel Arc A770",                    "Intel"),
        new("Intel Arc A750",                    "Intel"),
        new("Intel Arc A580",                    "Intel"),
        new("Intel Arc A380",                    "Intel"),
        new("Intel Arc A310",                    "Intel"),
        // === Intel Iris / UHD ===
        new("Intel Iris Xe Graphics",            "Intel"),
        new("Intel UHD Graphics 770",            "Intel"),
        new("Intel UHD Graphics 730",            "Intel"),
        new("Intel UHD Graphics 630",            "Intel"),
    ];

    public static List<GpuPreset> GetByVendor(string vendor) =>
        All.Where(p => p.Vendor == vendor).ToList();

}
