<template>
	<div id="externalLoginConfirmation" v-if="loginProvider">
		<div v-if="isLogin" class="container">
			<Connect :loginProvider="loginProvider" />
			<span style="margin-top: 20px; margin-bottom: 20px">您已经通过{{ loginProvider }}授权,完成登录即可完成账号绑定</span>
			<el-form ref="form" :model="form">
				<el-form-item>
					<el-input v-model="form.userName" placeholder="用户名" />
				</el-form-item>
				<el-form-item>
					<el-input type="password" v-model="form.password" placeholder="密码" />
				</el-form-item>
				<el-form-item>
					<el-button type="primary" @click="externalLoginWithLocalLogin">登录并绑定</el-button>
				</el-form-item>
			</el-form>
			<span style="font-size: 14px"> 没有账号? <el-link type="primary" :underline="false" style="padding-bottom: 3px" @click="switchType">注册</el-link> </span>
		</div>
		<div v-else class="container">
			<Connect :loginProvider="loginProvider" />
			<span style="margin-top: 20px; margin-bottom: 20px">您已经通过{{ loginProvider }}授权,完善以下信息即可完成账号绑定</span>
			<el-form ref="form" :model="form">
				<el-form-item>
					<div class="oneline">
						<el-input v-model="form.userName" placeholder="用户名" />
						<i v-if="checks.find(x => x == 'userName')" class="el-icon-success" style="font-size: 16px"></i>
					</div>
				</el-form-item>
				<el-form-item>
					<div class="oneline">
						<el-input v-model="form.email" placeholder="邮件地址" />
						<i v-if="checks.find(x => x == 'email')" class="el-icon-success" style="font-size: 16px"></i>
					</div>
				</el-form-item>
				<el-form-item>
					<el-input type="password" v-model="form.password" placeholder="密码" />
				</el-form-item>
				<el-form-item>
					<el-input type="password" v-model="form.confirmPassword" placeholder="确认密码" />
				</el-form-item>

				<el-form-item>
					<el-button type="primary" @click="externalRegister()">注册并绑定</el-button>
				</el-form-item>
			</el-form>
			<span style="font-size: 14px"> 已有账号? <el-link type="primary" :underline="false" style="padding-bottom: 3px" @click="switchType()">前往登录</el-link> </span>
		</div>
	</div>
</template>

<script>
import { externalRegister, checkExternalRegister, externalLoginWithLocalLogin } from '../net/api.js'
import Connect from './Connect.vue'

export default {
	components: {
		Connect,
	},
	data() {
		return {
			form: {
				email: this.$route.query.email,
				userName: this.$route.query.userName,
				password: '',
			},
			returnUrl: this.$route.query.returnUrl,
			loginProvider: this.$route.query.loginProvider,
			checks: [],
			isLogin: false,
		}
	},
	methods: {
		async externalRegister() {
			await externalRegister({
				usePassword: true,
				email: this.form.email,
				userName: this.form.userName,
				returnUrl: this.returnUrl,
				password: this.form.password,
				confirmPassword: this.form.confirmPassword,
			}).then(res => {
				if (res.route == 1) {
					let redirectUrl = res.data
					window.location.href = redirectUrl
				}
			})
		},
		switchType() {
			this.isLogin = !this.isLogin
		},
		async externalLoginWithLocalLogin() {
			let res = await externalLoginWithLocalLogin({
				userName: this.form.userName,
				returnUrl: this.returnUrl,
				password: this.form.password,
			})

			if (res.route == 1) {
				window.location.href = res.data
			} else if (res.route == 4) {
				this.$router.push({
					path: '/signinWith2fa',
					query: {
						rememberMe: res.data.rememberLogin,
						returnUrl: res.data.returnUrl,
						withExternalLogin: true,
					},
				})
			}
		},
	},
	async beforeMount() {
		await checkExternalRegister()
		console.log(this.$route.query)
	},
}
</script>
<style scoped>
#externalLoginConfirmation {
	height: 100vh;
	width: 100%;
	display: flex;
	flex-direction: column;
	justify-content: center;
	align-items: center;
	background-image: url(https://www.rancher.cn/imgs/footer-background.svg);
}

.container {
	display: flex;
	flex-direction: column;
	justify-content: center;
	align-items: center;
	height: 500px;
	width: 1000px;
	box-shadow: 10px 1px 50px 21px #d9d9d9;
	background-color: #ffffff;
}
.el-icon-success {
	color: #53a258;
}

.oneline {
	display: flex;
	align-items: center;
}

>>> .el-input__inner {
	width: 350px !important;
}

.el-button {
	width: 350px;
}
</style>
