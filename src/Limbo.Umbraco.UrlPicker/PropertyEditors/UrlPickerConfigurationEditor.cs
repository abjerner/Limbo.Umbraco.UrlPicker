using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors;

// [CHANGE: Umbraco 13 -> 17 upgrade] Fields no longer have a View or Description to rewrite, so this class is now
// just the strongly typed configuration editor. Related: documentation/UMBRACO-17-UPGRADE.md

public class UrlPickerConfigurationEditor : ConfigurationEditor<UrlPickerConfiguration> {

    public UrlPickerConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) { }

}
