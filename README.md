# Limbo URL Picker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/abjerner/Limbo.Umbraco.UrlPicker/blob/v17/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/vpre/Limbo.Umbraco.UrlPicker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.UrlPicker)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.UrlPicker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.UrlPicker)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.urlpicker)
[![Limbo.Umbraco.UrlPicker at packages.limbo.works](https://img.shields.io/badge/limbo-packages-blue)](https://packages.limbo.works/limbo.umbraco.urlpicker/)

**Limbo.Umbraco.UrlPicker** adds a new property editor that extends Umbraco's default Multi URL Picker property editor by adding an extra **Converter** option.

The purpose of the converter is to control the C# type returned by the `.Value()` method or the corresponding property in a ModelsBuilder generated model. This is particular useful in a SPA/Headless Umbraco implementation, where the ModelsBuilder model can then be returned directly via a WebAPI endpoint. Eg. so a more disirable type can be returned instead of the default `Link` og `IEnumerable<Link>` value.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/abjerner/Limbo.Umbraco.UrlPicker/blob/v17/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>Umbraco 17</td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>.NET 10</td>
  </tr>
</table>







<br /><br />

## Installation

### Umbraco 17

The `v17.x` package targets Umbraco 17 and is only available via [**NuGet**][NuGetPackage]. To install the package, you can use either .NET CLI

```
dotnet add package Limbo.Umbraco.UrlPicker --version 17.0.0-alpha001
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.UrlPicker -Version 17.0.0-alpha001
```

### Other versions of Umbraco

- [**`v13/main`**](https://github.com/abjerner/Limbo.Umbraco.UrlPicker/tree/v13/main#installation) Umbraco 13
- ~~[**`v1/main`**](https://github.com/abjerner/Limbo.Umbraco.UrlPicker/tree/v1/main#installation) Umbraco 10, 11 and 12~~ <sub title="Umbraco 10, 11 and 12 have reached end-of-life"><sup>(EOL)</sup></sub>



<br /><br />

## Usage

Umbraco's default Multi URL Picker returns a single `Link` or a collection of `Link`. With **Limbo.Umbraco.UrlPicker**, the type for the link item can be controlled by implementing a custom converterer like in the example below, and then selecting the converter on your **Limbo URL Picker** data type.

```csharp
using System;
using Limbo.Umbraco.UrlPicker.Converters;
using Limbo.Umbraco.UrlPicker.PropertyEditors;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbracoTen.Packages.UrlPicker {

    public class LinkItemConverter : UrlPickerItemConverterBase {

        public LinkItemConverter() : base("Link Item Converter") {}

        protected override object? ConvertItem(IPublishedElement owner, IPublishedPropertyType propertyType, Link source, UrlPickerConfiguration config) {
            return new LinkItem(source);
        }

        protected override Type GetItemType(IPublishedPropertyType propertyType, UrlPickerConfiguration config) {
            return typeof(LinkItem);
        }

    }

}
```

```csharp
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models;

namespace UmbracoTen.Packages.UrlPicker {

    public class LinkItem {

        #region Properties

        public string? Name { get; }

        public string? Url { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Target { get; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LinkType Type { get; }

        #endregion

        #region Constructors

        public LinkItem(Link link) {
            Name = link.Name;
            Url = link.Url;
            Target = link.Target;
            Type = link.Type;
        }

        #endregion

    }

}
```

> **Note:** `JsonStringEnumConverter` writes the enum member name verbatim — `"Content"`, `"Media"`, `"External"`. The Umbraco 13 version of this sample used `Skybrud.Essentials`' `EnumCamelCaseConverter`, which wrote `"content"`, `"media"` and `"external"` instead. If your frontend relies on the lowercase form, reference a camel cased converter instead:
>
> ```csharp
> public class CamelCaseEnumConverter() : JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
> ```



<br /><br />

## Building from source

The backoffice part of the package is a TypeScript/Lit extension living in `src/Limbo.Umbraco.UrlPicker/Client`, so **Node.js 20+** is required to build the package. `dotnet build` runs `npm install` and `npm run build` for you and emits the result to `src/Limbo.Umbraco.UrlPicker/wwwroot/App_Plugins/Limbo.Umbraco.UrlPicker`.

```
dotnet build src/Limbo.Umbraco.UrlPicker
```

Pass `-p:SkipClientBuild=true` to build the C# only. Note that a package built that way will not contain the backoffice assets.

See [**documentation/UMBRACO-17-UPGRADE.md**](./documentation/UMBRACO-17-UPGRADE.md) for what changed when the package was moved from Umbraco 13 to Umbraco 17.



[NuGetPackage]: https://www.nuget.org/packages/Limbo.Umbraco.UrlPicker
[GitHubRelease]: https://github.com/abjerner/Limbo.Umbraco.UrlPicker
