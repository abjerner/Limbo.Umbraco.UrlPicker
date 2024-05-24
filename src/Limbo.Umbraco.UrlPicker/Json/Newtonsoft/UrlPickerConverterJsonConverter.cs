using System;
using Limbo.Umbraco.UrlPicker.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Limbo.Umbraco.UrlPicker.Json.Newtonsoft;

public class UrlPickerConverterJsonConverter : JsonConverter {

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {

        if (value is UrlPickerConverter converter && !string.IsNullOrWhiteSpace(converter.Type)) {
            new JObject { { "type", converter.Type } }.WriteTo(writer);
            return;
        }

        writer.WriteNull();

    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        switch (reader.TokenType) {
            case JsonToken.Null:
                return null;
            case JsonToken.String: {
                string? type = reader.Value as string;
                return string.IsNullOrWhiteSpace(type) ? null : new UrlPickerConverter(type);
            }
            case JsonToken.StartObject: {
                JObject json = JObject.Load(reader);
                if (json.GetString("key") is { Length: > 0 } key) return new UrlPickerConverter(key);
                if (json.GetString("type") is { Length: > 0 } type) return new UrlPickerConverter(type);
                return null;
            }
            default:
                throw new Exception($"Unsupported token type '{reader.TokenType}'...");
        }
    }

    public override bool CanConvert(Type objectType) {
        return false;
    }

}