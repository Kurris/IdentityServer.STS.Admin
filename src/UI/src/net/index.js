import axios from 'axios'
import ElementUI from 'element-ui'

export default function axiosRequest(config) {
	const instance = axios.create()

	instance.interceptors.request.use(config => {
		return config
	})

	instance.interceptors.response.use(
		result => {
			return result.data
		},
		error => {
			if (error.status == 500) {
				ElementUI.Notification.error(error)
			} else {
				return error
			}
		}
	)

	return instance(config)
}
