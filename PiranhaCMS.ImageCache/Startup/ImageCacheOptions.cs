namespace PiranhaCMS.ImageCache.Startup;

public class ImageCacheOptionsBuilder
{
    public bool ConvertToWebP { get; set; }
    public string[] FileExtensionsForWebP { get; set; } = ["jpg", "jpeg", "png"];
}
