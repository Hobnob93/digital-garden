namespace DigitalGarden.Shared.Components.Models;

public record SimpleCardData
(
    string FontAwesomeIcon,
    string Title,
    string? Description = null,
    string FontAwesomeType = "fa-solid"
);
