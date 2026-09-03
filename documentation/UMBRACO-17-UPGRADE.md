# Umbraco 13 → Umbraco 17 upgrade

Recap of the work done on the `v17/dev` branch to move **Limbo.Umbraco.UrlPicker** from Umbraco 13 to Umbraco 17.

Before this upgrade `v17/dev` was byte-identical to `v13/main`: it still targeted `net8`, pinned Umbraco `[13.0.0,13.999)`, and shipped an AngularJS backoffice. Umbraco 14 replaced the backoffice wholesale, so a large part of the package had to be rewritten rather than retargeted.

## Result

| | Before | After |
|---|---|---|
| Target framework | `net8` | `net10.0` |
| Umbraco | `[13.0.0,13.999)` | `[17.0.0,17.9.9)` |
| Package version | `13.0.1` | `17.0.0-alpha000` |
| Backoffice | AngularJS views + controllers, registered via `IManifestFilter` | TypeScript/Lit, built with Vite, registered via `umbraco-package.json` |
| Backoffice API | `UmbracoAuthorizedApiController` + `[PluginController]` | Management API controller + own OpenAPI document |
| JSON | Newtonsoft.Json | System.Text.Json |

Verified with a clean `dotnet build -c Release /t:rebuild /t:pack` — 0 errors, and the resulting `.nupkg` contains both the assembly and all nine `staticwebassets/App_Plugins/Limbo.Umbraco.UrlPicker/*` files.

The public converter API is **unchanged**: `IUrlPickerConverter`, `UrlPickerConverterBase` and `UrlPickerItemConverterBase` keep the same members, so existing converter implementations only need recompiling against .NET 10.

---

## 1. Project file

`src/Limbo.Umbraco.UrlPicker/Limbo.Umbraco.UrlPicker.csproj`

- `net8` → `net10.0`, `VersionPrefix` `13.0.1` → `17.0.0`.
- `VersionSuffix` is now `alpha000` by default, so `pack` produces `Limbo.Umbraco.UrlPicker.17.0.0-alpha000.nupkg`. Debug builds keep their timestamp, appended to the same suffix (`17.0.0-alpha000.build202607291016`), which keeps them sorting *above* the plain alpha in NuGet's semver ordering. Drop the `VersionSuffix` line when the line goes stable.
- `StaticWebAssetBasePath` changed from `App_Plugins/$(AssemblyName)` to `/`. The Vite build now writes straight into `wwwroot/App_Plugins/Limbo.Umbraco.UrlPicker`, so the base path must not prepend `App_Plugins` a second time.
- Package references, all pinned to `[17.0.0,17.9.9)` as requested:
  - `Umbraco.Cms.Web.BackOffice` **removed** — the assembly no longer hosts the backoffice API.
  - `Umbraco.Cms.Web.Common`, `Umbraco.Cms.Api.Common` and `Umbraco.Cms.Api.Management` **added** — needed for `[BackOfficeRoute]`, `[MapToApi]` and the Swagger plumbing.
  - `Skybrud.Essentials` `1.1.57` → `1.1.68`.
- Removed the Web Compiler `compilerconfig.json` wiring (the LESS stylesheet is gone).
- Added `RestoreClient` / `BuildClient` targets that run `npm install` and `npm run build` in `Client/` as part of `dotnet build`. `-p:SkipClientBuild=true` opts out.
- Added an `IncludeClientAssets` target. **This one matters:** on a clean checkout `wwwroot/` does not exist when MSBuild evaluates the project, so the SDK's `wwwroot/**` glob matches nothing and the packed `.nupkg` silently ends up without any backoffice assets. The target re-adds whatever the client build produced before static web assets are resolved. Without it, the first build after a fresh clone produces a broken package while the second one is fine — an easy trap in CI.

## 2. Property editor (C#)

### `PropertyEditors/UrlPickerEditor.cs`

`DataEditorAttribute` lost its `name`, `view`, `Group` and `Icon` arguments — presentation now lives entirely in the client manifest. The attribute is down to `[DataEditor(EditorAlias, ValueType = ValueTypes.Json, ValueEditorIsReusable = true)]`, and the `EditorView` constant (`"multiurlpicker"`) was deleted.

`MultiUrlPickerPropertyEditor`'s constructor no longer takes `IEditorConfigurationParser`.

### `PropertyEditors/UrlPickerConfiguration.cs`

`ConfigurationField` in Umbraco 17 only carries a `Key` — no `Name`, `Description` or `View`. So:

