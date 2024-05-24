using System.Diagnostics.CodeAnalysis;
using Limbo.Umbraco.UrlPicker.Converters;
using Limbo.Umbraco.UrlPicker.Json.Newtonsoft;
using Newtonsoft.Json;

namespace Limbo.Umbraco.UrlPicker.Models;

/// <summary>
/// Class describing a selected converter.
/// </summary>
[JsonConverter(typeof(UrlPickerConverterJsonConverter))]
public class UrlPickerConverter {

    /// <summary>
    /// Gets or sets the alias of the item converter type.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Initializes a new instance with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The alias of the item converter type.</param>
    [SetsRequiredMembers]
    public UrlPickerConverter(string type) {
        Type = type;
    }

    /// <summary>
    /// Creates a new instance based on the specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the converter.</typeparam>
    /// <returns>An instance of <see cref="UrlPickerConverter"/>.</returns>
    public static UrlPickerConverter Create<T>() where T : IUrlPickerConverter {
        return new UrlPickerConverter(UrlPickerUtils.GetTypeAlias(typeof(T)));
    }

}