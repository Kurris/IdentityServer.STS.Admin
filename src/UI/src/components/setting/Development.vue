<template>
	<div id="development">
		<h1>开发者设置(development)</h1>
		<el-tabs tab-position="top">
			<el-tab-pane label="OAuth应用(OAuth Apps)">
				<MyOAuthApp />
			</el-tab-pane>
			<el-tab-pane label="个人访问令牌(Person access tokens)">
				<Pat />
			</el-tab-pane>
		</el-tabs>
	</div>
</template>

<script>
import Pat from './Pat.vue'
import MyOAuthApp from './MyOAuthApp.vue'
export default {
	components: { Pat, MyOAuthApp },
}
</script>
<style scoped>
</style>
