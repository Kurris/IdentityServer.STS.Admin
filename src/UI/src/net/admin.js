import http from './http'

export const getUserPage = data => {
	return http({
		url: 'api/user/page',
		method: 'get',
		params: data,
	})
}

export const getUserById = data => {
	return http({
		url: 'api/user',
		method: 'get',
		params: data,
	})
}

export const getUserRoles = data => {
	return http({
		url: 'api/user/roles',
		method: 'get',
		params: data,
	})
}

export const getRoles = () => {
	return http({
		url: 'api/role/roles',
		method: 'get',
	})
}
export const getUserProviderPage = data => {
	return http({
		url: 'api/user/externalProvider/page',
		method: 'get',
		params: data,
	})
}

export const getUserClaimsPage = data => {
	return http({
		url: 'api/user/claims/page',
		method: 'get',
		params: data,
	})
}

export const getStandardClaims = () => {
	return http({
		url: 'api/configuration/claims',
		method: 'get',
	})
}

export const getIdentityResourcePage = data => {
	return http({
		url: 'api/configuration/identityResource/page',
		method: 'get',
		params: data,
	})
}

export const getIdentityResourceById = data => {
	return http({
		url: 'api/configuration/identityResource',
		method: 'get',
		params: data,
	})
}

export const saveIdentityResource = data => {
	return http({
		url: 'api/configuration/identityResource',
		method: 'post',
		data: data,
	})
}
