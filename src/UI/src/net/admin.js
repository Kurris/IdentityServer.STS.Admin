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

export const getApiResourcePage = data => {
	return http({
		url: 'api/configuration/ApiResource/page',
		method: 'get',
		params: data,
	})
}

export const getApiResourceById = data => {
	return http({
		url: 'api/configuration/apiResource',
		method: 'get',
		params: data,
	})
}

export const saveApiResource = data => {
	return http({
		url: 'api/configuration/apiResource',
		method: 'post',
		data: data,
	})
}

export const getApiScopePage = data => {
	return http({
		url: 'api/configuration/apiScope/page',
		method: 'get',
		params: data,
	})
}

export const getApiScopeById = data => {
	return http({
		url: 'api/configuration/apiScope',
		method: 'get',
		params: data,
	})
}

export const saveApiScope = data => {
	return http({
		url: 'api/configuration/apiScope',
		method: 'post',
		data: data,
	})
}

export const getRolePage = data => {
	return http({
		url: 'api/role/page',
		method: 'get',
		params: data,
	})
}

export const getRoleById = data => {
	return http({
		url: 'api/role',
		method: 'get',
		params: data,
	})
}

export const saveRole = data => {
	return http({
		url: 'api/role',
		method: 'post',
		data: data,
	})
}
//

export const getRoleUserPage = data => {
	return http({
		url: 'api/role/userPage',
		method: 'get',
		params: data,
	})
}

export const getClientPage = data => {
	return http({
		method: 'get',
		url: 'api/configuration/client/page',
		params: data,
	})
}

export const removeClient = data => {
	return http({
		method: 'delete',
		url: `api/configuration/client/${data.id}`,
	})
}

export const getClientById = data => {
	return http({
		method: 'get',
		url: 'api/configuration/client',
		params: data,
	})
}

export const getClientByIdForCopy = data => {
	return http({
		method: 'get',
		url: 'api/configuration/newClient',
		params: data,
	})
}

export const saveClient = data => {
	return http({
		method: 'post',
		url: 'api/configuration/client',
		data: data,
	})
}

export const getGrantTypes = () => {
	return http({
		url: 'api/configuration/grantTypes',
		method: 'get',
	})
}

export const getClientSecrets = data => {
	return http({
		url: 'api/configuration/clientSecrets',
		method: 'get',
		params: data,
	})
}

export const addClientSecret = data => {
	return http({
		url: 'api/configuration/clientSecret',
		method: 'post',
		data: data,
	})
}

export const removeClientSecret = data => {
	return http({
		url: 'api/configuration/clientSecret',
		method: 'delete',
		params: data,
	})
}

export const getScopes = () => {
	return http({
		url: 'api/configuration/scopes',
		method: 'get',
	})
}

export const getEnums = () => {
	return http({
		url: 'api/configuration/enums',
		method: 'get',
	})
}

export const getExternalProviders = () => {
	return http({
		url: 'api/configuration/externalProviders',
		method: 'get',
	})
}
