import vue from 'vue'
import vueRouter from 'vue-router'
import NProgress from 'nprogress'

import adminRouters from './adminRouters'

const signIn = () => import('../components/SignIn.vue')
const zone = () => import('../components/Zone.vue')
const discoveryDocument = () => import('../components/DiscoveryDocument.vue')
const logout = () => import('../components/Logout.vue')
const loggedOut = () => import('../components/LoggedOut.vue')
const externalLoginConfirmation = () => import('../components/ExternalLoginConfirmation.vue')
const consent = () => import('../components/Consent.vue')
const error = () => import('../components/Error.vue')
const signinWith2fa = () => import('../components/SigninWith2fa.vue')
const twoFactorAuthentication = () => import('../components/setting/TwoFactorAuthentication.vue')
const loginWithRecoveryCode = () => import('../components/LoginWithRecoveryCode.vue')
const grants = () => import('../components/setting/Grants.vue')
const password = () => import('../components/setting/Password.vue')
const profile = () => import('../components/setting/Profile.vue')
const account = () => import('../components/setting/Account.vue')
const externalLogins = () => import('../components/setting/ExternalLogins.vue')
const forgotPassword = () => import('../components/ForgotPassword.vue')
const resetPassword = () => import('../components/ResetPassword.vue')
const register = () => import('../components/Register.vue')
const device = () => import('../components/Device.vue')
const userCodeCapture = () => import('../components/UserCodeCapture.vue')
const userCodeConfirmation = () => import('../components/UserCodeConfirmation.vue')
const successed = () => import('../components/Successed.vue')
const setting = () => import('../components/Setting.vue')
const development = () => import('../components/setting/Development.vue')
const security = () => import('../components/setting/Security.vue')
const notfound = () => import('../views/404.vue')

vue.use(vueRouter)

const routes = [
	{
		path: '/',
		redirect: '/signIn',
	},
	...adminRouters,
	{
		name: 'signIn',
		path: '/signIn',
		component: signIn,
	},
	{
		path: '/zone/:userName',
		component: zone,
	},
	{
		path: '/setting',
		component: setting,
		children: [
			{
				path: '',
				redirect: '/setting/profile',
			},
			{
				path: 'security',
				component: security,
			},
			{
				path: 'twoFactorAuthentication',
				component: twoFactorAuthentication,
			},
			{
				path: 'externalLogins',
				component: externalLogins,
				meta: {
					title: '外部登录',
				},
			},
			{
				path: 'profile',
				component: profile,
			},
			{
				path: 'development',
				component: development,
				meta: {
					title: '开发者设置',
				},
			},
			{
				path: 'grants',
				component: grants,
				meta: {
					title: '授权应用',
				},
			},
			{
				path: 'account',
				component: account,
			},
		],
	},
	{
		name: 'zone',
		path: '/zone',
		component: zone,
	},
	{
		path: '/discoveryDocument',
		component: discoveryDocument,
	},
	{
		path: '/logout',
		component: logout,
	},
	{
		path: '/loggedOut',
		component: loggedOut,
	},
	{
		path: '/externalLoginConfirmation',
		component: externalLoginConfirmation,
	},
	{
		path: '/error',
		component: error,
	},
	{
		path: '/consent',
		component: consent,
	},
	{
		path: '/signinWith2fa',
		component: signinWith2fa,
	},
	{
		path: '/loginWithRecoveryCode',
		component: loginWithRecoveryCode,
	},
	{
		path: '/password',
		component: password,
	},
	{
		path: '/forgotPassword',
		component: forgotPassword,
	},
	{
		path: '/resetPassword',
		component: resetPassword,
	},
	{
		path: '/register',
		component: register,
	},
	{
		path: '/device',
		component: device,
	},
	{
		path: '/userCodeConfirmation',
		component: userCodeConfirmation,
	},
	{
		path: '/userCodeCapture',
		component: userCodeCapture,
	},
	{
		path: '/successed',
		component: successed,
	},
	{
		path: '*', //会匹配所有路径
		name: '404',
		component: notfound,
	},
]

const router = new vueRouter({
	routes,
	mode: 'history',
	base: '/identity/',
})

router.beforeEach((to, from, next) => {
	NProgress.start()
	next()
})

router.beforeEach((to, from, next) => {
	/* 路由发生变化修改页面title */
	let curr = to?.meta?.title ? ' | ' + to.meta.title : ''
	document.title = '认证中心' + curr
	next()
})

router.afterEach(() => {
	NProgress.done()
})

export default router
