<template>
	<div id="twoFactorAuthentication" v-loading="isLoading">
		<div class="flex">
			<h2>双重身份验证(2FA)</h2>
			<template v-if="setting != null">
				<el-button v-if="setting.is2FaEnabled" type="danger" @click="enable2Fa(false)">停用2FA</el-button>
				<el-button v-else type="success" @click="enable2Fa(true)">启用</el-button>
			</template>
		</div>
		<el-divider></el-divider>
		<div v-if="setting != null">
			<template v-if="setting.is2FaEnabled">
				<el-card shadow="hover" v-if="setting.isMachineRemembered">
					<div class="flex">
						<span>已记住当前设备</span>
						<el-button type="success" @click="forget2faBrowser">忘记这个设备</el-button>
					</div>
				</el-card>
				<el-card shadow="hover">
					<p v-if="setting.recoveryCodesLeft <= 3">
						<el-alert type="warning" show-icon :closable="false">
							<template slot="title">
								<span>你剩下{{ setting.recoveryCodesLeft }}个恢复码,您应该重新生成一组新的恢复码</span>
							</template>
						</el-alert>
					</p>

					<div class="flex">
						<span>恢复码</span>
						<el-button type="danger" @click="resetRecoveryCode()">重新生成恢复码</el-button>
					</div>

					<template v-if="recoveryCodes.length > 0">
						<p>
							<el-alert title="新的恢复码将不会再显示,请妥善保管,确保您丢失了设备,无法访问自己的帐户时,能够使用恢复码进行登录" type="warning" show-icon :closable="false"> </el-alert>
							<template v-for="(code, index) in recoveryCodes">
								<template v-if="index % 3 == 0">
									<br :key="code" />
								</template>
								<el-tag style="margin-left: 10px" type="info" :key="code">{{ code }}</el-tag>
							</template>
						</p>
					</template>
				</el-card>
				<el-card shadow="hover">
					<div class="flex">
						<span>身份验证器应用</span>
						<div>
							<template v-if="!setting.hasAuthenticator">
								<el-button type="primary" @click="setup()">添加身份验证器应用</el-button>
							</template>
							<template v-else>
								<el-button type="primary" @click="setup()">设置身份验证器应用</el-button>
								<el-button type="danger" @click="reset()">重置身份验证器应用</el-button>
							</template>
						</div>
					</div>
				</el-card>
			</template>
		</div>
		<EnalbeAuthenticator :authenticatorVisible="authenticatorVisible" @close="close" />
	</div>
</template>

<script>
import { getTwofactorSetting, forget2faClient, enable2Fa, resetAuthenticator, getRecoveryCodes } from '../../net/api.js'
import EnalbeAuthenticator from '../EnableAuthenticator.vue'
export default {
	components: {
		EnalbeAuthenticator,
	},
	data() {
		return {
			setting: null,
			authenticatorVisible: false,
			recoveryCodes: [],
			isLoading: false,
		}
	},
	computed: {},
	watch: {},
	methods: {
		async close() {
			this.authenticatorVisible = false
			this.refresh()
		},
		setup() {
			this.authenticatorVisible = true
		},
		reset() {
			this.$confirm('请阅读每个提醒和警告,确保您理解当前的操作,重置身份验证器应用是一个危险操作,除非您泄漏了身份验证二维码或者key,否则请勿操作,是否还要继续?', '危险', {
				confirmButtonText: '我会阅读每个提醒和警告',
				cancelButtonText: '取消',
				type: 'error',
			})
				.then(() => {
					this.$confirm('重置身份验证器应用需要您重新在身份验证应用程序中进行设置,否则双重验证功能将会关闭,您的账号信息安全存在风险,是否要继续?', '危险', {
						confirmButtonText: '重置并且执行身份验证应用程序替换',
						cancelButtonText: '取消',
						type: 'error',
					})
						.then(() => {
							resetAuthenticator().then(() => {
								this.authenticatorVisible = true
							})
						})
						.catch(() => {})
				})
				.catch(() => {})
		},
		async forget2faBrowser() {
			await forget2faClient()
			let res = await getTwofactorSetting()
			this.setting = res.data
		},
		async enable2Fa(enable) {
			if (enable) {
				this.authenticatorVisible = true
			} else {
				await enable2Fa({ enable })
				await this.refresh()
			}
		},
		resetRecoveryCode() {
			this.$confirm('重新生成恢复码将会导致旧的恢复码失效,是否要继续?', '提醒', {
				confirmButtonText: '生成恢复码',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(() => {
					getRecoveryCodes().then(res => {
						this.recoveryCodes = res.data
						this.refresh()
					})
				})
				.catch(() => {})
		},
		async refresh() {
			this.isLoading = true
			let res = await getTwofactorSetting().finally(() => {
				this.isLoading = false
			})
			this.setting = res.data
		},
	},

	async beforeMount() {
		await this.refresh()
	},
}
</script>
<style scoped>
.flex {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
</style>
