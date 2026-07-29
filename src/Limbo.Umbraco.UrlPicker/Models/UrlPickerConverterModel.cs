namespace Limbo.Umbraco.UrlPicker.Models;

// [CHANGE: Umbraco 13 -> 17 upgrade] The backoffice controller used to hand-roll a Newtonsoft JObject. It now
// returns this typed model so the endpoint can be described properly in the package's OpenAPI document.
// Related: documentation/UMBRACO-17-UPGRADE.md

/// <summary>
/// Class representing an available item converter, as returned by the package's management API.
/// </summary>
public class UrlPickerConverterModel {

    /// <summary>
    /// Gets or sets the type alias of the converter — eg. <c>My.Namespace.MyConverter, My.Assembly</c>.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Gets or sets the friendly name of the converter.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the icon of the converter.
    /// </summary>
    public required string Icon { get; set; }

    /// <summary>
    /// Gets or sets a description of the converter — the name of the assembly declaring it.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the full name of the assembly declaring the converter.
    /// </summary>
    public string? Assembly { get; set; }

}
