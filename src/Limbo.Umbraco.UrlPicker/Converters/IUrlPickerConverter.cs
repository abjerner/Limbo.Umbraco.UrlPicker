using System;
using Limbo.Umbraco.UrlPicker.PropertyEditors;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Limbo.Umbraco.UrlPicker.Converters {

    /// <summary>
    /// Interface describing an item converter.
    /// </summary>
    public interface IUrlPickerConverter {

        /// <summary>
        /// Gets the friendly name of the item converter.
        /// </summary>
        [JsonProperty("name")]
        string Name { get; }

        /// <summary>
        /// Gets the icon of the item converter.
        /// </summary>
        [JsonProperty("icon")]
        public string? Icon => null;

        /// <summary>
        /// Returns the overall value type for properties using thís converter.
        /// </summary>
        /// <param name="propertyType">The property type.</param>
        /// <param name="config">The URL picker configuration.</param>
        /// <returns></returns>
        Type GetType(IPublishedPropertyType propertyType, UrlPickerConfiguration config);

        /// <summary>
        /// Returns the converted value based on <paramref name="source"/>.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="propertyType">The property type.</param>
        /// <param name="source">The source.</param>
        /// <param name="config">The URL picker configuration.</param>
        /// <returns>A collection with converted items.</returns>
        object? Convert(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, UrlPickerConfiguration config);

    }

}