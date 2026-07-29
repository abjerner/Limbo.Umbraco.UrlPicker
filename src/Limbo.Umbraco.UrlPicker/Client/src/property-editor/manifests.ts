import {
	LIMBO_URL_PICKER_CONVERTER_UI_ALIAS,
	LIMBO_URL_PICKER_SCHEMA_ALIAS,
	LIMBO_URL_PICKER_UI_ALIAS,
} from '../constants.js';
import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/property-editor';

/**
 * The property editor schema. The server side counterpart is `UrlPickerEditor`, and the settings declared here are
 * what used to be `[ConfigurationField]` metadata on `UrlPickerConfiguration` before Umbraco 14.
 */
const schemaManifest: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: 'Limbo URL Picker',
	alias: LIMBO_URL_PICKER_SCHEMA_ALIAS,
	meta: {
		defaultPropertyEditorUiAlias: LIMBO_URL_PICKER_UI_ALIAS,
		settings: {
			properties: [
				{
					alias: 'minNumber',
					label: 'Minimum number of items',
					description: '',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Integer',
					config: [{ alias: 'min', value: 0 }],
				},
				{
					alias: 'maxNumber',
					label: 'Maximum number of items',
					description: '',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Integer',
					config: [{ alias: 'min', value: 0 }],
				},
				{
					alias: 'ignoreUserStartNodes',
					label: 'Ignore user start nodes',
					description: 'Selecting this option allows a user to choose nodes that they normally dont have access to.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'converter',
					label: 'Converter',
					description:
						'Select a converter to control the type of the items returned by properties of this data type.',
					propertyEditorUiAlias: LIMBO_URL_PICKER_CONVERTER_UI_ALIAS,
				},
			],
			defaultData: [
				{ alias: 'minNumber', value: 0 },
				{ alias: 'maxNumber', value: 0 },
			],
		},
	},
};

/**
 * The property editor UI shown when editing content.
 */
const uiManifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: LIMBO_URL_PICKER_UI_ALIAS,
	name: 'Limbo URL Picker Property Editor UI',
	element: () => import('./property-editor-ui-url-picker.element.js'),
	meta: {
		label: 'Limbo URL Picker',
		propertyEditorSchemaAlias: LIMBO_URL_PICKER_SCHEMA_ALIAS,
		icon: 'icon-link',
		group: 'pickers',
		supportsReadOnly: true,
		settings: {
			properties: [
				{
					alias: 'overlaySize',
					label: 'Overlay Size',
					description: 'Select the width of the overlay.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.OverlaySize',
				},
				{
					alias: 'hideAnchor',
					label: 'Hide anchor/query string input',
					description: 'Selecting this hides the anchor/query string input field in the link picker overlay.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'allowCultureSpecificDocumentLinks',
					label: '#linkPicker_configCultureSpecificDocumentLinksLabel',
					description: '{#linkPicker_configCultureSpecificDocumentLinksDescription}',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
		},
	},
};

/**
 * The property editor UI backing the "Converter" data type option. It deliberately has no
 * `propertyEditorSchemaAlias`, so it is only usable as a configuration field and never offered as a property editor
 * in its own right.
 */
const converterUiManifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: LIMBO_URL_PICKER_CONVERTER_UI_ALIAS,
	name: 'Limbo URL Picker Converter Property Editor UI',
	element: () => import('./property-editor-ui-converter.element.js'),
	meta: {
		label: 'Limbo URL Picker Converter',
		icon: 'icon-brackets',
		group: '',
	},
};

export const manifests = [schemaManifest, uiManifest, converterUiManifest];
