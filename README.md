# Limbo URL Picker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/vpre/Limbo.Umbraco.MultiNodeTreePicker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.UrlPicker) [![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.UrlPicker.svg)](https://www.nuget.org/packages/Limbo.Umbraco.UrlPicker) [![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.urlpicker)

**Limbo.Umbraco.UrlPicker** adds a new property editor that extends Umbraco's default Multi URL Picker property editor by adding an extra **Converter** option.

The purpose of the converter is to control the C# type returned by the `.Value()` method or the corresponding property in a ModelsBuilder generated model. This is particular useful in a SPA/Headless Umbraco implementation, where the ModelsBuilder model can then be returned directly via a WebAPI endpoint. Eg. so a more disirable type can be returned instead of the default `Link` og `IEnumerable<Link>` value.

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="./LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>Umbraco 10 + Umbraco 11</td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>.NET 6</td>
  </tr>
</table>







<br /><br />

## Installation

The package targets Umbraco 10 and is only available via [**NuGet**][NuGetPackage]. To install the package, you can use either .NET CLI

```
dotnet add package Limbo.Umbraco.UrlPicker --version 1.0.1
```

or the NuGet Package Manager:

```
Install-Package Limbo.Umbraco.UrlPicker -Version 1.0.1
```





[NuGetPackage]: https://www.nuget.org/packages/Limbo.Umbraco.UrlPicker
[GitHubRelease]: https://github.com/abjerner/Limbo.Umbraco.UrlPicker
