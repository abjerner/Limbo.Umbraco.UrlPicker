using Limbo.Umbraco.UrlPicker.Api;
using Limbo.Umbraco.UrlPicker.Converters;
using Limbo.Umbraco.UrlPicker.Manifests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Skybrud.Essentials.Umbraco.Composing;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.UrlPicker.Composers;

/// <inheritdoc />
public class UrlPickerComposer : IComposer {

    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder) {

        builder.AddPackageManifestReader<UrlPickerPackageManifestReader>();

        builder
            .WithCollectionBuilder<UrlPickerConverterCollectionBuilder>()
            .Add(() => builder.TypeLoader.GetTypes<IUrlPickerConverter>());

        builder.Services.AddSingleton<IOperationIdHandler, UrlPickerOperationIdHandler>();

        builder.Services.Configure<SwaggerGenOptions>(ConfigureSwagger);

    }

    private static void ConfigureSwagger(SwaggerGenOptions options) {

        options.SwaggerDoc(UrlPickerApiConstants.Alias, new OpenApiInfo {
            Title = UrlPickerApiConstants.Name,
            Version = UrlPickerApiConstants.Version,
            Description = UrlPickerApiConstants.Description
        });

        options.OperationFilter<UrlPickerSecurityFilter>();
        options.DocumentFilter<UrlPickerDocumentFilter>();

    }

}