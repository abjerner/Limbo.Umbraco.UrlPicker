using System;
using Asp.Versioning;
using Limbo.Umbraco.UrlPicker.Converters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.UrlPicker.Composers;

// [CHANGE: Umbraco 13 -> 17 upgrade] IManifestFilter was removed in Umbraco 14 — the backoffice assets are now
// registered through Client/public/umbraco-package.json instead. In its place this composer registers the Swagger
// document for the package's own management API. Related: documentation/UMBRACO-17-UPGRADE.md

/// <inheritdoc />
public class UrlPickerComposer : IComposer {

    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder) {

        builder
            .WithCollectionBuilder<UrlPickerConverterCollectionBuilder>()
            .Add(() => builder.TypeLoader.GetTypes<IUrlPickerConverter>());

        builder.Services.AddSingleton<IOperationIdHandler, UrlPickerOperationIdHandler>();

        builder.Services.Configure<SwaggerGenOptions>(options => {

            options.SwaggerDoc(UrlPickerPackage.ApiName, new OpenApiInfo {
                Title = $"{UrlPickerPackage.Name} API",
                Version = "1.0",
                Description = $"Management API for the {UrlPickerPackage.Name} package."
            });

            options.OperationFilter<UrlPickerSecurityFilter>();

        });

    }

    /// <summary>
    /// Enables Umbraco backoffice authentication for the endpoints of this package.
    /// </summary>
    private class UrlPickerSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {
        protected override string ApiName => UrlPickerPackage.ApiName;
    }

    /// <summary>
    /// Generates short operation IDs (eg. <c>GetConverters</c>) for the endpoints of this package.
    /// </summary>
    private class UrlPickerOperationIdHandler : OperationIdHandler {

        public UrlPickerOperationIdHandler(IOptions<ApiVersioningOptions> apiVersioningOptions) : base(apiVersioningOptions) { }

        protected override bool CanHandle(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor) {
            return controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith("Limbo.Umbraco.UrlPicker.Controllers", StringComparison.OrdinalIgnoreCase) is true;
        }

        public override string Handle(ApiDescription apiDescription) {
            return $"{apiDescription.ActionDescriptor.RouteValues["action"]}";
        }

    }

}
