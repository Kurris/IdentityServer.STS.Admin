<template>
	<div id="loginWithRecoveryCode">
		<div>
			<div class="title">
				<AppAvatar />
				<div style="margin-top: 40px">
					<h2>恢复码验证</h2>
				</div>
			</div>

			<div class="panel">
				<div class="notice">
					<div style="text-align: center">
						<i class="el-icon-key" style="font-size: 40px; color: #555b65" />
					</div>

					<span style="font-size: 13px">恢复码:</span>
					<el-input ref="input" maxlength="8" v-model="code" style="margin-bottom: 20px; margin-top: 10px"
						placeholder="请输入恢复码" @keyup.enter.native="login" v-focus />
					<el-button type="primary" style="width: 290px" @click="login" :loading="isLoading">恢复码验证</el-button>

					<p style="font-size: 13px; color: #636d74">您已请求使用恢复码登录,当您无法访问双重验证器所在的设备,输入一组恢复码来验证您的身份。</p>
					<el-divider></el-divider>
				</div>
				<div class="footer">
					<TipLink tipText="账号已被锁定?" hrefText="尝试恢复您的账号" />
				</div>
			</div>
		</div>
	</div>
</template>

<script>
import { signInWithCode, getLoginStatus, goSignInWithCode } from '../net/api.js'
import NProgress from 'nprogress'
import TipLink from './TipLink.vue'
import AppAvatar from './AppAvatar.vue'

export default {
	components: {
		TipLink,
		AppAvatar
	},
	data() {
		return {
			code: '',
			isLoading: false,
		}
	},
	methods: {
		async login() {
			this.isLoading = true
			let res = await signInWithCode({
				recoveryCode: this.code,
				returnUrl: this.$route.query.returnUrl,
			}).finally(() => {
				this.$refs['input'].focus()
				this.isLoading = false
			})

			if (res.route == 1) {
				NProgress.start()
				window.location.href = res.data
			} else if (res.route == 2) {
				let statusResult = await getLoginStatus()
				let userName = statusResult.data.user.userName
				this.$router.push('/zone/' + userName)
			}
		},
	},
	async beforeMount() {
		await goSignInWithCode()
	},
}
</script>
<style scoped>
#loginWithRecoveryCode {
	display: flex;
	height: 100vh;
	justify-content: center;
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
