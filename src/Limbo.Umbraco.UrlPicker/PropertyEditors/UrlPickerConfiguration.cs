using Limbo.Umbraco.UrlPicker.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.UrlPicker.PropertyEditors;

/// <summary>
/// Class representing the configuration of a <see cref="UrlPickerEditor"/> data type.
/// </summary>
public class UrlPickerConfiguration : MultiUrlPickerConfiguration {

    // [CHANGE: Umbraco 13 -> 17 upgrade] ConfigurationField now only carries the key. The label, description and
    // editor UI of the field are declared in the client manifest instead (Client/src/property-editor/manifests.ts).
    // Related: documentation/UMBRACO-17-UPGRADE.md

    /// <summary>
    /// Gets or sets the information about the selected item converter.
    /// </summary>
    [ConfigurationField("converter")]
    public UrlPickerConverter? Converter { get; set; }

}
