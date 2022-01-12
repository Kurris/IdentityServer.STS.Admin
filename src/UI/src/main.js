import Vue from 'vue'
import App from './App.vue'
import router from './router/index'

import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'

// import NProgress from 'nprogress' // 引入nprogress插件
import 'nprogress/nprogress.css' // 这个nprogress样式必须引入

import url from './utils/url.js'

Vue.prototype.$url = url

Vue.use(ElementUI)
// Vue.use(NProgress)

Vue.config.productionTip = false

new Vue({
	router,
	render: h => h(App),
}).$mount('#app')
