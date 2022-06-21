<template>
	<div id="profile">
		<el-form ref="form" :form="profile">
			<el-form-item>
				<el-avatar shape="square" :size="100" :src="profile.picture"></el-avatar>
			</el-form-item>
			<el-form-item label="用户名 (暂不支持修改)">
				<el-input v-model="profile.userName" disabled />
			</el-form-item>
			<el-form-item label="Email">
				<el-input v-model="profile.email" />
				<template v-if="!profile.isEmailConfirmed">
					<div style="display: flex; align-items: center">
						<el-link type="primary" @click="sendVerificationEmail" :underline="false" :disabled="isSend">{{ isSend ? '已发送验证邮件' : '发送验证邮件' }}</el-link>
						<i v-if="isSend" class="el-icon-success" style="font-size: 16px; color: #40b883"></i>
						<span style="margin-left: 10px" v-if="isSend">剩余{{ timeInterval }}秒才可继续发送邮件</span>
					</div>
				</template>
			</el-form-item>
			<el-form-item label="电话号码">
				<el-input v-model="profile.phoneNumber" />
			</el-form-item>
			<el-form-item label="全名">
				<el-input v-model="profile.name" />
			</el-form-item>
			<el-form-item label="网站 URL">
				<el-input v-model="profile.website" />
			</el-form-item>
			<el-form-item label="个人资料 URL">
				<el-input v-model="profile.profile" />
			</el-form-item>
			<el-form-item label="街道地址">
				<el-input v-model="profile.streetAddress" />
			</el-form-item>
			<el-form-item label="城市">
				<el-input v-model="profile.locality" />
			</el-form-item>
			<el-form-item label="地区">
				<el-input v-model="profile.region" />
			</el-form-item>
			<el-form-item label="邮政编码">
				<el-input v-model="profile.postalCode" />
			</el-form-item>
			<el-form-item label="国家">
				<el-input v-model="profile.country" />
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="saveProfile" style="width: 100%">保存</el-button>
			</el-form-item>
		</el-form>
	</div>
</template>

<script>
import { getProfile, saveProfile } from '../../net/api.js'
export default {
	data() {
		return {
			profile: {},
			isSend: false,
			timeInterval: 60,
			timer: null,
		}
	},
	methods: {
		async sendVerificationEmail() {
			if (this.timer != null) {
				clearInterval(this.timer)
				this.timer = null
			}

			this.timeInterval = 60
			this.isSend = true

			this.timer = setInterval(() => {
				this.timeInterval--
				if (this.timeInterval <= 0) {
					clearInterval(this.timer)
					this.isSend = false
					this.timer = null
				}
			}, 1000)
		},
		async saveProfile() {
			await saveProfile(this.profile)
			this.$notify({
				title: '提醒',
				type: 'success',
				message: '成功保存',
			})
		},
	},
	beforeDestroy() {
		clearInterval(this.timer)
		this.timer = null
	},
	async beforeMount() {
		let res = await getProfile()
		this.profile = res.data
	},
}
</script>
<style scoped></style>
