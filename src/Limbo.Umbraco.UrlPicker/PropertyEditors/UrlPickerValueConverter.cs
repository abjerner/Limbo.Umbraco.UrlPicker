using System;
using Limbo.Umbraco.UrlPicker.Converters;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.DeliveryApi;
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

    public UrlPickerValueConverter(ILogger<UrlPickerValueConverter> logger, IPublishedSnapshotAccessor publishedSnapshotAccessor, IProfilingLogger profilingLogger, IJsonSerializer jsonSerializer, IUmbracoContextAccessor umbracoContextAccessor, IPublishedUrlProvider publishedUrlProvider, UrlPickerConverterCollection converterCollection, IApiContentNameProvider apiContentNameProvider, IApiMediaUrlProvider apiMediaUrlProvider, IApiContentRouteBuilder apiContentRouteBuilder) : base(publishedSnapshotAccessor, profilingLogger, jsonSerializer, umbracoContextAccessor, publishedUrlProvider, apiContentNameProvider, apiMediaUrlProvider, apiContentRouteBuilder) {
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

        // Get the alias of the converter, if any
        string? typeAlias = config.Converter?.Type;

        // If the converter is found, we use it to convert the value received from the base value converter
        if (typeAlias is not null && _converterCollection.TryGet(typeAlias, out IUrlPickerConverter? converter)) return converter.Convert(owner, propertyType, value, config);

        // If a converter is specified, but isn't found, we write a debug message to the log, and return the value
        // received from the base value converter
        _logger.LogDebug("Converter with alias '{Alias}' not found.", typeAlias);
        return value;

    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {

        UrlPickerConfiguration config = propertyType.DataType.ConfigurationAs<UrlPickerConfiguration>()!;

        // Get the alias of the converter, if any
        string? typeAlias = config.Converter?.Type;

        // Get the value type from the base method if a converter hasn't been selected
        if (string.IsNullOrWhiteSpace(typeAlias)) return base.GetPropertyValueType(propertyType);

        // Also get the value type from the base method if the converter isn't found
        if (!_converterCollection.TryGet(typeAlias, out IUrlPickerConverter? converter)) return base.GetPropertyValueType(propertyType);

        // As of v1.0 it is up to the converter to return the correct type (e.g. if a single or multi picker)
        return converter.GetType(propertyType, config);

    }

    #endregion

}