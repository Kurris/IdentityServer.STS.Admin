import vue from 'vue'
import vueRouter from 'vue-router'

const signIn = () => import('../components/SignIn.vue')

vue.use(vueRouter)

const routes = [
	{
		path: '/',
		redirect: '/signIn',
	},
	{
		path: '/signIn',
		component: signIn,
	},
]

const router = new vueRouter({
	routes,
	mode: 'history',
})
export default router
