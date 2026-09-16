using Limbo.Umbraco.UrlPicker.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.UrlPicker.PropertyEditors;

/// <summary>
/// Class representing the configuration of a <see cref="UrlPickerPropertyEditor"/> data type.
/// </summary>
public class UrlPickerConfiguration : MultiUrlPickerConfiguration {

    /// <summary>
    /// Gets or sets the information about the selected item converter.
    /// </summary>
    [ConfigurationField("converter")]
    public UrlPickerConverter? Converter { get; set; }

    /// <remarks>
    /// Umbraco has removed this from the parent <see cref="MultiUrlPickerConfiguration"/> class, as the overlay size
    /// option is now specified as part of the property editor UI rather than property editor schema. We do however
    /// still need this in order to set the configuration programmatically.
    /// </remarks>
    [ConfigurationField("overlaySize")]
    public string? OverlaySize { get; set; }

    /// <summary>
    /// Gets or sets whether the anchor/query string input field should be hidden in the overlay. Defaults to <c>false</c>.
    /// </summary>
    /// <remarks>
    /// Umbraco has removed this from the parent <see cref="MultiUrlPickerConfiguration"/> class, as the hide anchor
    /// option is now specified as part of the property editor UI rather than property editor schema. We do however
    /// still need this in order to set the configuration programmatically.
    /// </remarks>
    [ConfigurationField("hideAnchor")]
    public bool HideAnchor { get; set; }

}