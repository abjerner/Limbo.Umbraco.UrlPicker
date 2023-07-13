using System;
using System.Collections.Generic;
using System.Linq;
using Limbo.Umbraco.UrlPicker.Converters;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Collections;
using Skybrud.Essentials.Json.Extensions;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Web;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.PropertyEditors {

    public class UrlPickerValueConverter : MultiUrlPickerValueConverter {

        #region Constructors

        private readonly UrlPickerConverterCollection _converterCollection;

        public UrlPickerValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, IProfilingLogger profilingLogger, IJsonSerializer jsonSerializer, IUmbracoContextAccessor umbracoContextAccessor, IPublishedUrlProvider publishedUrlProvider, UrlPickerConverterCollection converterCollection) : base(publishedSnapshotAccessor, profilingLogger, jsonSerializer, umbracoContextAccessor, publishedUrlProvider) {
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

            // Return "value" if item converter wasn't found
            if (!_converterCollection.TryGet(key, out IUrlPickerConverter? converter)) return value;

            return value switch {
                null => config.MaxNumber == 0 ? null : ArrayUtils.Empty(converter.GetType(propertyType)),
                Link link => converter.Convert(owner, propertyType, link),
                IEnumerable<Link> links => links.Select(x => converter.Convert(owner, propertyType, x)),
                _ => value
            };

        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {

            UrlPickerConfiguration config = propertyType.DataType.ConfigurationAs<UrlPickerConfiguration>()!;

            bool single = config.MaxNumber == 1;

            // Get the key of the converter
            string? key = GetConverterKey(config.Converter);
            if (string.IsNullOrWhiteSpace(key)) return base.GetPropertyValueType(propertyType);

            // Return "value" if item converter wasn't found
            if (!_converterCollection.TryGet(key, out IUrlPickerConverter? converter)) return base.GetPropertyValueType(propertyType);

            Type type = converter.GetType(propertyType);

            return single ? type : typeof(IEnumerable<>).MakeGenericType(type);

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

}