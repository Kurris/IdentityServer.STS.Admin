const user = () => import('../views/admin/User.vue')
const admin = () => import('../views/admin/Admin.vue')
export default [
	{
		path: '/admin',
		component: admin,
		children: [
			{
				path: 'user',
				component: user,
			},
		],
	},
]
