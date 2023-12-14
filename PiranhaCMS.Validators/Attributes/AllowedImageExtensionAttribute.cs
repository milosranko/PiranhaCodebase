using Piranha.Extend.Fields;
using PiranhaCMS.Validators.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PiranhaCMS.Validators.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AllowedImageExtensionAttribute : ValidationAttribute
{
	public override bool IsValid(object? value)
	{
		if (value == null) return true;

		if (value is DocumentField document)
			return Regex.IsMatch(document.Media.Filename, RegularExpressions.ImageExtensionsRegexString);

		if (value is ImageField image)
			return Regex.IsMatch(image.Media.Filename, RegularExpressions.ImageExtensionsRegexString);

		return true;
	}
}
