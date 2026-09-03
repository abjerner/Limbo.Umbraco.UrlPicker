using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.UrlPicker.Constants;
using Limbo.Umbraco.UrlPicker.PropertyEditors;
using Skybrud.Essentials.Umbraco.Constants;
using Skybrud.Essentials.Umbraco.Manifests.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.PropertyEditors;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.UrlPicker.Manifests;

public class UrlPickerPackageManifestReader : IPackageManifestReader {

    public static string Alias => UrlPickerPackage.Alias;

    public static string Name => UrlPickerPackage.Name;

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        IEnumerable<PackageManifest> manifests = [
            new() {
                Id = Alias,
                Name = Name,
                AllowTelemetry = true,
                Version = UrlPickerPackage.InformationalVersion,
                Extensions = [..GetExtensions()],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string>() {
                        {"@limbo/urlpicker/constants", $"/App_Plugins/{Alias}/Constants.js"},
                        {"@limbo/urlpicker/service", $"/App_Plugins/{Alias}/Service.js"},
                    }
                }
            }
        ];

        return Task.FromResult(manifests);

    }

    private static string CreateButton(string url, string text) {

        // Must be written in a single line as Umbraco will replace all newlines with <br /> tags, which will break the button

        return $"""
               <a href="{url}" target="_blank" rel="noopener noreferrer"><uui-button look="outline" compact label="{text}">&nbsp;{text} &rarr;&nbsp;</uui-button></a>
               """;

    }

    private static IEnumerable<IExtension> GetExtensions() {

        yield return new PropertyEditorSchemaExtension {
            Name = $"{Name}: URL Picker Property Editor Schema",
            Alias = UrlPickerEditor.EditorAlias,
            Meta = new PropertyEditorSchemaMeta {
                DefaultPropertyEditorUiAlias = UrlPickerEditor.EditorUiAlias,
                Settings = new PropertyEditorSettings {
                    Properties = [
                        new PropertyEditorSettingsProperty {
                            Alias = "minNumber",
                            Label = "Minimum number of items",
                            Description = "",
                            PropertyEditorUiAlias = UmbracoPropertyEditorUiAliases.Integer,
                            Config = [
                                new PropertyEditorConfigProperty { Alias = "min", Value = 0 }
                            ]
                        },
                        new PropertyEditorSettingsProperty {
                            Alias = "maxNumber",
                            Label = "Maximum number of items",
                            Description = "",
                            PropertyEditorUiAlias = UmbracoPropertyEditorUiAliases.Integer,
                            Config = [
                                new PropertyEditorConfigProperty { Alias = "min", Value = 0 }
                            ]
                        },
                        new PropertyEditorSettingsProperty {
                            Alias = "ignoreUserStartNodes",
                            Label = "Ignore user start nodes",
                            Description = "Selecting this option allows a user to choose nodes that they normally don’t have access to.",
                            PropertyEditorUiAlias = UmbracoPropertyEditorUiAliases.Toggle,
                        },
                        new PropertyEditorSettingsProperty {
                            Alias = "converter",
                            Label = "Converter",
                            Description = "Select a converter to control the type of the items returned by properties of this data type.<br /><br />" +
                                CreateButton("https://packages.limbo.works/1b8ada3e", "See the documentation"),
                            PropertyEditorUiAlias = UrlPickerPropertyEditorUiAliases.Converter,
                        },
                    ],
                    DefaultData = [
                        new PropertyEditorSettingsDefaultData { Alias = "minNumber", Value = 0 },
                        new PropertyEditorSettingsDefaultData { Alias = "maxNumber", Value = 0 }
                    ]
                },
            }
        };

        yield return new PropertyEditorUiExtension() {
            Alias = UrlPickerPropertyEditorUiAliases.UrlPicker,
            Name = $"{Name}: URL Picker Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/UrlPicker.js",
            Meta = new PropertyEditorUiMeta {
                Label = "Limbo URL Picker",
                PropertyEditorSchemaAlias = UrlPickerPropertyEditorAliases.UrlPicker,
                Icon = "icon-link",
                Group = "Limbo",
                SupportsReadOnly = true,
                Settings = new PropertyEditorSettings {
                    Properties = [
                        new PropertyEditorSettingsProperty {
                            Alias = "overlaySize",
                            Label = "Overlay Size",
                            Description = "Select the width of the overlay.",
                            PropertyEditorUiAlias = UmbracoPropertyEditorUiAliases.OverlaySize
                        },
                        new PropertyEditorSettingsProperty {
                            Alias = "hideAnchor",
                            Label = "Hide anchor/query string input",
                            Description = "Selecting this hides the anchor/query string input field in the link picker overlay.",
                            PropertyEditorUiAlias = UmbracoPropertyEditorUiAliases.Toggle
                        },
                        new PropertyEditorSettingsProperty {
                            Alias = "allowCultureSpecificDocumentLinks",
                            Label = "#linkPicker_configCultureSpecificDocumentLinksLabel",
                            Description = "{#linkPicker_configCultureSpecificDocumentLinksDescription}",
                            PropertyEditorUiAlias = UmbracoPropertyEditorUiAliases.Toggle
                        }
                    ]
                }
            }
        };

        yield return new PropertyEditorUiExtension {
            Alias = UrlPickerPropertyEditorUiAliases.Converter,
            Name = $"{Name}: Converter Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/Converter.js",
            Meta = new PropertyEditorUiMeta {
                Label = "Limbo URL Picker Converter",
                Icon = "icon-brackets",
                Group = "Limbo"
            },
        };

    }

}