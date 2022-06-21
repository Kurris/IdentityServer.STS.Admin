<template>
	<div id="password">
		<div v-if="hasPassword != null">
			<div>
				<b style="font-size: 30px">密码(Password)</b>
			</div>
			<el-divider></el-divider>
			<el-card shadow="hover">
				<el-form ref="form" :form="form">
					<el-form-item :label="hasPassword ? '旧密码' : ''">
						<template v-if="hasPassword">
							<el-input v-model="form.oldPassword" type="password" autocomplete="off" name="oldPassword"></el-input>
						</template>
						<template v-else>
							<el-alert type="warning" title="你还没有设置密码，设置一个密码，以便无需外部登录即可登录" :closable="false"> </el-alert>
						</template>
					</el-form-item>
					<el-form-item label="新密码">
						<el-input type="password" v-model="form.newPassword" name="newPassword"></el-input>
					</el-form-item>
					<el-form-item label="确定密码">
						<el-input type="password" v-model="form.confirmPassword" name="confirmPassword"></el-input>
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="confirm" :loading="isLoading">{{ hasPassword ? '更新密码' : '设置密码' }}</el-button>
						<el-link style="margin-left: 10px" type="primary" :underline="false">我忘记了密码</el-link>
					</el-form-item>
				</el-form>
			</el-card>
		</div>
	</div>
</template>

<script>
import { checkPassword, savePassword } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			form: {
				oldPassword: '',
				newPassword: '',
				confirmPassword: '',
			},
			hasPassword: null,
			isLoading: false,
		}
	},
	methods: {
		async confirm() {
			if (!this.hasPassword) this.form.oldPassword = null

			this.isLoading = true
			await savePassword(this.form).finally(() => {
				this.isLoading = false
			})

			this.form = {
				oldPassword: '',
				newPassword: '',
				confirmPassword: '',
			}

			this.$notify({
				title: '成功',
				message: `${this.hasPassword ? '更新密码' : '设置密码'}成功`,
				type: 'success',
			})
			await this.refresh()
		},
		async refresh() {
			let res = await checkPassword()
			this.hasPassword = res.data
		},
	},
	async beforeMount() {
		await this.refresh()
	},
}
</script>
<style scoped></style>
