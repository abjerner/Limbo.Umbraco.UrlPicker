using System;
using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Limbo.Umbraco.UrlPicker.Converters;
using Limbo.Umbraco.UrlPicker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;

#pragma warning disable 1591

namespace Limbo.Umbraco.UrlPicker.Controllers;

// [CHANGE: Umbraco 13 -> 17 upgrade] UmbracoAuthorizedApiController and [PluginController] were removed in Umbraco
// 14. This is now a management API controller routed at /umbraco/limbo/url-picker/api/v1/converter and grouped into
// the package's own OpenAPI document. Related: documentation/UMBRACO-17-UPGRADE.md

[ApiController]
[BackOfficeRoute("limbo/url-picker/api/v{version:apiVersion}")]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi(UrlPickerPackage.ApiName)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = UrlPickerPackage.ApiName)]
public class UrlPickerController : ControllerBase {

    private readonly UrlPickerConverterCollection _converterCollection;

    public UrlPickerController(UrlPickerConverterCollection converterCollection) {
        _converterCollection = converterCollection;
    }

    /// <summary>
    /// Returns a list of the item converters available on the server.
    /// </summary>
    [HttpGet("converter")]
    [ProducesResponseType(typeof(IEnumerable<UrlPickerConverterModel>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<UrlPickerConverterModel>> GetConverters() {
        return Ok(_converterCollection.Select(Map).ToList());
    }

    private static UrlPickerConverterModel Map(IUrlPickerConverter converter) {

        Type type = converter.GetType();

        return new UrlPickerConverterModel {
            Assembly = type.Assembly.FullName,
            Type = UrlPickerUtils.GetTypeAlias(type),
            // The legacy "color-xxx" suffix is gone — the new backoffice colors icons via its own theming
            Icon = converter.Icon ?? "icon-box",
            Name = converter.Name,
            Description = type.AssemblyQualifiedName?.Split([", Version"], StringSplitOptions.None)[0] + ".dll"
        };

    }

}
