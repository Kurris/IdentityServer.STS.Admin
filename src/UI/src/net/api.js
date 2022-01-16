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

//检查登录
export const checkLogin = data => {
	return http({
		url: 'api/authenticate/checkLogin',
		method: 'get',
		params: data,
	})
}

export const logout = data => {
	return http({
		url: 'api/authenticate/logout',
		method: 'get',
		params: data,
	})
}

export const loggedOut = data => {
	return http({
		url: 'api/authenticate/loggedOut',
		method: 'post',
		data: data,
	})
}

export const externalLogin = data => {
	return http({
		url: 'api/authenticate/externalLogin',
		method: 'get',
		params: data,
	})
}

export const externalRegister = data => {
	return http({
		url: 'api/authenticate/externalRegister',
		method: 'post',
		data: data,
	})
}

export const getError = data => {
	return http({
		url: 'api/authenticate/error',
		method: 'get',
		params: data,
	})
}

export const getConsentSetting = data => {
	return http({
		url: 'api/consent/setting',
		method: 'get',
		params: data,
	})
}

// export const processConsent = data => {
// 	return http({
// 		url: 'api/consent/setting/process',
// 		method: 'post',
// 		data: data,
// 	})
// }