```csharp
[ConfigurationField("converter")]
public UrlPickerConverter? Converter { get; set; }
```

The label, description and editor UI for the field moved to `Client/src/property-editor/manifests.ts`.

### `PropertyEditors/UrlPickerConfigurationEditor.cs`

Previously it looped over `Fields` to substitute `{version}` into the view URL and to prepend a documentation link to the field description. Neither property exists any more, so the class is now just the strongly typed configuration editor. `UrlPickerUtils.PrependLinkToDescription` was deleted; the documentation link is rendered by the converter element instead.

### `PropertyEditors/UrlPickerValueConverter.cs`

- Base constructor changed: `IPublishedSnapshotAccessor` and `IUmbracoContextAccessor` are gone, replaced by `IPublishedContentCache` and `IPublishedMediaCache`.
- `propertyType.DataType.Configuration` → `propertyType.DataType.ConfigurationObject`.
- `GetPropertyValueType` used to do `ConfigurationAs<UrlPickerConfiguration>()!`, which throws `InvalidCastException` if the data type is configured with anything else. It now pattern-matches on `ConfigurationObject` and falls back to the base implementation, matching what `ConvertIntermediateToObject` already did.
- The `GetPropertyCacheLevel` override was dropped — `PropertyCacheLevel.Snapshot` is obsolete in Umbraco 17 and the base class already returns the right level.

## 3. Backoffice API

`Controllers/UrlPickerController.cs`

`UmbracoAuthorizedApiController` and `[PluginController]` were removed in Umbraco 14. The controller is now a plain `ControllerBase` with:

```csharp
[ApiController]
[BackOfficeRoute("limbo/url-picker/api/v{version:apiVersion}")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi(UrlPickerPackage.ApiName)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = UrlPickerPackage.ApiName)]
```

- Endpoint moved from `/umbraco/backoffice/Limbo/UrlPicker/GetConverters` to **`/umbraco/limbo/url-picker/api/v1/converter`**.
- The hand-rolled Newtonsoft `JObject` was replaced by a typed `Models/UrlPickerConverterModel`, so the endpoint can be described properly in OpenAPI.
- The icon no longer gets a `color-<assembly>` suffix appended — those CSS classes do not exist in the new backoffice.

`Composers/UrlPickerComposers.cs` lost the `IManifestFilter` registration (the whole interface is gone) and gained the Swagger registration for the package's own document, served at `/umbraco/swagger/limbo-urlpicker/swagger.json`. `UrlPickerPackage.ApiName` (`"limbo-urlpicker"`) was added as the group name.

`UrlPickerManifestFilter.cs` was **deleted**.

## 4. JSON

Data type configuration is serialized with System.Text.Json from Umbraco 14 onwards, so the Newtonsoft converter would simply never run.

- `Json/Newtonsoft/UrlPickerConverterJsonConverter.cs` → `Json/SystemTextJson/UrlPickerConverterJsonConverter.cs`, rewritten against `Utf8JsonReader`/`Utf8JsonWriter`.
- The legacy tolerance is preserved: the stored value is read whether it is a bare string, `{ "key": … }` or `{ "type": … }`, and is always written back as `{ "type": … }`. Data types configured by the Umbraco 13 package therefore keep working.
- `[JsonProperty]` attributes were removed from `IUrlPickerConverter`; converters are no longer serialized directly.

The package no longer references Newtonsoft.Json itself.

## 5. Backoffice client (new)

The whole of `wwwroot/` — `Views/Converter.html`, `Views/ConverterOverlay.html`, `Scripts/Controllers/Converter.js`, `Scripts/Controllers/ConverterOverlay.js`, `Styles/Styles.less`, `Styles/Styles.css` — was deleted and replaced by `src/Limbo.Umbraco.UrlPicker/Client`:

```
Client/
  package.json          @umbraco-cms/backoffice ^17.5.3, TypeScript, Vite
  tsconfig.json
  vite.config.ts        bundles to ../wwwroot/App_Plugins/Limbo.Umbraco.UrlPicker
  public/
    umbraco-package.json        replaces IManifestFilter — declares one bundle extension
  src/
    constants.ts
    types.ts
    bundle.manifests.ts         bundle entry point
    repository/
      converter.repository.ts
      converter.server.data-source.ts
    property-editor/
      manifests.ts
      property-editor-ui-url-picker.element.ts
      property-editor-ui-converter.element.ts
```

### Manifests

Three extensions are registered:

