import http from './http'

export const newCode = () => {
	return http({
		url: 'api/qrCode/new',
		method: 'get',
	})
}

export const getScanResult = data => {
	return http({
		url: 'api/qrCode/scanResult',
		method: 'get',
		params: data,
	})
}
