<template>
	<div id="signin" v-if="show">
		<!-- <img class="bg" src="https://www.rancher.cn/imgs/footer-background.svg" alt="" srcset="" /> -->
		<div class="panel">
			<div :class="{ container_mobile: !slotVisible, container: slotVisible }">
				<div class="slot" v-if="slotVisible">
					<img src="../assets/login_left.svg" alt="" srcset="" />
				</div>

				<div v-if="isPre" class="tenantSelect">
					<div class="title">
						<h1>选择你的基地</h1>
						<div style="color: #a6a7a8;cursor: pointer;" @click="() => isPre = false">
							<i class="el-icon-arrow-left"></i>
							返回
						</div>
					</div>

					<div>
						<el-form>
							<el-scrollbar style="height: 300px;">
								<div v-for="item in tenants" :key="item.id">
									<el-form-item class="tenant">
										<div style="font-size: 13.6px;color: #171a1d; background-color: #f2f2f6; display: flex;justify-content: space-between;align-items: center;border-radius: 5%;
	padding: 10px;" @click="login(item.code)">
											{{ item.name }}
											<i class="el-icon-right"></i>
										</div>
									</el-form-item>
								</div>
							</el-scrollbar>
						</el-form>
					</div>
				</div>
				<div v-else class="signin">
					<div class="title">
						<h1>登录</h1>
						<!-- @click="$router.push('/register')" -->
						<div>没有帐号？<el-link type="primary" :underline="false">点此注册
							</el-link>
						</div>
					</div>

					<div v-if="loginType == 'password'" id="accountpassword" style="padding-top: 30px;">
						<el-form>
							<el-form-item>
								<el-input v-focus type="text" v-model="form.userName" placeholder="用户名/账号"
									@keyup.enter.native="login(null)" prefix-icon="el-icon-user" />
							</el-form-item>
							<el-form-item>
								<el-input type="password" v-model="form.password" placeholder="密码"
									@keyup.enter.native="login(null)" show-password prefix-icon="el-icon-key" />
							</el-form-item>
							<el-form-item>
								<el-checkbox label="记住我" v-model="form.remember" name="type"></el-checkbox>
								<!-- @click="$router.push('/forgotPassword')" -->
								<el-link type="primary" style="float: right" :underline="false">忘记密码?</el-link>
							</el-form-item>
							<el-form-item>
								<el-button style="width: 100%" type="primary" @click="login(null)" :loading="loginLoading">
									登录
								</el-button>
								<!-- <div style="float: right;margin-top: 4px;" @click="changeLoginType()">
									<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor"
										class="bi bi-qr-code" viewBox="0 0 16 16">
										<path d="M2 2h2v2H2V2Z" />
										<path d="M6 0v6H0V0h6ZM5 1H1v4h4V1ZM4 12H2v2h2v-2Z" />
										<path d="M6 10v6H0v-6h6Zm-5 1v4h4v-4H1Zm11-9h2v2h-2V2Z" />
										<path
											d="M10 0v6h6V0h-6Zm5 1v4h-4V1h4ZM8 1V0h1v2H8v2H7V1h1Zm0 5V4h1v2H8ZM6 8V7h1V6h1v2h1V7h5v1h-4v1H7V8H6Zm0 0v1H2V8H1v1H0V7h3v1h3Zm10 1h-1V7h1v2Zm-1 0h-1v2h2v-1h-1V9Zm-4 0h2v1h-1v1h-1V9Zm2 3v-1h-1v1h-1v1H9v1h3v-2h1Zm0 0h3v1h-2v1h-1v-2Zm-4-1v1h1v-2H7v1h2Z" />
										<path d="M7 12h1v3h4v1H7v-4Zm9 2v2h-3v-1h2v-1h1Z" />
									</svg>
								</div> -->
							</el-form-item>
						</el-form>
					</div>
					<div v-else id="qrcode" style="display: flex;justify-content: center;align-items: center;">

						<div style="position: relative;height: 213px;width: 213px;">

							<div v-if="qrCodeResult == 'Wait'" style="position: absolute; z-index: 3;">
								<qriously v-if="qrCode != null" :value="qrCode" :size="213" />
							</div>
							<div v-else-if="qrCodeResult == 'Expired'">
								<div style="position: absolute;z-index: 2;opacity: 0.1;">
									<qriously v-if="qrCode != null" :value="qrCode" :size="213" />
								</div>
								<div style="position: absolute;z-index: 3;">
									<div
										style="display: flex;justify-content: center;align-items: center;height: 213px;width: 213px;z-index: 10">
										<el-link type="primary" @click="createQrCode" :underline="false">已过期,请刷新
											<i class="el-icon-refresh-right"></i>
										</el-link>
									</div>
								</div>
							</div>
							<div v-else-if="qrCodeResult == 'WaitConfirm'">
								<div style="position: absolute;z-index: 2;opacity: 0.1;">
									<qriously v-if="qrCode != null" :value="qrCode" :size="213" />
								</div>
								<div style="position: absolute;z-index: 3;">
									<div
										style="display: flex;justify-content: center;align-items: center;height: 213px;width: 213px;z-index: 10">
										<el-link type="success" @click="createQrCode" :underline="false">等待确认登陆
										</el-link>
									</div>
								</div>
							</div>
							<div v-else-if="qrCodeResult != null && qrCodeResult == 'Success'">
								<div style="position: absolute;z-index: 2;opacity: 0.1;">
									<qriously v-if="qrCode != null" :value="qrCode" :size="213" />
								</div>
								<div style="position: absolute;z-index: 3;">
									<div
										style="display: flex;justify-content: center;align-items: center;height: 213px;width: 213px;z-index: 10">
										<el-link type="success" @click="createQrCode" :underline="false">登陆成功,等待跳转
										</el-link>
									</div>
								</div>
							</div>
						</div>
						<i class="el-icon-key" @click="changeLoginType"></i>
					</div>



					<div v-if="setting.externalProviders != null && setting.externalProviders.length > 0">
						<el-divider content-position="center"><span style="color: #8d92a2">其他外部登录</span></el-divider>
						<div class="externalLogins">
							<template v-for="(item, i) in setting.externalProviders">
								<template v-if="item.displayName == 'Alipay'">
									<div :key="i">
										<img class="externalProvider" src="../assets/auth2logo/Alipay-32.svg"
											@click="externalLogin(item.authenticationScheme)" title="使用支付宝登录" />
									</div>
								</template>
								<template v-else-if="item.displayName == 'GitHub'">
									<div :key="i">
										<img class="externalProvider" src="../assets/auth2logo/GitHub-32.png"
											@click="externalLogin(item.authenticationScheme)" title="使用github登录" />
									</div>
								</template>
								<template v-else-if="item.displayName == 'Weibo'">
									<div :key="i">
										<img class="externalProvider" src="../assets/auth2logo/WeiBo-32.png"
											@click="externalLogin(item.authenticationScheme)" title="使用微博登录" />
									</div>
								</template>
								<template v-else-if="item.displayName == 'Discord'">
									<div :key="i">
										<img class="externalProvider" src="../assets/auth2logo/Discord-32.svg"
											@click="externalLogin(item.authenticationScheme)" title="使用discord登录" />
									</div>
								</template>
								<template v-else-if="item.displayName == 'Jinko'">
									<div :key="i">
										<img class="externalProvider" src="../assets/auth2logo/JInko.png"
											@click="externalLogin(item.authenticationScheme)" title="使用晶彩登录" />
									</div>
								</template>
							</template>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script>

