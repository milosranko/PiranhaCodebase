namespace PiranhaCMS.Validators.Helpers;

public class RegularExpressions
{
    public const string PhoneRegexString = @"^\s*\+?\s*([0-9][\s-]*){7,}$";
    public const string ImageExtensionsRegexString = @"^.*\.(jpg|JPG|gif|GIF|png|PNG|svg|SVG)$";
}
