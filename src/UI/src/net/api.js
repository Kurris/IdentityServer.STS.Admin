import http from './http'

//用户登录
export const signIn = data => {
	return http({
		url: 'api/account/login',
		data: data,
		method: 'post',
	})
}

export const getUserByName = data => {
	return http({
		url: 'api/account/user',
		method: 'get',
		params: data,
	})
}

//
export const siginTwoFactorAuthenticationUser = data => {
	return http({
		url: 'api/account/twoFactorAuthenticationUser/signIn',
		data: data,
		method: 'post',
	})
}

export const checkTwoFactorAuthenticationUser = () => {
	return http({
		url: 'api/account/twoFactorAuthenticationUser',
		method: 'get',
	})
}

export const externalLoginWithLocalLogin = data => {
	return http({
		url: 'api/account/externalLoginWithLocalLogin',
		method: 'post',
		data: data,
	})
}

//是否已经登录
export const getLoginStatus = () => {
	return http({
		url: 'api/account/status',
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
		url: 'api/account/2fa/signInWithCode',
		method: 'get',
		params: data,
	})
}

export const signInWithCode = data => {
	return http({
		url: 'api/account/2fa/signInWithCode',
		method: 'post',
		data: data,
	})
}

//检查登录
export const checkLogin = data => {
	return http({
		url: 'api/account/loginUiSetting',
		method: 'get',
		params: data,
	})
}

export const logout = data => {
	return http({
		url: 'api/account/logout',
		method: 'get',
		params: data,
	})
}

export const loggedOut = data => {
	return http({
		url: 'api/account/loggedOut',
		method: 'post',
		data: data,
	})
}

export const externalLogin = data => {
	return http({
		url: 'api/account/externalLogin',
		method: 'get',
		params: data,
	})
}

export const externalRegister = data => {
	return http({
		url: 'api/account/externalRegister',
		method: 'post',
		data: data,
	})
}
export const checkExternalRegister = () => {
	return http({
		url: 'api/account/externalRegister',
		method: 'get',
	})
}

export const register = data => {
	return http({
		url: 'api/account/accout/register',
		method: 'post',
		data: data,
	})
}

export const getError = data => {
	return http({
		url: 'api/account/error',
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
//
export const resetAuthenticator = () => {
	return http({
		url: 'api/manager/setting/2fa/authenticator/reset',
		method: 'put',
	})
}
export const enable2Fa = data => {
	return http({
		url: `api/manager/setting/2fa/${data.enable}`,
		method: 'put',
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

export const getGrants = data => {
	return http({
		url: 'api/grants/grants',
		method: 'get',
		params: data,
	})
}
//

export const deleteGrant = data => {
	return http({
		url: `api/grants/client/${data.clientId}`,
		method: 'delete',
	})
}

//
export const checkPassword = () => {
	return http({
		url: 'api/manager/password/status',
		method: 'get',
	})
}

export const savePassword = data => {
	return http({
		url: 'api/manager/password',
		method: 'post',
		data: data,
	})
}

export const getProfile = () => {
	return http({
		url: 'api/manager/profile',
		method: 'get',
	})
}

export const saveProfile = data => {
	return http({
		url: 'api/manager/profile',
		method: 'post',
		data: data,
	})
}

export const downloadProfile = () => {
	return http({
		url: 'api/manager/profile/download',
		method: 'get',
		responseType: 'arraybuffer',
	})
}

export const deleteProfile = data => {
	return http({
		url: 'api/manager/profile',
		method: 'delete',
		data: data,
	})
}

export const getExternalLogins = () => {
	return http({
		url: 'api/manager/externalLogins',
		method: 'get',
	})
}

export const deleteExternalLogin = data => {
	return http({
		url: 'api/manager/externalLogin',
		method: 'delete',
		data: data,
	})
}

export const forgetPasswordAndSendEmail = data => {
	return http({
		url: 'api/account/password/email',
		method: 'post',
		data: data,
	})
}

export const resetPassword = data => {
	return http({
		url: 'api/account/password/email/found',
		method: 'put',
		data: data,
	})
}

export const getUserGetUserCodeConfirmationModel = data => {
	return http({
		url: 'api/device/confirmation',
		method: 'get',
		params: data,
	})
}

export const createPAT = data => {
	return http({
		url: 'api/pat',
		method: 'post',
		data: data,
	})
}

export const getAllPats = data => {
	return http({
		url: 'api/pat/all',
		method: 'get',
		params: data,
	})
}

export const removePat = data => {
	return http({
		url: 'api/pat',
		method: 'delete',
		data: data,
	})
}
