<template>
	<div id="register">
		<h4>注册一个新账户。</h4>

		用户名
		<el-input v-model="userName" />
		邮件
		<el-input v-model="email" />

		密码
		<el-input v-model="password" />

		确认密码
		<el-input v-model="confirmPassword" />

		<el-button type="primary" @click="register">注册</el-button>
	</div>
</template>

<script>
import { register } from '../net/api'

export default {
	components: {},
	data() {
		return {
			userName: '',
			email: '',
			password: '',
			confirmPassword: '',
		}
	},
	computed: {},
	watch: {},
	methods: {
		async register() {
			await register({
				userName: this.userName,
				email: this.email,
				password: this.password,
				confirmPassword: this.confirmPassword,
			})
		},
	},
}
</script>
<style scoped></style>
