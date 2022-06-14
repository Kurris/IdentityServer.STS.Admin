import axiosRequest from './index'

export default async function http(config) {
	config.timeout = 60 * 1000 * 2 //30sec
	config.withCredentials = true
	config.baseURL = 'http://101.35.47.169:5000'

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
