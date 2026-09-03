using Asp.Versioning;
using Limbo.Umbraco.UrlPicker.Controllers;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Limbo.Umbraco.UrlPicker.Api;

/// <summary>
/// Generates short operation IDs (e.g. <c>GetConverters</c>) for the endpoints of this package.
/// </summary>
internal class UrlPickerOperationIdHandler : OperationIdHandler {

    public UrlPickerOperationIdHandler(IOptions<ApiVersioningOptions> apiVersioningOptions) : base(apiVersioningOptions) { }

    protected override bool CanHandle(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor) {
        return controllerActionDescriptor.ControllerTypeInfo == typeof(UrlPickerController);
    }

    public override string Handle(ApiDescription apiDescription) {
        return apiDescription.ActionDescriptor.RouteValues["action"] ?? string.Empty;
    }

}