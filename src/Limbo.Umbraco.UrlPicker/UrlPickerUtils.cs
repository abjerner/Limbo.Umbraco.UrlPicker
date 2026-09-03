using System;
using System.Linq;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Strings.Extensions;

namespace Limbo.Umbraco.UrlPicker;

// [CHANGE: Umbraco 13 -> 17 upgrade] PrependLinkToDescription was removed — ConfigurationField no longer has a
// Description, and the documentation link now lives in the client manifest. Related: documentation/UMBRACO-17-UPGRADE.md

internal static class UrlPickerUtils {

    public static string GetTypeAlias(Type type) {
        if (type.AssemblyQualifiedName is null) throw new PropertyNotSetException(nameof(type.AssemblyQualifiedName));
        return GetTypeAlias(type.AssemblyQualifiedName);
    }

    public static string GetTypeAlias(string typeName) {
        return typeName.Split(',').Take(2).Join(",");
    }

}
