using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Skybrud.Essentials.Collections.Extensions;
using Umbraco.Cms.Core.Models;
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
        /// Returns the converted item based on <paramref name="source"/>.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="propertyType">The property type.</param>
        /// <param name="source">The source <see cref="Link"/>.</param>
        /// <returns>The converted item.</returns>
        object? Convert(IPublishedElement owner, IPublishedPropertyType propertyType, Link? source);

        /// <summary>
        /// Returns a collection of converted items based on <paramref name="source"/>.
        /// </summary>
        /// <param name="owner">The property owner.</param>
        /// <param name="propertyType">The property type.</param>
        /// <param name="source">The source.</param>
        /// <returns>A collection with converted items.</returns>
        object? ConvertList(IPublishedElement owner, IPublishedPropertyType propertyType, IEnumerable<Link> source) {

            // Get the type from the converter
            Type type = GetType(propertyType);

            // Initialize a new list ... of objects for now
            List<object> temp = new();

            // Iterate through the list of links
            foreach (Link link in source) {

                // Convert the item
                object? result = Convert(owner, propertyType, link);

                // Skip if null
                if (result is null) continue;

                // Append the result/item to the list
                temp.Add(result);

            }

            // Convert the list to a list of type "type"
            return temp.Cast(type).ToList(type);

        }

        /// <summary>
        /// Returns the <see cref="Type"/> of the items returned by this item converter.
        /// </summary>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The <see cref="Type"/> of the items returned by this item converter.</returns>
        Type GetType(IPublishedPropertyType propertyType);

    }

}