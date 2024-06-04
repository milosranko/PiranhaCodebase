using System.Text;

namespace PiranhaCMS.ImageCache.Tag;

internal struct ResizeParams
{
    public int w;
    public int? h;
    public bool? autorotate;
    public int? quality; // 0 - 100
    public string? format; // png, jpg, jpeg
    public ResizeMode? mode;
    //public string mode; // pad, max, crop, stretch

    public static string[] modes = ["pad", "max", "crop", "stretch"];

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"w: {w}, ");
        sb.Append($"h: {h}, ");
        sb.Append($"autorotate: {autorotate}, ");
        sb.Append($"quality: {quality}, ");
        sb.Append($"format: {format}, ");
        sb.Append($"mode: {mode}");

        return sb.ToString();
    }
}
