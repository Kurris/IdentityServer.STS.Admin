<template>
	<div id="comfirm">
		<el-input v-model="code" placeholder="密码"></el-input>
		<el-button type="primary" @click="">验证</el-button>
	</div>
</template>

<script>
export default {
	data() {
		return {
			code: '',
		}
	},
}
</script>
