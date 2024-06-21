using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PiranhaCMS.Common.Extensions;

public static class StringExtensions
{
    public static string TruncateAtWord(this string input, int length)
    {
        if (input == null || input.Length < length)
            return input;

        int iNextSpace = input.LastIndexOf(" ", length);

        return string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
    }

    public static string CropString(this string text, int length, bool smartcrop)
    {
        if (!String.IsNullOrEmpty(text))
        {
            // If string is shorter than desired length, return entire string
            if (length >= text.Length)
                return text;

            // If string should use smart crop, set length to index of closest blank space
            if (smartcrop)
                length = text.LastIndexOf(" ", length, length);

            // Crop and return string            
            return text.Substring(0, length) + "...";
        }

        return string.Empty;
    }

    public static string RemoveAccents(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return new string(
            input
            .Normalize(System.Text.NormalizationForm.FormD)
            .ToCharArray()
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        // the normalization to FormD splits accented letters in accents+letters
        // the rest removes those accents (and other non-spacing characters)
    }

    public static string ConvertNewLineToBR(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.Replace("\n", "<br>");
    }

    public static string SanitizeSearchString(this string input)
    {
        string result = input.ToLower().Trim();

        if (!string.IsNullOrEmpty(result))
        {
            // remove newlines and tabs
            result = Regex.Replace(result, @"\t|\n|\r", "");

            // remove not-supported characters (supported are: numbers, regular letters, hyphens, spaces)
            result = Regex.Replace(result, "[^0-9a-zA-Z\\s\\/\\._-]+", "");

            // remove double spaces (also trims)
            result = string.Join(" ", result.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        return result;
    }

    public static string AddOrReplaceQueryStringParameter(this string input, string name, string value, bool isArray = false)
    {
        var queryString = QueryHelpers.ParseQuery(input);

        if (queryString.Keys.Count == 0)
            return string.Empty;

        if (queryString.ContainsKey(name) && !isArray)
        {
            queryString.Remove(name);
            queryString.Add(name, value);
        }
        else if (queryString.ContainsKey(name) && isArray)
        {
            queryString[name].Append(value);
        }
        else
        {
            queryString.Add(name, value);
        }

        return QueryString.Create(queryString).ToString();
    }

    public static string RemoveQueryStringParameter(this string input, string name)
    {
        var queryString = QueryHelpers.ParseQuery(input);

        if (queryString.Keys.Count == 0)
            return string.Empty;

        queryString.Remove(name);

        return QueryString.Create(queryString).ToString();
    }
}
