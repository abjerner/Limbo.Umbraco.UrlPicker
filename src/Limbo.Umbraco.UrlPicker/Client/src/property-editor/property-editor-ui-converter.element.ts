import { customElement, css, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UMB_ITEM_PICKER_MODAL, umbOpenModal } from '@umbraco-cms/backoffice/modal';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import { LimboUrlPickerConverterRepository } from '../repository/converter.repository.js';
import { LIMBO_URL_PICKER_DOCS_URL } from '../constants.js';
import type { UrlPickerConverterModel, UrlPickerConverterValue } from '../types.js';

/**
 * Property editor UI for the "Converter" option on a Limbo URL Picker data type.
 *
 * Replaces the AngularJS `Converter.html` / `Converter.js` pair from the Umbraco 13 version of the package.
 *
 * @element limbo-property-editor-ui-url-picker-converter
 */
@customElement('limbo-property-editor-ui-url-picker-converter')
export class LimboPropertyEditorUIUrlPickerConverterElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#repository = new LimboUrlPickerConverterRepository(this);

	#value?: UrlPickerConverterValue;

	/**
	 * The stored configuration value.
	 *
	 * Older versions of the package persisted this as a bare string or as an object with a `key` property, and the
	 * type alias sometimes carried a `, Version=…` suffix. All of those are normalised to `{ type }` on read, which
	 * matches what `UrlPickerConverterJsonConverter` writes on the server. Keep the two in sync.
	 */
	@property({ attribute: false })
	public set value(value: UrlPickerConverterValue | string | undefined) {
		this.#value = LimboPropertyEditorUIUrlPickerConverterElement.normalize(value);
		this.#resolveSelected();
	}
	public get value(): UrlPickerConverterValue | undefined {
		return this.#value;
	}

	@state()
	private _loading = true;

	@state()
	private _converters?: Array<UrlPickerConverterModel>;

	@state()
	private _selected?: UrlPickerConverterModel;

	@state()
	private _notFound = false;

	override async connectedCallback() {
		super.connectedCallback();
		if (this._converters) return;

		// tryExecute inside the repository already surfaces failures as backoffice notifications
		const { data } = await this.#repository.requestConverters();

		this._converters = data ?? [];
		this._loading = false;
		this.#resolveSelected();
	}

	/**
	 * Normalises every value shape the package has persisted over time into `{ type }`.
	 */
	static normalize(value: UrlPickerConverterValue | string | undefined | null): UrlPickerConverterValue | undefined {
		let type: string | undefined;

		if (typeof value === 'string') {
			type = value;
		} else if (value && typeof value === 'object') {
			// `key` was the property name used by some earlier versions of the package
			type = value.type ?? (value as { key?: string }).key;
		}

		if (!type) return undefined;

		// Strip any assembly version information, so only "Namespace.Type, Assembly" is left
		type = type.split(', Version=')[0].trim();

		return type ? { type } : undefined;
	}

	#resolveSelected() {
		if (!this._converters) return;

		if (!this.#value) {
			this._selected = undefined;
			this._notFound = false;
			return;
		}

		this._selected = this._converters.find((x) => x.type === this.#value?.type);
		this._notFound = !this._selected;
	}

	async #onAdd() {
		const converters = this._converters ?? [];
		if (!converters.length) return;

		const picked = await umbOpenModal(this, UMB_ITEM_PICKER_MODAL, {
			data: {
				headline: 'Select converter',
				items: converters.map((converter) => ({
					label: converter.name,
					description: converter.description ?? undefined,
					icon: converter.icon,
					value: converter.type,
				})),
			},
		}).catch(() => undefined);

		if (!picked?.value) return;

		this.#value = { type: picked.value };
		this.#resolveSelected();
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onRemove() {
		this.#value = undefined;
		this.#resolveSelected();
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`
			<a href=${LIMBO_URL_PICKER_DOCS_URL} target="_blank" rel="noopener noreferrer">
				<uui-button look="secondary" compact label="See the documentation">
					See the documentation &rarr;
				</uui-button>
			</a>
			${this.#renderValue()}
		`;
	}

	#renderValue() {
		if (this._loading) {
			return html`<uui-loader></uui-loader>`;
		}

		if (!this.#value) {
			if (!this._converters?.length) {
				return html`<div class="message">No converters are registered on the server.</div>`;
			}

			return html`
				<uui-button look="placeholder" label=${this.localize.term('general_add')} @click=${this.#onAdd}></uui-button>
			`;
		}

		return html`
			${this._notFound
				? html`<div class="message error">
						The selected type <strong>${this.#value.type}</strong> could not be found.
					</div>`
				: nothing}
			<uui-ref-node
				standalone
				name=${this._selected?.name ?? this.#value.type}
				detail=${this._selected?.description ?? ''}
				@open=${this.#onAdd}>
				${this._selected ? html`<umb-icon slot="icon" name=${this._selected.icon}></umb-icon>` : nothing}
				<uui-action-bar slot="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						color="danger"
						@click=${this.#onRemove}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}

	static override styles = [
		css`
			:host {
				display: block;
			}

			uui-button[look='placeholder'] {
				width: 100%;
			}

			.message {
				margin: var(--uui-size-space-3) 0;
			}

			.message.error {
				color: var(--uui-color-danger);
			}
		`,
	];
}

export default LimboPropertyEditorUIUrlPickerConverterElement;

declare global {
	interface HTMLElementTagNameMap {
		'limbo-property-editor-ui-url-picker-converter': LimboPropertyEditorUIUrlPickerConverterElement;
	}
}
