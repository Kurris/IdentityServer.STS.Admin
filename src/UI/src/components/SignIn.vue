<template>
	<div class="panel">
		<div class="container">
			<div class="slot">
				<img src="../assets/review.png" alt="" srcset="" />
			</div>
			<div class="signin">
				<div class="title">
					<h1>登录</h1>
					<div>没有帐号？<el-link type="success" @click="$router.push('/register')">点此注册</el-link></div>
				</div>
				<el-form>
					<el-form-item>
						<el-input type="text" v-model="form.userName" placeholder="Username" />
					</el-form-item>
					<el-form-item>
						<el-input type="password" v-model="form.password" placeholder="Password" />
					</el-form-item>
					<el-form-item>
						<el-checkbox label="记住我" v-model="form.remember" name="type"></el-checkbox>
						<el-link type="success" style="float: right" @click="$router.push('/forgotPassword')">忘记密码?</el-link>
					</el-form-item>
					<el-form-item>
						<el-button style="width: 100%" type="success" @click="login()">登录</el-button>
					</el-form-item>
				</el-form>

				<!-- <div>
                        <el-button @click="cancel()">取消</el-button>
                    </div> -->

				<div v-if="externalProviders != null && externalProviders.length > 0">
					<el-divider content-position="center"><span style="color: #8d92a2">其他登录</span></el-divider>
					<template v-for="(item, i) in externalProviders">
						<template v-if="item.displayName == 'GitHub'">
							<div :key="i">
								<img src="../assets/auth2logo/GitHub-Mark-32px.png" @click="externalLogin(item.authenticationScheme)" title="使用github登录" />
							</div>
						</template>
					</template>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
import { signIn, checkLogin } from '../net/api.js'

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
			externalProviders: [],
		}
	},
	methods: {
		async login() {
			const returnUrl = this.$route.query.returnUrl
			console.log(returnUrl)
			const username = this.form.userName
			const password = this.form.password

			let response = await signIn({
				username,
				password,
				returnUrl,
				rememberLogin: this.form.remember,
				requestType: 'login',
			})

			if (response.route == 2) {
				this.$router.push('/home')
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
		cancel() {
			let returnUrl = this.$route.query.returnUrl
			if (returnUrl === undefined) {
				this.$router.push('/home')
			} else {
				window.location.href = returnUrl
			}
		},
		async externalLogin(provider) {
			let returnUrl = this.$route.query.returnUrl

			if (returnUrl === undefined) {
				returnUrl = location.protocol + '//' + location.host
			}
			NProgress.start()
			let url = 'http://localhost:5000/api/authenticate/externalLogin'
			// window.location = url

			document.write('<form action=' + url + " method=post name=form1 style='display:none'>")
			document.write("<input type=hidden name=provider value='" + provider + "'/>")
			document.write("<input type=hidden name=returnUrl value='" + returnUrl + "'/>")
			document.write('</form>')
			document.form1.submit()
		},
	},
	beforeMount() {
		checkLogin({ returnUrl: this.$route.query.returnUrl }).then(res => {
			this.externalProviders = res.data.externalProviders
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
	background-image: url(https://www.rancher.cn/imgs/footer-background.svg);
}

.container {
	height: 500px;
	width: 1000px;
	box-shadow: 10px 1px 50px 21px #d9d9d9;
	display: flex;
	justify-content: space-around;
	align-items: center;
	background-color: #ffffff;
}
>>> .el-input__inner {
	width: 300px !important;
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

img:hover {
	cursor: pointer;
}
</style>
