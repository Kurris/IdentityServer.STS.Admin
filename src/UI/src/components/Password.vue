<template>
	<div id="password">
		<template v-if="status">
			<div>旧密码:<el-input v-model="form.oldPassword" /></div>
		</template>
		<template v-else>
			<div>你还没有设置密码，设置一个密码，以便无需外部登录即可登录</div>
		</template>

		<div>
			<div>新密码:<el-input v-model="form.newPassword"></el-input></div>
			<div>确定密码:<el-input v-model="form.confirmPassword"></el-input></div>
			<div>
				<el-button type="primary" @click="confirm()">{{ status ? '重置密码' : '设置密码' }}</el-button>
			</div>
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
			status: false,
		}
	},
	computed: {},
	watch: {},
	methods: {
		confirm() {
			if (!this.status) this.form.oldPassword = null

			savePassword(this.form).then(res => {
				if (res) {
					this.$router.push('/zone')
				}
			})
		},
	},
	async beforeMount() {
		let res = await checkPassword()
		this.status = res.data
	},
}
</script>
<style scoped></style>
