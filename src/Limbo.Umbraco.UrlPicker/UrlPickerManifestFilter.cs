using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.UrlPicker;

/// <inheritdoc />
public class UrlPickerManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
            AllowPackageTelemetry = true,
            PackageId = UrlPickerPackage.Alias,
            PackageName = UrlPickerPackage.Name,
            Version = UrlPickerPackage.InformationalVersion,
            BundleOptions = BundleOptions.Independent,
            Scripts = [
                $"/App_Plugins/{UrlPickerPackage.Alias}/Scripts/Controllers/Converter.js",
                $"/App_Plugins/{UrlPickerPackage.Alias}/Scripts/Controllers/ConverterOverlay.js"
            ],
            Stylesheets = [
                $"/App_Plugins/{UrlPickerPackage.Alias}/Styles/Styles.css"
            ]
        };

        // Append the manifest
        manifests.Add(manifest);

    }

}