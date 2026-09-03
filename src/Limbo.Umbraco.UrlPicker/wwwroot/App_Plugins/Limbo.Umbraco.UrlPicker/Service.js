
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { tryExecute } from '@umbraco-cms/backoffice/resources';


import { LIMBO_URL_PICKER_API_PATH } from "@limbo/urlpicker/constants";

export class UrlPickerService {

	static async getConverters() {
		return umbHttpClient.get({
			url: `${LIMBO_URL_PICKER_API_PATH}/converters`,
			security: [{ type: "http", scheme: "bearer" }],
		});
	}

}