<template>
	<div id="error">
		<h3>错误消息:</h3>
		<span>
			{{ error || remoteError }}
		</span>
		<template v-if="$route.query.errorId"> ErrorId: {{ $route.query.errorId }} </template>

		<div v-if="!isLocal || isLocal == 'False'">
			<el-link type="primary" @click="reLogin()">重新登录</el-link>
		</div>
	</div>
</template>

<script>
import { getError } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			error: null,
			remoteError: this.$route.query.remoteError,
			isLocal: this.$route.query.isLocal,
			returnUrl: this.$route.query.returnUrl,
		}
	},
	methods: {
		reLogin() {
			this.$router.push({
				path: '/signIn',
				params: {
					returnUrl: this.returnUrl,
				},
			})
		},
	},
	async beforeMount() {
		if (!this.error && !this.remoteError) {
			this.error = await getError({ errorId: this.$route.query.errorId })
		}
	},
}
</script>
<style scoped>
#error {
	margin-top: 30px;
	text-align: center;
}
</style>
