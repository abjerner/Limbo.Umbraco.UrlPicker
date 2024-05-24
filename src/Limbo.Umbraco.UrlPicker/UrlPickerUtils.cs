using System;
using System.Linq;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.UrlPicker;

internal static class UrlPickerUtils {

    public static string GetTypeAlias(Type type) {
        if (type.AssemblyQualifiedName is null) throw new PropertyNotSetException(nameof(type.AssemblyQualifiedName));
        return GetTypeAlias(type.AssemblyQualifiedName);
    }

    public static string GetTypeAlias(string typeName) {
        return typeName.Split(',').Take(2).Join(",");
    }

    public static void PrependLinkToDescription(ConfigurationField field, string text, string url) {
        string a = $"<a href=\"{url}\" class=\"btn btn-primary btn-xs limbo-urlpicker-button\" target=\"_blank\" rel=\"noreferrer noopener\">{text}</a>";
        field.Description = $"{a}\r\n{field.Description}";
    }

}