import axiosRequest from './index'
import { baseURL } from '../utils/apiUrlHelper'

export default async function http(config) {
	config.timeout = 60 * 1000 * 2
	config.withCredentials = true
	if (config.baseURL == undefined) {
		config.baseURL = baseURL
	}

	return new Promise((resolve, reject) =>
		axiosRequest(config)
			.then(res => {
				resolve(res)
			})
			.catch(err => {
				reject(err)
			})
	)
}
