import http from './http'

//用户登录
export const signIn = data => {
	return http({
		url: 'api/authenticate/login',
		data: data,
		method: 'post',
	})
}
//
//
export const siginTwoFactorAuthenticationUser = data => {
	return http({
		url: 'api/authenticate/twoFactorAuthenticationUser/signIn',
		data: data,
		method: 'post',
	})
}

export const checkTwoFactorAuthenticationUser = data => {
	return http({
		url: 'api/authenticate/twoFactorAuthenticationUser',
		params: data,
		method: 'get',
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
//
export const goSignInWithCode = data => {
	return http({
		url: 'api/authenticate/2fa/signInWithCode',
		method: 'get',
		params: data,
	})
}

export const signInWithCode = data => {
	return http({
		url: 'api/authenticate/2fa/signInWithCode',
		method: 'post',
		data: data,
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

export const getRecoveryCodes = data => {
	return http({
		url: 'api/manager/setting/2fa/recoveryCodes',
		method: 'get',
		data: data,
	})
}

export const getTwofactorSetting = () => {
	return http({
		url: 'api/manager/setting/2fa',
		method: 'get',
	})
}

export const forget2faClient = () => {
	return http({
		url: 'api/manager/setting/2fa/client',
		method: 'delete',
	})
}

export const diable2fa = () => {
	return http({
		url: 'api/manager/setting/2fa',
		method: 'delete',
	})
}

export const getAuthenticatorSetting = () => {
	return http({
		url: 'api/manager/setting/2fa/authenticator',
		method: 'get',
	})
}

export const verifyAuthode = data => {
	return http({
		url: 'api/manager/setting/2fa/authenticator/verify',
		method: 'post',
		data: data,
	})
}
