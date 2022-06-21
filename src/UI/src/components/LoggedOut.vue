<template>
	<div id="loggedOut">
		<h1>你现在可以安全退出</h1>
		<template v-if="$route.query.postLogoutRedirectUri">
			<div>
				<el-link type="primary" :href="$route.query.postLogoutRedirectUri">返回 {{ $route.query.clientName }} </el-link>
			</div>
		</template>

		<template v-if="$route.query.signOutIframeUrl">
			<div style="display: none">
				<iframe width="0" height="0" :src="$route.query.signOutIframeUrl"></iframe>
			</div>
		</template>
	</div>
</template>

<script>
export default {
	beforeCreate() {
		if (Boolean(parseInt(this.$route.query.automaticRedirectAfterSignOut)) == true) {
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
