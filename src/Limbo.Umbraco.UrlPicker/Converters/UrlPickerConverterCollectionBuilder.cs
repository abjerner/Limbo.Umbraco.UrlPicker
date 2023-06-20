using Umbraco.Cms.Core.Composing;

namespace Limbo.Umbraco.UrlPicker.Converters {

    internal sealed class UrlPickerConverterCollectionBuilder : LazyCollectionBuilderBase<UrlPickerConverterCollectionBuilder, UrlPickerConverterCollection, IUrlPickerConverter> {

        protected override UrlPickerConverterCollectionBuilder This => this;

    }

}