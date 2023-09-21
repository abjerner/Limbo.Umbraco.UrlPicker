using System;
using System.Collections.Generic;
using Limbo.Umbraco.UrlPicker.PropertyEditors;
using Skybrud.Essentials.Collections;
using Skybrud.Essentials.Collections.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.UrlPicker.Converters;

/// <summary>
/// Abstract class representing a base implementation of the <see cref="IUrlPickerConverter"/>. Use this converter if
/// you wish to control how each item is converted.
/// </summary>
public abstract class UrlPickerItemConverterBase : IUrlPickerConverter {

    #region Properties

    /// <summary>
    /// Gets the friendly name of the converter.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets the icon of the item converter.
    /// </summary>
    public string? Icon { get; protected set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    protected UrlPickerItemConverterBase() {
        Name = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The friendly name of the converter.</param>
    protected UrlPickerItemConverterBase(string name) {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="name"/> and <paramref name="icon"/>.
    /// </summary>
    /// <param name="name">The friendly name of the converter.</param>
    /// <param name="icon">The icon of the converter.</param>
    protected UrlPickerItemConverterBase(string name, string? icon) {
        Name = name;
        Icon = icon;
    }

    #endregion

    #region Member methods

    /// <summary>
    /// Returns a type representing the type for the overall property value.
    /// </summary>
    /// <param name="propertyType">The property type.</param>
    /// <param name="config">The URL picker configuration.</param>
    /// <returns>An instance of <see cref="Type"/>.</returns>
    public Type GetType(IPublishedPropertyType propertyType, UrlPickerConfiguration config) {
        Type itemType = GetItemType(propertyType, config);
        return config.MaxNumber == 1 ? itemType : typeof(IEnumerable<>).MakeGenericType(itemType);
    }

    /// <summary>
    /// Returns a a common type for each item.
    /// </summary>
    /// <param name="propertyType">The property type.</param>
    /// <param name="config">The URL picker configuration.</param>
    /// <returns>An instance of <see cref="Type"/>.</returns>
    protected abstract Type GetItemType(IPublishedPropertyType propertyType, UrlPickerConfiguration config);

    /// <summary>
    /// Returns the converted value based on <paramref name="source"/>.
    /// </summary>
    /// <param name="owner">The property owner.</param>
    /// <param name="propertyType">The property type.</param>
    /// <param name="source">The source.</param>
    /// <param name="config">The URL picker configuration.</param>
    /// <returns>A collection with converted items.</returns>
    public object? Convert(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, UrlPickerConfiguration config) {
        bool single = config.MaxNumber == 1;
        return source switch {
            null => single ? null : ArrayUtils.Empty(GetItemType(propertyType, config)),
            Link link => ConvertItem(owner, propertyType, link, config),
            IEnumerable<Link> links => ConvertList(owner, propertyType, links, config),
            _ => source
        };
    }

    /// <summary>
    /// Methods responsible for converting a single <see cref="Link"/> item. If <see langword="null"/> is returned, the item is ignored.
    /// </summary>
    /// <param name="owner">The property owner.</param>
    /// <param name="propertyType">The property type.</param>
    /// <param name="source">The source.</param>
    /// <param name="config">The URL picker configuration.</param>
    /// <returns>An instance representing the converted item.</returns>
    protected abstract object? ConvertItem(IPublishedElement owner, IPublishedPropertyType propertyType, Link source, UrlPickerConfiguration config);

    /// <summary>
    /// Method responsible for convert a list of <see cref="Link"/> to a corresponding value.
    /// </summary>
    /// <param name="owner">The property owner.</param>
    /// <param name="propertyType">The property type.</param>
    /// <param name="source">The source.</param>
    /// <param name="config">The URL picker configuration.</param>
    /// <returns>A collection of converted items.</returns>
    protected object ConvertList(IPublishedElement owner, IPublishedPropertyType propertyType, IEnumerable<Link> source, UrlPickerConfiguration config) {

        List<object> temp = new();

        foreach (Link link in source) {
            if (ConvertItem(owner, propertyType, link, config) is { } converted) temp.Add(converted);
        }

        return temp.Cast(GetItemType(propertyType, config));

    }

    #endregion

}