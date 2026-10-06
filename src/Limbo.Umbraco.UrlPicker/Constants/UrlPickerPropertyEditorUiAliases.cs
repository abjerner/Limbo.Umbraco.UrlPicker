using Limbo.Umbraco.UrlPicker.PropertyEditors;

namespace Limbo.Umbraco.UrlPicker.Constants;

/// <summary>
/// Static class with constants for the property editor UI aliases of this package.
/// </summary>
public class UrlPickerPropertyEditorUiAliases {

    /// <summary>
    /// The alias of the URL picker property editor UI.
    /// </summary>
    public const string UrlPicker = UrlPickerPropertyEditor.EditorUiAlias;

    /// <summary>
    /// The alias of the convert property editor UI.
    /// </summary>
    public const string Converter = $"{UrlPickerPackage.Alias}.PropertyEditorUi.Converter";

}