| Alias | Type | Purpose |
|---|---|---|
| `Limbo.Umbraco.UrlPicker` | `propertyEditorSchema` | Client-side counterpart of the C# `UrlPickerEditor`. Declares `minNumber`, `maxNumber`, `ignoreUserStartNodes` and `converter` as data type settings. |
| `Limbo.Umbraco.UrlPicker.PropertyEditorUi` | `propertyEditorUi` | What editors see on a content property. Adds `overlaySize`, `hideAnchor` and `allowCultureSpecificDocumentLinks`, mirroring Umbraco's own Multi URL Picker UI. |
| `Limbo.Umbraco.UrlPicker.PropertyEditorUi.Converter` | `propertyEditorUi` | The **Converter** field on the data type. It deliberately has no `propertyEditorSchemaAlias`, so it can only be used as a configuration field and is never offered as a property editor in its own right. |

### `property-editor-ui-url-picker.element.ts`

The picker itself is functionally identical to Umbraco's, so this element wraps the same `<umb-input-multi-url>` component from `@umbraco-cms/backoffice/multi-url-picker` rather than reimplementing link picking. Umbraco does not export its own `UmbPropertyEditorUIMultiUrlPickerElement`, which is why the thin wrapper exists at all.

### `property-editor-ui-converter.element.ts`

Replaces the AngularJS `Converter.html` / `Converter.js` pair.

- Loads the converter list through the repository, and shows a `<uui-loader>` while it does.
- Opens Umbraco's built-in `UMB_ITEM_PICKER_MODAL` (which already gives search/filter, icons and descriptions) instead of the custom `ConverterOverlay` view.
- Renders the selection as a `<uui-ref-node>` with change/remove actions, and shows the "could not be found" warning when the persisted type alias no longer resolves — same behaviour as before.
- The legacy value normalisation from the old `init()` (bare string, `key` property, `, Version=` suffix) is preserved in a `normalize` static, kept deliberately symmetric with the server-side JSON converter. **If you change one, change the other.**

### `repository/`

Follows the standard Umbraco repository + data source split. `LimboUrlPickerConverterServerDataSource` uses `umbHttpClient` from `@umbraco-cms/backoffice/http-client` — the backoffice's shared, pre-authenticated client. A raw `fetch()` would be rejected with 401, since Umbraco 17 uses cookie-based backoffice auth where the `security` metadata on the request is what makes the client attach credentials. The call is wrapped in `tryExecute`, so failures surface as backoffice notifications rather than a hand-rolled error `<div>`.

No generated OpenAPI client is committed: generation requires a running Umbraco instance to read `swagger.json` from, and this repository is a standalone package with no host site. The Swagger document is registered anyway, so consumers who want a generated client can point `@hey-api/openapi-ts` at `/umbraco/swagger/limbo-urlpicker/swagger.json`.

## 6. Other

- `Converters/UrlPickerItemConverterBase.cs` — Skybrud.Essentials moved its non-generic `Cast`/`ToList` helpers to `Skybrud.Essentials.Collections.Enumerables.Extensions`.
- `.gitignore` — added `Client/node_modules/` and `wwwroot/` (both are build output now).
- `debug.bat` — output path `c:\nuget\Umbraco13` → `c:\nuget\Umbraco17`.
- `README.md` — version table, install instructions, a v13 branch link, a "Building from source" section, and the sample `LinkItem` converted from Newtonsoft to System.Text.Json.

---

## Not done / worth knowing

- **No browser verification.** Everything here compiles and packs cleanly, but the package has not been installed into a running Umbraco 17 site, so the backoffice UI is unverified at runtime. The two elements and the manifest aliases should be smoke-tested against a real instance before release.
- **No automated tests** — the repository has never had a test project, and none was added.
- **No localization file.** The package-specific strings ("Select converter", the "could not be found" warning, the documentation link) are hardcoded English, exactly as they were in the Umbraco 13 version. Only Umbraco's own `general_add` / `general_remove` terms are localized. Adding a `localization` manifest would be a straightforward follow-up.
- `dotnet restore` reports 44 `NU1902`/`NU1903` advisories. All of them come from Umbraco 17's own transitive dependencies (`MessagePack`, `MailKit`/`MimeKit`, `System.Security.Cryptography.Xml`, `Microsoft.OpenApi`), not from anything this package references directly.
- The documentation URL `https://packages.limbo.works/1b8ada3e` used by the converter field, and the `docs/v17/` URLs in the assembly metadata, point at pages that may not exist yet.
