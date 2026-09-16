using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors;

[DataEditor(EditorAlias, ValueType = EditorValueType, ValueEditorIsReusable = true)]
public class UrlPickerPropertyEditor : MultiUrlPickerPropertyEditor {

    /// <summary>
    /// Gets the alias of the property editor.
    /// </summary>
    public const string EditorAlias = "Limbo.Umbraco.UrlPicker";

    public const string EditorUiAlias = "Limbo.Umbraco.UrlPicker.PropertyEditorUi";

    public const string EditorName = "Limbo URL Picker";

    public const string EditorGroup = "Limbo";

    public const string EditorIcon = "icon-link";

    public const string EditorValueType = ValueTypes.Json;

    private readonly IIOHelper _ioHelper;

    public UrlPickerPropertyEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory) : base(ioHelper, dataValueEditorFactory) {
        _ioHelper = ioHelper;
    }

    protected override IConfigurationEditor CreateConfigurationEditor() {
        return new UrlPickerConfigurationEditor(_ioHelper);
    }

}