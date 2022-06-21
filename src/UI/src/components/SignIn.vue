<template>
	<div class="panel" v-if="show">
		<div class="container">
			<div class="slot">
				<img src="../assets/login_left.svg" alt="" srcset="" />
			</div>
			<div class="signin">
				<div class="title">
					<h1>登录</h1>
					<div>没有帐号？<el-link type="success" @click="$router.push('/register')" :underline="false">点此注册</el-link></div>
				</div>
				<template>
					<el-form>
						<el-form-item>
							<el-input type="text" v-model="form.userName" placeholder="用户名/账号" />
						</el-form-item>
						<el-form-item>
							<el-input type="password" v-model="form.password" placeholder="密码" />
						</el-form-item>
						<el-form-item>
							<el-checkbox label="记住我" v-model="form.remember" name="type"></el-checkbox>
							<el-link type="success" style="float: right" @click="$router.push('/forgotPassword')" :underline="false">忘记密码?</el-link>
						</el-form-item>
						<el-form-item>
							<el-button style="width: 100%" type="success" @click="login()" :loading="loginLoading">登录</el-button>
						</el-form-item>
					</el-form>
				</template>

				<div v-if="setting.externalProviders != null && setting.externalProviders.length > 0">
					<el-divider content-position="center"><span style="color: #8d92a2">其他登录</span></el-divider>
					<div class="externalLogins">
						<template v-for="(item, i) in setting.externalProviders">
							<template v-if="item.displayName == 'Alipay'">
								<div :key="i">
									<img class="externalProvider" src="../assets/auth2logo/Alipay-32.svg" @click="externalLogin(item.authenticationScheme)" title="使用支付宝登录" />
								</div>
							</template>
							<template v-else-if="item.displayName == 'GitHub'">
								<div :key="i">
									<img class="externalProvider" src="../assets/auth2logo/GitHub-32.png" @click="externalLogin(item.authenticationScheme)" title="使用github登录" />
								</div>
							</template>
							<template v-else-if="item.displayName == 'Weibo'">
								<div :key="i">
									<img class="externalProvider" src="../assets/auth2logo/WeiBo-32.png" @click="externalLogin(item.authenticationScheme)" title="使用微博登录" />
								</div>
							</template>
							<template v-else-if="item.displayName == 'Discord'">
								<div :key="i">
									<img class="externalProvider" src="../assets/auth2logo/Discord-32.svg" @click="externalLogin(item.authenticationScheme)" title="使用discord登录" />
								</div>
							</template>
						</template>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
// getLoginStatus
import { signIn, checkLogin, getLoginStatus } from '../net/api.js'

import NProgress from 'nprogress'

export default {
	name: 'signIn',
	data() {
		return {
			form: {
				userName: '',
				password: '',
				remember: false,
			},
			setting: null,
			show: false,
			loginLoading: false,
		}
	},
	methods: {
		async login() {
			const returnUrl = this.$route.query.returnUrl
			const userName = this.form.userName
			const password = this.form.password

			this.loginLoading = true
			let response = await signIn({
				userName,
				password,
				returnUrl,
				rememberLogin: this.form.remember,
				requestType: 'login',
			}).finally(() => {
				this.loginLoading = false
			})

			if (response.route == 2) {
				let res = await getLoginStatus()
				let userName = res.data.user.userName
				console.log(userName)
				this.$router.push({
					path: '/zone/' + userName,
				})
			} else if (response.route == 1) {
				window.location = response.data
			} else if (response.route == 4) {
				this.$router.push({
					path: '/signinWith2fa',
					query: {
						rememberMe: response.data.rememberLogin,
						returnUrl: response.data.returnUrl,
					},
				})
			}
		},
		async externalLogin(provider) {
			let returnUrl = this.$route.query.returnUrl || ''

			NProgress.start()
			let url = 'http://101.35.47.169:5000/api/account/externalLogin'

			document.write('<form action=' + url + " method=post name=form1 style='display:none'>")
			document.write("<input type=hidden name=provider value='" + provider + "'/>")
			document.write("<input type=hidden name=returnUrl value='" + returnUrl + "'/>")
			document.write('</form>')

			try {
				document.form1.submit()
			} catch (error) {
				console.log(error)
			}
		},
	},
	beforeMount() {
		checkLogin({ returnUrl: this.$route.query.returnUrl }).then(res => {
			console.log(res)
			this.setting = res.data
			if (this.setting.userName) {
				this.form.userName = this.setting.userName
			}

			this.show = this.setting.enableLocalLogin
			let provider = this.setting.externalProviders[0].authenticationScheme
			if (!this.show && provider) {
				this.externalLogin(provider)
			}
		})
	},
}
</script>

<style scoped>
/* 容器的样式 */
.panel {
	height: 100vh;
	width: 100%;
	display: flex;
	justify-content: center;
	align-items: center;
	/* background-image: url(https://www.rancher.cn/imgs/footer-background.svg); */
}

.container {
	height: 500px;
	width: 1000px;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	display: flex;
	justify-content: center;
	align-items: center;
	background-color: #ffffff;
}

>>> .el-input__inner {
	width: 300px !important;
}
.signin {
	min-width: 300px;
}

.signin .title {
	display: flex;
	justify-content: space-between;
	align-items: center;
	margin-bottom: 30px;
	font-weight: 400;
	font-size: 14px;
}

.signin .title .el-link {
	padding-bottom: 2px;
}

img.externalProvider:hover {
	cursor: pointer;
}

.externalLogins {
	display: flex;
	align-items: center;
	justify-content: space-around;
}
</style>
