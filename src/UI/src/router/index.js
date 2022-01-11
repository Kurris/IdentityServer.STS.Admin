import vue from 'vue'
import vueRouter from 'vue-router'

const signIn = () => import('../components/SignIn.vue')
const home = () => import('../components/Home.vue')
const discoveryDocument = () => import('../components/DiscoveryDocument.vue')
const logout = () => import('../components/Logout.vue')
const loggedOut = () => import('../components/LoggedOut.vue')
const externalLoginConfirmation = () => import('../components/ExternalLoginConfirmation.vue')

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
]

const router = new vueRouter({
	routes,
	mode: 'history',
})
export default router
