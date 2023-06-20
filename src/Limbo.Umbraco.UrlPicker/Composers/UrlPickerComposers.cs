using Limbo.Umbraco.UrlPicker.Converters;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.UrlPicker.Composers {

    /// <inheritdoc />
    public class UrlPickerComposer : IComposer {

        /// <inheritdoc />
        public void Compose(IUmbracoBuilder builder) {

            builder
                .WithCollectionBuilder<UrlPickerConverterCollectionBuilder>()
                .Add(() => builder.TypeLoader.GetTypes<IUrlPickerConverter>());

            builder.ManifestFilters().Append<UrlPickerManifestFilter>();

        }

    }

}