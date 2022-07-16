<template>
	<div id="resetPassword">
		<div class="panel">
			<div class="container">
				<h4>重置密码</h4>
				<el-form>
					<el-form-item>
						<el-input v-model="email" placeholder="邮件地址"></el-input>
					</el-form-item>
					<el-form-item>
						<el-input v-model="password" type="password" show-password placeholder="新密码"></el-input>
					</el-form-item>
					<el-form-item>
						<el-input v-model="confirmPassword" type="password" show-password placeholder="确定密码"></el-input>
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="resetPassword" :loading="loading">重置密码</el-button>
					</el-form-item>
				</el-form>
			</div>
		</div>
	</div>
</template>

<script>
import { resetPassword } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			confirmPassword: '',
			password: '',
			email: this.$route.query.email,
			code: this.$route.query.code,
			loading: false,
		}
	},

	methods: {
		async resetPassword() {
			this.loading = true
			await resetPassword({
				code: this.code,
				email: this.email,
				password: this.password,
				confirmPassword: this.confirmPassword,
			}).finally(() => {
				this.loading = false
			})

			this.$router.push({
				path: '/successed',
				query: {
					title: '密码已重置',
					subTitle: '现在可以前往登录',
					returnUrl: '/signIn',
				},
			})
		},
	},
}
</script>
<style scoped>
.panel {
	height: 100vh;
	width: 100%;
	display: flex;
	justify-content: center;
	align-items: center;
}

.container {
	height: 500px;
	width: 1000px;
	display: flex;
	flex-direction: column;
	justify-content: center;
	align-items: center;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	background-color: #ffffff;
}

>>> .el-input__inner {
	width: 300px !important;
}

.el-button {
	width: 300px;
}
</style>
