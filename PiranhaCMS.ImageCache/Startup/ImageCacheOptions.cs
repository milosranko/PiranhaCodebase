using PiranhaCMS.ImageCache.Models;

namespace PiranhaCMS.ImageCache.Startup;

public class ImageCacheOptionsBuilder
{
    public ImageQuality Quality { get; set; } = ImageQuality.Medium;
    public bool ConvertToWebP { get; set; }
    public string[] FileExtensionsForWebP { get; set; } = [".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp"];
    public string[] HorizontalSizePresets { get; set; } = ["800x600", "1280x720", "1920x1080"];
    public string[] VerticalSizePresets { get; set; } = ["360x640", "720x1280", "1080x1920"];
    public string DefaultImageCacheFolderName { get; set; } = "ImageCache";
    public Guid? DefaultImageCacheFolderId { get; set; }
}
