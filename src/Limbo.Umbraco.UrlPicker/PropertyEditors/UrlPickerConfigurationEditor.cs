using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors {

    public class UrlPickerConfigurationEditor : ConfigurationEditor<UrlPickerConfiguration> {

        public UrlPickerConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser) {

            foreach (ConfigurationField field in Fields) {

                if (field.View is not null) field.View = field.View.Replace("{version}", UrlPickerPackage.InformationalVersion);

                switch (field.Key) {

                    case "converter":
                        UrlPickerUtils.PrependLinkToDescription(
                            field,
                            "See the documentation &rarr;",
                            "https://packages.limbo.works/2e359b25"
                        );
                        break;

                }

            }

        }

    }

}