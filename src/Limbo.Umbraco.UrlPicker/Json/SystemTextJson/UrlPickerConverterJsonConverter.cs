using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Limbo.Umbraco.UrlPicker.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.UrlPicker.Json.SystemTextJson;

// [CHANGE: Umbraco 13 -> 17 upgrade] Data type configuration is serialized with System.Text.Json in Umbraco 14+,
// so this replaces the former Newtonsoft based converter in Json/Newtonsoft. The legacy formats are still read so
// that data types configured by older versions of the package keep working.
// Related: documentation/UMBRACO-17-UPGRADE.md

// [CHANGE: code review — legacy ", Version=" suffix was not stripped server side, so data types configured by older
// versions of the package resolved in the backoffice but silently fell through to the un-converted Link value]
// Related: Limbo.Umbraco.UrlPicker.csproj, README.md

/// <summary>
/// JSON converter for reading and writing the <c>converter</c> data type configuration value.
/// </summary>
/// <remarks>
/// Historically the value has been stored in three different shapes: as a bare string, as an object with a
/// <c>key</c> property, and as an object with a <c>type</c> property. All three are read, and the value is always
/// written back as <c>{ "type": "..." }</c>. Older versions also stored the full assembly qualified name, so any
/// <c>, Version=...</c> suffix is stripped on read — mirroring the <c>normalize</c> static in
/// <c>property-editor-ui-converter.element.ts</c>. If you change one, change the other.
/// </remarks>
public class UrlPickerConverterJsonConverter : JsonConverter<UrlPickerConverter?> {

    public override UrlPickerConverter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {

        switch (reader.TokenType) {

            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
                return Create(reader.GetString());

            case JsonTokenType.StartObject: {

                using JsonDocument document = JsonDocument.ParseValue(ref reader);

                JsonElement root = document.RootElement;

                if (TryGetString(root, "key", out string? key)) return Create(key);
                if (TryGetString(root, "type", out string? type)) return Create(type);

                return null;

            }

            default:
                throw new JsonException($"Unsupported token type '{reader.TokenType}'...");

        }

    }

    public override void Write(Utf8JsonWriter writer, UrlPickerConverter? value, JsonSerializerOptions options) {

        if (value is null || string.IsNullOrWhiteSpace(value.Type)) {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("type", value.Type);
        writer.WriteEndObject();

    }

    /// <summary>
    /// Initializes a new <see cref="UrlPickerConverter"/> from a persisted type alias. Older versions of the package
    /// stored the full assembly qualified name (eg. <c>Ns.Type, Asm, Version=1.0.0.0, Culture=neutral, ...</c>),
    /// whereas <c>UrlPickerUtils.GetTypeAlias</c> — which is what <c>UrlPickerConverterCollection</c> keys its
    /// lookup by — only uses the first two segments. Run the persisted value through the same method so legacy
    /// values still resolve to a converter.
    /// </summary>
    private static UrlPickerConverter? Create(string? type) {
        if (string.IsNullOrWhiteSpace(type)) return null;
        string alias = UrlPickerUtils.GetTypeAlias(type).Trim();
        return string.IsNullOrWhiteSpace(alias) ? null : new UrlPickerConverter(alias);
    }

    private static bool TryGetString(JsonElement element, string propertyName, out string value) {

        value = string.Empty;

        if (!element.TryGetProperty(propertyName, out JsonElement property)) return false;
        if (property.ValueKind != JsonValueKind.String) return false;

        string? str = property.GetString();
        if (string.IsNullOrWhiteSpace(str)) return false;

        value = str;
        return true;

    }

}
