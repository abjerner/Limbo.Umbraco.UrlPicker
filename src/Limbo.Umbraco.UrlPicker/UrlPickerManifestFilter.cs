using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.UrlPicker {

    /// <inheritdoc />
    public class UrlPickerManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {
            manifests.Add(new PackageManifest {
                AllowPackageTelemetry = true,
                PackageName = UrlPickerPackage.Name,
                Version = UrlPickerPackage.InformationalVersion,
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    $"/App_Plugins/{UrlPickerPackage.Alias}/Scripts/Controllers/Converter.js",
                    $"/App_Plugins/{UrlPickerPackage.Alias}/Scripts/Controllers/ConverterOverlay.js"
                },
                Stylesheets = new[] {
                    $"/App_Plugins/{UrlPickerPackage.Alias}/Styles/Styles.css"
                }
            });
        }

    }

}