using System.Collections.Generic;
using System.Reflection;
using Umbraco.Cms.Core.Manifest;

namespace Limbo.Umbraco.UrlPicker;

/// <inheritdoc />
public class UrlPickerManifestFilter : IManifestFilter {

    /// <inheritdoc />
    public void Filter(List<PackageManifest> manifests) {

        // Initialize a new manifest filter for this package
        PackageManifest manifest = new() {
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
        };

        // The "PackageId" property isn't available prior to Umbraco 12, and since the package is build against
        // Umbraco 10, we need to use reflection for setting the property value for Umbraco 12+. Ideally this
        // shouldn't fail, but we might at least add a try/catch to be sure
        try {
            PropertyInfo? property = manifest.GetType().GetProperty("PackageId");
            property?.SetValue(manifest, UrlPickerPackage.Alias);
        } catch {
            // We don't really care about the exception
        }

        // Append the manifest
        manifests.Add(manifest);

    }

}