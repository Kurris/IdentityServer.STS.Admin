const admin = () => import('../views/admin/Admin.vue')

const user = () => import('../views/admin//User/User.vue')
const identityResource = () => import('../views/admin/IdentityResource/IdentifyResource.vue')
const apiResource = () => import('../views/admin/ApiResource/ApiResource.vue')
const role = () => import('../views/admin/Role/Role.vue')
const apiScope = () => import('../views/admin/ApiScope/ApiScope.vue')
const client = () => import('../views/admin/Client/Client.vue')

export default [
	{
		path: '/admin',
		component: admin,
		children: [
			{
				path: 'user',
				component: user,
			},
			{
				path: 'identityResource',
				component: identityResource,
			},
			{
				path: 'apiResource',
				component: apiResource,
			},
			{
				path: 'role',
				component: role,
			},
			{
				path: 'apiScope',
				component: apiScope,
			},
			{
				path: 'client',
				component: client,
			},
		],
	},
]
