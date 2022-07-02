<template>
	<div id="signinWith2fa">
		<div>
			<div class="title">
				<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="65"></el-avatar>
				<div style="margin-top: 40px">
					<h2>双重身份验证</h2>
				</div>
			</div>

			<div class="panel">
				<div class="notice">
					<div style="text-align: center">
						<i class="el-icon-mobile" style="font-size: 40px; color: #555b65" />
					</div>

					<span style="font-size: 13px">验证码:</span>
					<el-input
						ref="input"
						v-model="model.twoFactorCode"
						maxlength="6"
						style="margin-bottom: 20px; margin-top: 10px"
						placeholder="请输入6位验证码"
						autofocus
						@keyup.enter.native="login"
					/>

					<el-button :loading="isLoading" style="width: 290px" type="primary" @click="login">验证</el-button>

					<p style="font-size: 13px; color: #636d74">您的登录使用身份验证器应用程序进行保护,打开双重验证器应用(TOTP)查看您的验证码</p>
					<div>
						<el-checkbox v-model="model.rememberMachine"> 记住当前设备 </el-checkbox>
					</div>
					<el-divider></el-divider>
				</div>
				<div class="footer">
					<TipLink tipText="无法访问您的身份验证设备?" hrefText="使用恢复码登录" @click="goUseRecoveryCode" />
				</div>
			</div>
		</div>
	</div>
</template>

<script>
import { checkTwoFactorAuthenticationUser, siginTwoFactorAuthenticationUser, getLoginStatus } from '../net/api.js'
import NProgress from 'nprogress'
import TipLink from './TipLink.vue'

export default {
	components: {
		TipLink,
	},
	data() {
		return {
			model: {
				twoFactorCode: '',
				rememberMachine: false,
				returnUrl: this.$route.query.returnUrl,
				rememberMe: this.$route.query.rememberMe,
				withExternalLogin: this.$route.query.withExternalLogin,
			},
			isLoading: false,
		}
	},
	methods: {
		async login() {
			this.isLoading = true
			let res = await siginTwoFactorAuthenticationUser(this.model).finally(() => {
				this.isLoading = false
				this.$refs['input'].focus()
			})

			if (res.route == 1) {
				NProgress.start()
				window.location.href = res.data
			} else if (res.route == 2) {
				let statusResult = await getLoginStatus()
				let userName = statusResult.data.user.userName
				this.$router.push({
					path: '/zone/' + userName,
				})
			}
		},
		async goUseRecoveryCode() {
			let returnUrl = this.$route.query.returnUrl
			//恢复码登录,禁止记住当前机器,记住我
			this.$router.push({
				path: '/loginWithRecoveryCode',
				query: {
					returnUrl,
				},
			})
		},
	},
	async beforeMount() {
		await checkTwoFactorAuthenticationUser()
	},
}
</script>
<style scoped>
#signinWith2fa {
	display: flex;
	height: 100vh;
	justify-content: center;
	background: #f6f8fa;
}

.title {
	text-align: center;
	margin-top: 30px;
	margin-bottom: 30px;
}

.panel {
	width: 350px;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	border-radius: 7px;
	background-color: white;
}

.notice {
	padding: 30px;
}

.footer {
	display: flex;
	justify-content: center;
	align-items: center;
	width: 350px;
	height: 65px;
	border-radius: 7px;
	margin-top: 20px;
	font-size: 14px;
	color: #92969a;
}
</style>
