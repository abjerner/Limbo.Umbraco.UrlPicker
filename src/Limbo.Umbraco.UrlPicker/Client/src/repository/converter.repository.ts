import { LimboUrlPickerConverterServerDataSource } from './converter.server.data-source.js';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

/**
 * Repository for the item converters registered on the server.
 */
export class LimboUrlPickerConverterRepository extends UmbControllerBase {
	#dataSource: LimboUrlPickerConverterServerDataSource;

	constructor(host: UmbControllerHost) {
		super(host);
		this.#dataSource = new LimboUrlPickerConverterServerDataSource(this);
	}

	/**
	 * Requests the available item converters. Errors are surfaced as backoffice notifications by `tryExecute`.
	 */
	async requestConverters() {
		return this.#dataSource.getConverters();
	}
}
