using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors;

// [CHANGE: Umbraco 13 -> 17 upgrade] The DataEditor attribute no longer carries a name, view, group or icon — those
// now live in the client manifest (Client/src/property-editor/manifests.ts) — and the base constructor no longer
// takes an IEditorConfigurationParser. Related: documentation/UMBRACO-17-UPGRADE.md

[DataEditor(EditorAlias, ValueType = ValueTypes.Json, ValueEditorIsReusable = true)]
public class UrlPickerEditor : MultiUrlPickerPropertyEditor {

    /// <summary>
    /// Gets the alias of the property editor.
    /// </summary>
    public const string EditorAlias = "Limbo.Umbraco.UrlPicker";

    private readonly IIOHelper _ioHelper;

    public UrlPickerEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory) : base(ioHelper, dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new UrlPickerConfigurationEditor(_ioHelper);
    }

}
