export default function UseCustomDirective(Vue) {
	// v-focus
	Vue.directive('focus', {
		inserted(el) {
			// 聚焦元素
			el.querySelector('input').focus()
		},
	})
}
