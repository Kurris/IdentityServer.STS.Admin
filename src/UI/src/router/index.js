import vue from 'vue'
import vueRouter from 'vue-router'

import NProgress from 'nprogress'

const signIn = () => import('../components/SignIn.vue')
const home = () => import('../components/Home.vue')
const discoveryDocument = () => import('../components/DiscoveryDocument.vue')
const logout = () => import('../components/Logout.vue')
const loggedOut = () => import('../components/LoggedOut.vue')
const externalLoginConfirmation = () => import('../components/ExternalLoginConfirmation.vue')
const consent = () => import('../components/Consent.vue')
const error = () => import('../components/Error.vue')
const signinWithRecoveryCode = () => import('../components/SigninWithRecoveryCode.vue')
const signinWith2fa = () => import('../components/SigninWith2fa.vue')
const enableAuthenticator = () => import('../components/EnableAuthenticator.vue')
const twoFactorAuthentication = () => import('../components/TwoFactorAuthentication.vue')
const showRecoveryCodes = () => import('../components/ShowRecoveryCodes.vue')
const loginWithRecoveryCode = () => import('../components/LoginWithRecoveryCode.vue')
const grants = () => import('../components/Grants.vue')
const password = () => import('../components/Password.vue')
const profile = () => import('../components/Profile.vue')
const personalData = () => import('../components/PersonalData.vue')
const externalLogins = () => import('../components/ExternalLogins.vue')
const forgotPassword= () => import('../components/ForgotPassword.vue')

vue.use(vueRouter)

const routes = [
	{
		path: '/',
		redirect: '/home',
	},
	{
		name: 'signin',
		path: '/signIn',
		component: signIn,
	},
	{
		path: '/home',
		component: home,
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
		path: '/twoFactorAuthentication',
		component: twoFactorAuthentication,
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
		path: '/profile',
		component: profile,
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
