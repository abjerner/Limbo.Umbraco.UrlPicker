using System;
using Limbo.Umbraco.UrlPicker.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Web;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors;

public class UrlPickerValueConverter : MultiUrlPickerValueConverter {

    #region Constructors

    private readonly ILogger<UrlPickerValueConverter> _logger;
    private readonly UrlPickerConverterCollection _converterCollection;

    public UrlPickerValueConverter(ILogger<UrlPickerValueConverter> logger, IPublishedSnapshotAccessor publishedSnapshotAccessor, IProfilingLogger profilingLogger, IJsonSerializer jsonSerializer, IUmbracoContextAccessor umbracoContextAccessor, IPublishedUrlProvider publishedUrlProvider, UrlPickerConverterCollection converterCollection) : base(publishedSnapshotAccessor, profilingLogger, jsonSerializer, umbracoContextAccessor, publishedUrlProvider) {
        _logger = logger;
        _converterCollection = converterCollection;
    }

    #endregion

    #region Member methods

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias.Equals(UrlPickerEditor.EditorAlias);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Snapshot;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object? inter, bool preview) {

        object? value = base.ConvertIntermediateToObject(owner, propertyType, cacheLevel, inter, preview);

        // Return "value" if the data type isn't configured with a converter
        if (propertyType.DataType.Configuration is not UrlPickerConfiguration config) return value;

        // Get the key of the converter
        string? key = GetConverterKey(config.Converter);
        if (string.IsNullOrWhiteSpace(key)) return value;

        // If the converter is found, we use it to convert the value received from the base value converter
        if (_converterCollection.TryGet(key, out IUrlPickerConverter? converter)) return converter.Convert(owner, propertyType, inter, config);

        // If a converter is specified, but isn't found, we write a debug message to the log, and return the value
        // received from the base value converter
        _logger.LogDebug("Converter with alias '{Alias}' not found.", key);
        return value;

    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {

        UrlPickerConfiguration config = propertyType.DataType.ConfigurationAs<UrlPickerConfiguration>()!;

        // Get the key of the converter
        string? key = GetConverterKey(config.Converter);
        if (string.IsNullOrWhiteSpace(key)) return base.GetPropertyValueType(propertyType);

        // Return "value" if item converter wasn't found
        if (!_converterCollection.TryGet(key, out IUrlPickerConverter? converter)) return base.GetPropertyValueType(propertyType);

        // As of v1.0 is up to the converter to return the correct type (eg. if a single or multi picker)
        return converter.GetType(propertyType, config);

    }

    private static string? GetConverterKey(JToken? token) {
        return token switch {
            null => null,
            JObject obj => obj.GetString("key"),
            _ => token.Type switch {
                JTokenType.String => token.ToString(),
                _ => null
            }
        };
    }

    #endregion

}