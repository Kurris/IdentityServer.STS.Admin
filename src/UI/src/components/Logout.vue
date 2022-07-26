<template>
	<div id="logout">
		<template v-if="$route.query.logoutId == null">
			<h1>是否要注销登录?</h1>
			<div>
				<el-button type="warning" @click="yes">注销</el-button>
				<el-button @click="$router.back()">取消</el-button>
			</div>
		</template>
	</div>
</template>

<script>
import { loggedOut } from '../net/api.js'

export default {
	methods: {
		async yes() {
			let logoutId = this.$route.query.logoutId

			let res = await loggedOut({ logoutId })
			if (res.route == 10) {
				this.$router.push({
					path: '/loggedOut',
					query: {
						postLogoutRedirectUri: res.data.postLogoutRedirectUri,
						signOutIframeUrl: res.data.signOutIframeUrl,
						automaticRedirectAfterSignOut: res.data.automaticRedirectAfterSignOut,
						clientName: res.data.clientName,
					},
				})
			}
		},
	},
	async beforeMount() {
		if (this.$route.query.logoutId) await this.yes()
	},
}
</script>
<style scoped>
#logout {
	display: flex;
	flex-direction: column;
	justify-content: center;
	align-items: center;
}
</style>
