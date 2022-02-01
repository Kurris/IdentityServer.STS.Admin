const admin = () => import('../views/admin/Admin.vue')

const user = () => import('../views/admin//User/User.vue')
const identityResource = () => import('../views/admin/IdentityResource/IdentifyResource.vue')
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
		],
	},
]
