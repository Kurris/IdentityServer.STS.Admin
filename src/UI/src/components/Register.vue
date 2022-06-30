<template>
	<div id="register">
		<div class="panel">
			<div class="container">
				<el-alert v-if="errorVisible" :title="errorTitle" type="error" show-icon style="width: 300px" :closable="false"> </el-alert>
				<h4>注册一个新账户。</h4>
				<el-form ref="form" :model="form">
					<el-form-item>
						<el-input v-model="form.userName" autocomplete="off" placeholder="用户名" name="userName" />
					</el-form-item>
					<el-form-item>
						<el-input type="email" v-model="form.email" autocomplete="off" placeholder="邮件" name="email" />
					</el-form-item>
					<el-form-item>
						<el-input type="password" v-model="form.password" autocomplete="off" placeholder="密码" name="password" />
					</el-form-item>
					<el-form-item>
						<el-input type="password" v-model="form.confirmPassword" autocomplete="off" placeholder="确认密码" name="confirmPassword" />
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="register" :loading="registerLoading">注册新用户</el-button>
					</el-form-item>
				</el-form>
				<TipLink tipText="已有账号?" href="/signIn" hrefText="前往登录" />
			</div>
		</div>
	</div>
</template>

<script>
import { register } from '../net/api'
import TipLink from './TipLink.vue'

export default {
	components: { TipLink },
	data() {
		return {
			form: {
				userName: '',
				email: '',
				password: '',
				confirmPassword: '',
			},
			errorVisible: false,
			errorTitle: '',
			registerLoading: false,
		}
	},
	methods: {
		async register() {
			this.registerLoading = true
			await register({
				userName: this.form.userName,
				email: this.form.email,
				password: this.form.password,
				confirmPassword: this.form.confirmPassword,
			})
				.then(() => {
					this.$router.push({
						path: '/successed',
						query: {
							title: '已发送验证邮件',
							subTitle: '请您在邮件中进行验证',
							returnUrl: '/signIn',
						},
					})
				})
				.catch(err => {
					console.log(err)
					// let that = this
					// setTimeout(() => {
					// 	that.errorVisible = false
					// }, 3000)
					// that.errorTitle = err
					// that.errorVisible = true
				})
				.finally(() => {
					this.registerLoading = false
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
