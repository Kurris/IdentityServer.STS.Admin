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
