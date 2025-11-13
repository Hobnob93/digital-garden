using System.Text;

namespace DigitalGarden.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Turns an arbitrary string into a URL-suitable "slug"
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSlugString(this string str)
    {
        str = str
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder(str.Length);
        var previousWasHyphen = false;

        foreach (var @char in str)
        {
            if ((@char >= 'a' && @char <= 'z') || (@char >= '0' && @char <= '9'))
            {
                builder.Append(@char);
                previousWasHyphen = false;
            }
            else if (@char == ' ' || @char == '-' || @char == '_' || @char == '.' || @char == '/')
            {
                if (!previousWasHyphen && builder.Length > 0)
                {
                    builder.Append('-');
                    previousWasHyphen = true;
                }
            }
        }

        if (previousWasHyphen)
            builder.Remove(builder.Length - 1, length: 1);

        return builder.ToString();
    }
}
