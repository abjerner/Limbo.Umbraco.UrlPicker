using Microsoft.OpenApi;
using Skybrud.Essentials.Time;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Limbo.Umbraco.UrlPicker.Api;

/// <summary>
/// This document filter is used to remove the <see cref="EssentialsTime"/> schema from the OpenAPI document for the
/// URL Picker API. This is necessary because the <see cref="EssentialsTime"/> type is represented as a string with a
/// date-time format, and we want to avoid including unnecessary object-like schema information in the generated
/// OpenAPI documentation. The filter only applies to the URL Picker API document.
/// </summary>
internal class UrlPickerDocumentFilter : IDocumentFilter {

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
        if (context.DocumentName != UrlPickerApiConstants.Alias) return;
        const string schemaKey = nameof(EssentialsTime);
        swaggerDoc.Components?.Schemas?.Remove(schemaKey);
    }

}