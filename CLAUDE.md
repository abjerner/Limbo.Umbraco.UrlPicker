# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

An Umbraco package (`Limbo.Umbraco.UrlPicker`) that extends Umbraco's built-in **Multi URL Picker** with a **Converter** data type option. The converter controls the CLR type returned by `.Value()` / ModelsBuilder properties — the point is headless/SPA setups where a ModelsBuilder model is serialized straight out of a WebAPI endpoint, so `Link` / `IEnumerable<Link>` isn't the desired shape.

Two halves: a C# project in `src/Limbo.Umbraco.UrlPicker`, and a TypeScript/Lit backoffice extension in `src/Limbo.Umbraco.UrlPicker/Client`.

No test project exists.

## Build

Node.js 20+ is required — `dotnet build` invokes `npm install` + `npm run build` in `Client/` via the `RestoreClient`/`BuildClient` targets, emitting to `wwwroot/App_Plugins/Limbo.Umbraco.UrlPicker` (gitignored).

```
dotnet build src/Limbo.Umbraco.UrlPicker                       # C# + client
dotnet build src/Limbo.Umbraco.UrlPicker -p:SkipClientBuild=true  # C# only
./release.bat                                                   # Release rebuild + pack -> releases/nuget
./debug.bat                                                     # Debug rebuild + pack -> c:\nuget\Umbraco17 (Windows)
```

On macOS the `.bat` files won't run; use:
`dotnet build src/Limbo.Umbraco.UrlPicker -c Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget`

Iterating on just the client: `cd src/Limbo.Umbraco.UrlPicker/Client && npm run watch`.

Debug builds get an auto timestamped `VersionSuffix` (`buildyyyyMMddHHmm`); the release version comes from `<VersionPrefix>`.

**Packaging trap:** on a clean checkout `wwwroot/` doesn't exist at MSBuild evaluation time, so the SDK's `wwwroot/**` glob matches nothing and the `.nupkg` would silently ship without any backoffice assets. The `IncludeClientAssets` target re-adds them before static web assets are resolved. Don't remove it; after touching the client build, verify with `unzip -l <nupkg> | grep staticwebassets`.

`dotnet restore` emits ~44 NU1902/NU1903 advisories — all from Umbraco 17's own transitive dependencies, none from this package's direct references.

## Branches / versions

- `v1/main` — Umbraco 10–12
- `v13/main` — Umbraco 13 / .NET 8
- `v17/dev` — current: Umbraco `[17.0.0,17.9.9)` / .NET 10. See `documentation/UMBRACO-17-UPGRADE.md` for what the 13→17 port changed and why.

## Architecture

Everything hangs off one idea: subclass Umbraco's Multi URL Picker and post-process its output through a pluggable converter chosen per data type. The picker UI itself is deliberately *not* reimplemented.

**Registration** — `Composers/UrlPickerComposers.cs` type-scans for `IUrlPickerConverter` into `UrlPickerConverterCollectionBuilder` (lazy collection), and registers the Swagger document for the package's management API. Backoffice assets are registered by `Client/public/umbraco-package.json`, not from C#.

**Converter identity** — a converter is referenced by a *type alias*: `"Namespace.Type, AssemblyName"` (assembly-qualified name truncated to two segments — see `UrlPickerUtils.GetTypeAlias`). This alias is persisted in data type config, so renaming a converter class or its assembly breaks existing data types. `UrlPickerConverterCollection` builds a case-insensitive alias → instance lookup at startup.

**Property editor path**
- `UrlPickerEditor` (alias `Limbo.Umbraco.UrlPicker`) subclasses `MultiUrlPickerPropertyEditor`; only the configuration editor is swapped. The attribute carries no name/icon/group — those live in the client manifest.
- `UrlPickerConfiguration` extends `MultiUrlPickerConfiguration` with the single `converter` field. `[ConfigurationField]` only carries the key; label/description/UI come from `Client/src/property-editor/manifests.ts`.
- `UrlPickerValueConverter` calls `base.ConvertIntermediateToObject` first, then hands the resulting `Link`/`IEnumerable<Link>` to the configured converter. `GetPropertyValueType` delegates to the converter's `GetType` — the converter owns the declared type, including whether it's single or collection.

**Writing a converter** — derive from one of two bases, both in `Converters/`:
- `UrlPickerItemConverterBase` — override `ConvertItem` + `GetItemType`; the base handles single-vs-multi and builds the enumerable.
- `UrlPickerConverterBase` — take over the whole property value.

**Config persistence quirk** — the stored `converter` value has drifted across versions: it may be a bare string, `{ "key": ... }`, or `{ "type": ... }`, with or without a `, Version=` suffix. Both `Json/SystemTextJson/UrlPickerConverterJsonConverter.cs` (server) and the static `normalize` in `property-editor-ui-converter.element.ts` (client) collapse all of these to `{ type }`. Keep the two in sync when touching either.

**Backoffice client** — one bundle (`Client/src/bundle.manifests.ts`) registering three extensions: the `propertyEditorSchema` `Limbo.Umbraco.UrlPicker`, the content-editing `propertyEditorUi` (a thin wrapper around Umbraco's `<umb-input-multi-url>`), and the `Converter` config-field UI. The converter list comes from `GET /umbraco/limbo/url-picker/api/v1/converter` (`Controllers/UrlPickerController.cs`), reached through `Client/src/repository/` — always via `umbHttpClient` + `tryExecute`, never raw `fetch` (401 otherwise).

## Conventions

`src/.editorconfig` governs the C#: 4-space indent, K&R braces (`{` on the same line), file-scoped namespaces, nullable enabled, XML docs on public members (`#pragma warning disable 1591` where deliberately skipped). JSON is System.Text.Json throughout — the package no longer references Newtonsoft. `Skybrud.Essentials` supplies collection/string helpers.

Client code uses tabs and follows Umbraco's own file naming (`*.element.ts`, `*.repository.ts`, `*.data-source.ts`, `manifests.ts`); elements extend `UmbLitElement`.
