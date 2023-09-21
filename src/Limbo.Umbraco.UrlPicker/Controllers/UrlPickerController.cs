using System;
using System.Linq;
using Limbo.Umbraco.UrlPicker.Converters;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.Controllers;

[PluginController("Limbo")]
public class UrlPickerController : UmbracoAuthorizedApiController {

    private readonly UrlPickerConverterCollection _converterCollection;

    public UrlPickerController(UrlPickerConverterCollection converterCollection) {
        _converterCollection = converterCollection;
    }

    public object GetConverters() {
        return _converterCollection.Select(Map);
    }

    private static JObject Map(IUrlPickerConverter converter) {

        Type type = converter.GetType();

        JObject json = new() {
            { "assembly", type.Assembly.FullName },
            { "type", UrlPickerUtils.GetTypeName(type) },
            { "icon", $"{converter.Icon ?? "icon-box"} color-{type.Assembly.FullName?.Split('.')[0].ToLower()}" },
            { "name", converter.Name },
            { "description", type.AssemblyQualifiedName?.Split(new[] { ", Version" }, StringSplitOptions.None)[0] + ".dll" }
        };

        return json;

    }

}