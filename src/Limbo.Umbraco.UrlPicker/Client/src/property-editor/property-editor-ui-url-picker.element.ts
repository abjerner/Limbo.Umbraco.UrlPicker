import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import { UMB_VALIDATION_EMPTY_LOCALIZATION_KEY, UmbFormControlMixin } from '@umbraco-cms/backoffice/validation';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';
import type { UmbInputMultiUrlElement, UmbLinkPickerLink } from '@umbraco-cms/backoffice/multi-url-picker';

// Registers the <umb-input-multi-url> custom element that this editor is built on.
import '@umbraco-cms/backoffice/multi-url-picker';

/**
 * Property editor UI for the Limbo URL Picker.
 *
 * The picker itself is identical to Umbraco's own Multi URL Picker — the package only differs in the extra
 * "Converter" option on the data type, which is applied server side. This element therefore wraps the same
 * `<umb-input-multi-url>` component that the built-in editor uses.
 *
 * @element limbo-property-editor-ui-url-picker
 */
@customElement('limbo-property-editor-ui-url-picker')
export class LimboPropertyEditorUIUrlPickerElement
	extends UmbFormControlMixin<Array<UmbLinkPickerLink>, typeof UmbLitElement, undefined>(UmbLitElement)
	implements UmbPropertyEditorUiElement
{
	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this._hideAnchor = Boolean(config.getValueByAlias('hideAnchor'));
		this._documentLinksConfig = {
			allowCultureSpecificLinks: Boolean(config.getValueByAlias('allowCultureSpecificDocumentLinks')),
		};
		this._min = this.#parseInt(config.getValueByAlias('minNumber'), 0);
		this._max = this.#parseInt(config.getValueByAlias('maxNumber'), Infinity);
		this._overlaySize = config.getValueByAlias<UUIModalSidebarSize>('overlaySize') ?? 'small';
	}

	@property({ type: Boolean, reflect: true })
	readonly = false;

	@property({ type: Boolean })
	mandatory = false;

	@property({ type: String })
	mandatoryMessage = UMB_VALIDATION_EMPTY_LOCALIZATION_KEY;

	@state()
	private _overlaySize?: UUIModalSidebarSize;

	@state()
	private _hideAnchor?: boolean;

	@state()
	private _documentLinksConfig?: { allowCultureSpecificLinks: boolean };

	@state()
	private _min = 0;

	@state()
	private _max = Infinity;

	@state()
	private _alias?: string;

	@state()
	private _variantId?: string;

	constructor() {
		super();

		this.consumeContext(UMB_PROPERTY_CONTEXT, (context) => {
			this.observe(context?.alias, (alias) => (this._alias = alias));
			this.observe(context?.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
		});
	}

	#parseInt(value: unknown, fallback: number): number {
		const num = Number(value);
		return !Number.isNaN(num) && num > 0 ? num : fallback;
	}

	protected override firstUpdated() {
		this.addFormControlElement(this.shadowRoot!.querySelector('umb-input-multi-url')!);
	}

	#onChange(event: CustomEvent & { target: UmbInputMultiUrlElement }) {
		this.value = event.target.urls;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`
			<umb-input-multi-url
				.alias=${this._alias}
				.max=${this._max}
				.min=${this._min}
				.overlaySize=${this._overlaySize}
				.urls=${this.value ?? []}
				.variantId=${this._variantId}
				.documentLinksConfig=${this._documentLinksConfig}
				?hide-anchor=${this._hideAnchor}
				?readonly=${this.readonly}
				?required=${this.mandatory}
				.requiredMessage=${this.mandatoryMessage}
				@change=${this.#onChange}>
			</umb-input-multi-url>
		`;
	}
}

export default LimboPropertyEditorUIUrlPickerElement;

declare global {
	interface HTMLElementTagNameMap {
		'limbo-property-editor-ui-url-picker': LimboPropertyEditorUIUrlPickerElement;
	}
}
