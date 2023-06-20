using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors {

    [DataEditor(EditorAlias, "Limbo URL Picker", EditorView, ValueType = ValueTypes.Text, Group = "Limbo", Icon = "icon-link color-limbo")]
    public class UrlPickerEditor : MultiUrlPickerPropertyEditor {

        /// <summary>
        /// Gets the alias of the property editor.
        /// </summary>
        public const string EditorAlias = "Limbo.Umbraco.UrlPicker";

        /// <summary>
        /// Gets the view of the property editor.
        /// </summary>
        public const string EditorView = "multiurlpicker";

        private readonly IIOHelper _ioHelper;
        private readonly IEditorConfigurationParser _editorConfigurationParser;

        public UrlPickerEditor(IIOHelper ioHelper,
            IDataValueEditorFactory dataValueEditorFactory,
            IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, dataValueEditorFactory, editorConfigurationParser) {
            _ioHelper = ioHelper;
            _editorConfigurationParser = editorConfigurationParser;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() {
            return new UrlPickerConfigurationEditor(_ioHelper, _editorConfigurationParser);
        }

    }

}