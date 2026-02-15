using DigitalGarden.Shared.Components.Models.Enums;

namespace DigitalGarden.Shared.Components.Extensions;

public static class EnumExtensions
{
    public static string GetCssClassFromTextColor(this TextColor textColor)
    {
        return textColor switch
        {
            TextColor.Muted => "muted",
            TextColor.Success => "success",
            TextColor.Info => "info",
            TextColor.Warning => "warning",
            TextColor.Danger => "danger",
            _ => string.Empty
        };
    }
}