import { signIn, checkLogin, getLoginStatus, signInWithQrCode, preLogin } from '../net/api.js'
import { newCode, getScanResult } from '../net/qrCode.js'
import { baseURL } from '../utils/apiUrlHelper'
import NProgress from 'nprogress'

export default {
	name: 'signIn',
	data() {
		return {
			loginType: 'password',
			form: {
				userName: '',
				password: '',
				remember: false,
			},
			isPre: false,
			setting: null,
			show: false,
			loginLoading: false,
			clientWidth: document.body.clientWidth,
			slotVisible: false,
			qrCode: null,
			qrCodeResult: null,
			tenants: []
		}
	},
	methods: {
		async login(tenant) {
			let returnUrl = this.$route.query.returnUrl
			const userName = this.form.userName
			const password = this.form.password

			let preLoginResult = await preLogin({
				userName, password
			})

			this.tenants = preLoginResult.data
			if (this.tenants != null && this.tenants.length > 0 && !tenant) {
				this.isPre = true
			} else {
				this.loginLoading = true
				let response = await signIn({
					userName,
					password,
					returnUrl,
					rememberLogin: this.form.remember,
					requestType: 'login',
					tenant
				}).finally(() => {
					this.loginLoading = false
				})

				if (userName != 'admin' && (returnUrl == '' || returnUrl == undefined)) {
					this.$router.push({
						path: '/404',
					})
				} else {
					if (response.route == 2) {
						let res = await getLoginStatus()
						let userName = res.data.user.userName

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
				}
			}
		},
		async externalLogin(provider) {
			let returnUrl = this.$route.query.returnUrl || ''

			NProgress.start()
			let url = `${baseURL}/api/account/externalLogin`

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
		changeLoginType() {
			if (this.loginType == 'password') {
				this.loginType = 'qrCode'
			} else {
				this.loginType = 'password'
			}
		},
		startGetQrCodeScanResult() {

			setInterval(() => {
				if (this.loginType == 'qrCode' && (this.qrCodeResult == 'Wait' || this.qrCodeResult == 'WaitConfirm')) {
					getScanResult({
						key: this.qrCode
					}).then(x => {
						this.qrCodeResult = x.data
						if (this.qrCodeResult == 'Success') {
							const returnUrl = this.$route.query.returnUrl
							signInWithQrCode({
								returnUrl,
								key: this.qrCode
							}).then(response => {
								if (response.route == 2) {
									getLoginStatus().then(res => {
										let userName = res.data.user.userName
										this.$router.push({
											path: '/zone/' + userName,
										})
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
							})
						}
						//取消,重置qrcode
						else if (this.qrCodeResult == 'Denied') {
							this.createQrCode()
						}
					})
				}
			}, 5000)
		},
		createQrCode() {
			newCode().then(x => {
				this.qrCode = x.data
				this.qrCodeResult = 'Wait'
			})
		}
	},
	watch: {
		clientWidth(newVal) {
			this.slotVisible = !(newVal < 620)
		},
		loginType(newValue) {
			if (newValue == 'qrCode') {
				this.createQrCode()
			}
		}
	},
	async beforeMount() {
		let that = this
		window.onresize = () => {
			return (() => {
				window.clientWidth = document.body.clientWidth
				that.clientWidth = window.clientWidth
			})()
		}
		this.slotVisible = !(this.clientWidth < 620)

		checkLogin({ returnUrl: this.$route.query.returnUrl }).then(res => {
			this.setting = res.data
			if (this.setting.userName) {
				this.form.userName = this.setting.userName
			}

			this.show = this.setting.enableLocalLogin
			if (!this.show) {
				let provider = this.setting.externalProviders[0].authenticationScheme
				if (provider) {
					this.externalLogin(provider)
				}
			}
		})
		this.startGetQrCodeScanResult()
	}
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
}

.container {
	height: 500px;
	width: 1000px;
	border-radius: 10px;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	display: flex;
	justify-content: center;
	align-items: center;
	background-color: #ffffff;
}

.container_mobile {
	display: flex;
	justify-content: center;
	align-items: center;
	margin-bottom: 150px;
	border-radius: 10px;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	padding: 30px;
}

>>>.el-input__inner {
	width: 300px !important;
}

.signin {
	width: 300px;
	height: 400px;
}

.tenantSelect {
	width: 300px;
	height: 400px;
}

.title {
	display: flex;
	justify-content: space-between;
	align-items: center;
	font-weight: 400;
	font-size: 14px;
}


.bi-qr-code:hover {
	cursor: pointer;
}

.el-icon-key:hover {
	cursor: pointer;
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

img.bg {
	position: absolute;
	z-index: -1;
	left: 0;
	bottom: 0;
}

.tenant {
	/* background-color: #f5ecec */
}

>>>.el-form-item__content.tenant {
	/* background-color: #f2f2f6; */
	border-radius: 5%;
	padding: 10px;
}

>>>.tenant .el-form-item__content {
	transition: all 0.3s ease-in-out;

}


>>>.tenant .el-form-item__content:hover {
	cursor: pointer;
	padding-left: 5px;
	margin-right: -5px;
	/* background-color: #9aeec5 */
}

>>>.el-scrollbar__wrap {
	overflow-x: hidden;
}
</style>
