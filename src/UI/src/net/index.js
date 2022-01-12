import axios from 'axios'
import ElementUI from 'element-ui'
import NProgress from 'nprogress'

export default function axiosRequest(config) {
	const instance = axios.create()

	instance.interceptors.request.use(config => {
		NProgress.start()
		return config
	})

	instance.interceptors.response.use(
		result => {
			NProgress.done()
			return result.data
		},
		error => {
			NProgress.done()
			if (error.response.status == 500) {
				ElementUI.Notification.error(error.response.data)
			}
			return error
		}
	)

	return instance(config)
}
