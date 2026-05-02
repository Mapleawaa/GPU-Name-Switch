namespace GpuSpoofer.Converters;

public static class BrandColors
{
    public static string GetHex(string vendor) => vendor switch
    {
        "NVIDIA" => "#76B900",
        "AMD"    => "#ED1C24",
        "Intel"  => "#0071C5",
        _        => "#888888"
    };
}
