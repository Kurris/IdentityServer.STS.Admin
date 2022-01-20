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
			} else if (result.data.code == 500) {
				ElementUI.Notification.error(result.data.msg)
			} else {
				return result.data
			}
		},
		error => {
			NProgress.done()
			if (error.message == 'Network Error') {
				ElementUI.Notification.error('网络请求错误')
			} else if (error.response.status != 200) {
				if (error.response.status == 404) {
					ElementUI.Notification.error('找不到请求的接口')
				} else {
					ElementUI.Notification.error(error.response.data)
				}
			} else {
				return error
			}
		}
	)

	return instance(config)
}
