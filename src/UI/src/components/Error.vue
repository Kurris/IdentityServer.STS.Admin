<template>
	<div id="error">
		<span> </span>
		<template v-if="$route.query.errorId"> ErrorId: {{ $route.query.errorId }} </template>

		<el-result icon="error" title="错误提示" :subTitle="subTitle">
			<template slot="extra">
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
			returnUrl: this.$route.query.returnUrl,
		}
	},
	computed: {
		subTitle() {
			return this.error || this.remoteError
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
