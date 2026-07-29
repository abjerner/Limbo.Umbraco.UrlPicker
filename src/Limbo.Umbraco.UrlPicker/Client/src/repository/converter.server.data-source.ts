import { LIMBO_URL_PICKER_API_PATH } from '../constants.js';
import type { UrlPickerConverterModel } from '../types.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { tryExecute } from '@umbraco-cms/backoffice/resources';

/**
 * Server data source for the item converters registered on the server.
 *
 * `umbHttpClient` is the backoffice's shared, pre-authenticated HTTP client — a raw `fetch()` would be rejected with
 * a 401. The `security` metadata is what makes the client attach the backoffice credentials to the request.
 */
export class LimboUrlPickerConverterServerDataSource {
	#host: UmbControllerHost;

	constructor(host: UmbControllerHost) {
		this.#host = host;
	}

	/**
	 * Fetches the available item converters.
	 */
	async getConverters() {
		const { data, error } = await tryExecute(
			this.#host,
			umbHttpClient.get({
				url: `${LIMBO_URL_PICKER_API_PATH}/converter`,
				security: [{ type: 'http', scheme: 'bearer' }],
			}),
		);

		return {
			data: data as Array<UrlPickerConverterModel> | undefined,
			error,
		};
	}
}
