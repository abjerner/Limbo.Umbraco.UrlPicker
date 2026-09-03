import { html, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UMB_PROPERTY_CONTEXT } from "@umbraco-cms/backoffice/property";
import {
	UMB_VALIDATION_EMPTY_LOCALIZATION_KEY,
	UmbFormControlMixin,
} from "@umbraco-cms/backoffice/validation";

// Registers the <umb-input-multi-url> custom element that this editor is built on.
import "@umbraco-cms/backoffice/multi-url-picker";

/**
 * Property editor UI for the Limbo URL Picker.
 *
 * The picker itself is identical to Umbraco"s own Multi URL Picker — the package only differs in the extra
 * "Converter" option on the data type, which is applied server side. This element therefore wraps the same
 * `<umb-input-multi-url>` component that the built-in editor uses.
 *
 * @element limbo-url-picker-element
 */
export class LimboUrlPickerElement extends UmbFormControlMixin(UmbLitElement) {

	static properties = {
		config: { attribute: false },
		readonly: { type: Boolean, reflect: true },
		mandatory: { type: Boolean },
		mandatoryMessage: { type: String },

		overlaySize: { state: true },
		hideAnchor: { state: true },
		documentLinksConfig: { state: true },
		min: { state: true },
		max: { state: true },
		alias: { state: true },
		variantId: { state: true },
	};

	#config;

	constructor() {

		super();

		this.readonly = false;
		this.mandatory = false;
		this.mandatoryMessage = UMB_VALIDATION_EMPTY_LOCALIZATION_KEY;

		this.overlaySize = undefined;
		this.hideAnchor = undefined;
		this.documentLinksConfig = undefined;
		this.min = 0;
		this.max = Infinity;
		this.alias = undefined;
		this.variantId = undefined;

		this.consumeContext(UMB_PROPERTY_CONTEXT, (context) => {
			this.observe(context?.alias, (alias) => {
				this.alias = alias;
			});

			this.observe(context?.variantId, (variantId) => {
				this.variantId = variantId?.toString() || "invariant";
			});
		});

	}

	set config(config) {

		const oldConfig = this.#config;
		this.#config = config;

		if (!config) {
			this.requestUpdate("config", oldConfig);
			return;
		}

		this.hideAnchor = Boolean(
			config.getValueByAlias("hideAnchor"),
		);

		this.documentLinksConfig = {
			allowCultureSpecificLinks: Boolean(
				config.getValueByAlias("allowCultureSpecificDocumentLinks"),
			),
		};

		this.min = this.#parseInt(
			config.getValueByAlias("minNumber"),
			0,
		);

		this.max = this.#parseInt(
			config.getValueByAlias("maxNumber"),
			Infinity,
		);

		this.overlaySize = config.getValueByAlias("overlaySize") ?? "small";

		this.requestUpdate("config", oldConfig);

	}

	get config() {
		return this.#config;
	}

	#parseInt(value, fallback) {
		const num = Number(value);
		return !Number.isNaN(num) && num > 0 ? num : fallback;
	}

	firstUpdated() {
		const input = this.shadowRoot?.querySelector("umb-input-multi-url");
		if (input) this.addFormControlElement(input);
	}

	#onChange(event) {
		this.value = event.target.urls;
		this.dispatchEvent(new UmbChangeEvent());
	}

	render() {
		return html`
			<umb-input-multi-url
				.alias=${ this.alias }
				.max = ${ this.max }
				.min=${ this.min }
				.overlaySize=${ this.overlaySize }
				.urls=${ this.value ?? [] }
				.variantId=${ this.variantId }
				.documentLinksConfig=${ this.documentLinksConfig }
				?hide-anchor=${ this.hideAnchor }
				?readonly=${ this.readonly }
				?required=${ this.mandatory }
				.requiredMessage=${ this.mandatoryMessage }
				@change=${ this.#onChange }>
			</umb-input-multi-url>
	`;
	}
}

customElements.define("limbo-url-picker-element", LimboUrlPickerElement);

export default LimboUrlPickerElement;