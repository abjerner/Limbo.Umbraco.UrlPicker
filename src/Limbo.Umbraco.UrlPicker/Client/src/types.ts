/**
 * An item converter available on the server, as returned by `GET /umbraco/limbo/url-picker/api/v1/converter`.
 *
 * Mirrors `Limbo.Umbraco.UrlPicker.Models.UrlPickerConverterModel`.
 */
export interface UrlPickerConverterModel {
	/** The type alias — eg. `My.Namespace.MyConverter, My.Assembly`. */
	type: string;
	/** The friendly name of the converter. */
	name: string;
	/** The icon of the converter. */
	icon: string;
	/** The name of the assembly declaring the converter. */
	description?: string | null;
	/** The full name of the assembly declaring the converter. */
	assembly?: string | null;
}

/**
 * The persisted value of the "converter" data type configuration field.
 *
 * Mirrors `Limbo.Umbraco.UrlPicker.Models.UrlPickerConverter`.
 */
export interface UrlPickerConverterValue {
	type: string;
}
