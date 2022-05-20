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
			if (result.headers['content-type'] && result.headers['content-type'].indexOf('application/json') < 0) {
				var a = document.createElement('a')
				document.body.append(a)

				let tempName = result.headers['content-disposition'].split(';')[1].split('filename=')[1]
				a.download = encodeURI(tempName)

				var blob = new Blob([result.data], { type: result.headers['content-type'] })
				a.href = URL.createObjectURL(blob)
				a.click()
				window.URL.revokeObjectURL(blob)
				document.body.removeChild(a)
			} else {
				if (result.data.code == 302) {
					//避免死循环
					if (window.location.href.indexOf('/signIn') < 0) {
						window.location.href = result.data.data + '?returnUrl=' + window.location
					}
				} else if (result.data.code == 500) {
					ElementUI.Notification.error(result.data.msg)
				} else if (result.data.code == 400) {
					let errors = result.data.data.map(x => `${x.message}`)
					ElementUI.Notification.error(errors.join(','))
				} else {
					return result.data
				}
			}
			return null
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
			return error
		}
	)

	return instance(config)
}
