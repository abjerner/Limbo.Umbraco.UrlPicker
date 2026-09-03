using Umbraco.Cms.Api.Management.OpenApi;

namespace Limbo.Umbraco.UrlPicker.Api;

internal class UrlPickerSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase {

    protected override string ApiName => UrlPickerApiConstants.Name;

}