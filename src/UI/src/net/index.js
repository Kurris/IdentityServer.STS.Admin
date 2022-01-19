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
			if (result.data.code == 302) {
				NProgress.start()
				window.location.href = result.data.data + '?ReturnUrl=' + window.location
			} else {
				return result.data
			}
		},
		error => {
			NProgress.done()
			if (error.message == 'Network Error') {
				ElementUI.Notification.error('网络请求错误')
			} else if (error.response.status != 200) {
				ElementUI.Notification.error(error.response.data)
			} else {
				return error
			}
		}
	)

	return instance(config)
}
