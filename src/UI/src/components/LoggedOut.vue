<template>
	<div id="loggedOut">
		<h2>
			你现在可以安全退出
		</h2>
		<template v-if="$route.query.postLogoutRedirectUri">
			<div>
				<el-link type="primary" :href="$route.query.postLogoutRedirectUri">返回 {{ $route.query.clientName }}
				</el-link>
			</div>
		</template>

		<template v-if="$route.query.signOutIframeUrl">
			<div style="display:none;">
				<iframe width="0" height="0" :src="$route.query.signOutIframeUrl"></iframe>
			</div>

		</template>
	</div>
</template>

<script>
export default {
	components: {},
	data() {
		return {}
	},
	beforeCreate() {
		if (this.$route.query.automaticRedirectAfterSignOut == 'true') {
			window.location = this.$route.query.postLogoutRedirectUri
		}
	},
}
</script>
<style scoped>
#loggedOut {
	text-align: center;
}
</style>
