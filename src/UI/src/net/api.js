import http from './http'

//用户登录
export const signIn = data => {
	return http({
		url: 'api/authenticate/login',
		data: data,
		method: 'post',
	})
}

//是否已经登录
export const isAuthenticated = () => {
	return http({
		url: 'api/authenticate/status',
		method: 'get',
	})
}

//获取发现文档
export const getDocument = () => {
	return http({
		url: '.well-known/openid-configuration',
		method: 'get',
	})
}

export const getLogin = data => {
	return http({
		url: 'api/authenticate/checkLogin',
		method: 'get',
		params: data,
	})
}
