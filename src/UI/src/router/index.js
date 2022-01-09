import vue from 'vue'
import vueRouter from 'vue-router'

const signIn = () => import('../components/SignIn.vue')
const home = () => import('../components/Home.vue')
const discoveryDocument = () => import('../components/DiscoveryDocument.vue')

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
]

const router = new vueRouter({
	routes,
	mode: 'history',
})
export default router
