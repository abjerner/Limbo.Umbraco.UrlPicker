using System;
using Limbo.Umbraco.UrlPicker.PropertyEditors;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.UrlPicker.Converters;

/// <summary>
/// Abstract class representing a base implementation of the <see cref="IUrlPickerConverter"/>. Use this converter if
/// you wish to control the value returned for the overall URL picker value.
/// </summary>
public abstract class UrlPickerConverterBase : IUrlPickerConverter {

    #region Properties

    /// <summary>
    /// Gets the friendly name of the item converter.
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
    protected UrlPickerConverterBase() {
        Name = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The friendly name of the converter.</param>
    protected UrlPickerConverterBase(string name) {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="name"/> and <paramref name="icon"/>.
    /// </summary>
    /// <param name="name">The friendly name of the converter.</param>
    /// <param name="icon">The icon of the converter.</param>
    protected UrlPickerConverterBase(string name, string? icon) {
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
    public abstract Type GetType(IPublishedPropertyType propertyType, UrlPickerConfiguration config);

    /// <summary>
    /// Returns the converted value based on <paramref name="source"/>.
    /// </summary>
    /// <param name="owner">The property owner.</param>
    /// <param name="propertyType">The property type.</param>
    /// <param name="source">The source.</param>
    /// <param name="config">The URL picker configuration.</param>
    /// <returns>A collection with converted items.</returns>
    public abstract object? Convert(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, UrlPickerConfiguration config);

    #endregion

}