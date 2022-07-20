<template>
	<div id="error">
		<el-result icon="error" title="错误提示" :subTitle="subTitle">
			<template slot="extra">
				<!-- <template v-if="$route.query.errorId"> ErrorId: {{ $route.query.errorId }} </template> -->
				<a :href="returnUrl">
					<el-button type="primary" size="medium">返回</el-button>
				</a>
			</template>
		</el-result>
	</div>
</template>

<script>
import { getError } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			error: this.$route.query.error,
			remoteError: this.$route.query.remoteError,
			returnUrl: this.$route.query.returnUrl || '/signIn',
		}
	},
	computed: {
		subTitle() {
			return this.error || this.remoteError
		},
	},
	async beforeMount() {
		if (!this.error && !this.remoteError) {
			let errorResult = await getError({ errorId: this.$route.query.errorId })
			this.error = errorResult.data
			this.returnUrl = this.error.redirectUri || '/signIn'
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
