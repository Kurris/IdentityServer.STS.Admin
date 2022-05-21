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
const signinWithRecoveryCode = () => import('../components/SigninWithRecoveryCode.vue')
const signinWith2fa = () => import('../components/SigninWith2fa.vue')
const enableAuthenticator = () => import('../components/EnableAuthenticator.vue')
const twoFactorAuthentication = () => import('../components/setting/TwoFactorAuthentication.vue')
const showRecoveryCodes = () => import('../components/ShowRecoveryCodes.vue')
const loginWithRecoveryCode = () => import('../components/LoginWithRecoveryCode.vue')
const grants = () => import('../components/Grants.vue')
const password = () => import('../components/Password.vue')
const profile = () => import('../components/setting/Profile.vue')
const personalData = () => import('../components/PersonalData.vue')
const externalLogins = () => import('../components/ExternalLogins.vue')
const forgotPassword = () => import('../components/ForgotPassword.vue')
const resetPassword = () => import('../components/ResetPassword.vue')
const register = () => import('../components/Register.vue')
const device = () => import('../components/Device.vue')
const userCodeCapture = () => import('../components/UserCodeCapture.vue')
const userCodeConfirmation = () => import('../components/UserCodeConfirmation.vue')
const successed = () => import('../components/Successed.vue')
const setting = () => import('../components/Setting.vue')

vue.use(vueRouter)

const routes = [
	// {
	// 	path: '/',
	// 	redirect: '/zone',
	// },
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
				path: 'twoFactorAuthentication',
				component: twoFactorAuthentication,
			},
			{
				path: 'profile',
				component: profile,
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
		path: '/signinWithRecoveryCode',
		component: signinWithRecoveryCode,
	},
	{
		path: '/enableAuthenticator',
		component: enableAuthenticator,
	},

	{
		path: '/showRecoveryCodes',
		component: showRecoveryCodes,
	},
	{
		path: '/loginWithRecoveryCode',
		component: loginWithRecoveryCode,
	},
	{
		path: '/grants',
		component: grants,
	},
	{
		path: '/password',
		component: password,
	},

	{
		path: '/personalData',
		component: personalData,
	},
	{
		path: '/externalLogins',
		component: externalLogins,
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
]

const router = new vueRouter({
	routes,
	mode: 'history',
})

router.beforeEach((to, from, next) => {
	NProgress.start()
	next()
})

router.afterEach(() => {
	NProgress.done()
})

export default router
