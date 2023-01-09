import http from './http'

export const newCode = () => {
	return http({
		baseURL: 'http://localhost:5001',
		url: '/api/qrCode/new',
		method: 'get',
		loading: false,
	})
}

export const getScanResult = data => {
	return http({
		baseURL: 'http://localhost:5001',
		url: '/api/qrCode/scanResult',
		method: 'get',
		params: data,
		loading: false,
	})
}
