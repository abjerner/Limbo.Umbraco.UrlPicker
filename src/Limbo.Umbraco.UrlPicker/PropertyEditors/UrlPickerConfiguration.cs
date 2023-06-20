using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.UrlPicker.PropertyEditors {

    /// <summary>
    /// Class representing the configuration of a <see cref="UrlPickerEditor"/> data type.
    /// </summary>
    public class UrlPickerConfiguration : MultiUrlPickerConfiguration {

        /// <summary>
        /// Gets or sets an instance of <see cref="JObject"/> representing the information about the selected item converter.
        /// </summary>
        [ConfigurationField("converter", "Converter", "/App_Plugins/Limbo.Umbraco.UrlPicker/Views/Converter.html?v={version}", Description = "Select a converter to control the type of the items returned by properties of this data type.")]
        public JToken? Converter { get; set; }

    }

}