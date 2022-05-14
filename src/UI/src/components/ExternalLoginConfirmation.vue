<template>
	<div id="externalLoginConfirmation">
		<el-form ref="form" :model="form" label-width="80px">
			<el-form-item label="用户名称">
				<el-input v-model="form.userName"></el-input>
			</el-form-item>
			<el-form-item label="邮件">
				<el-input v-model="form.email"></el-input>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="externalRegister()">注册</el-button>
			</el-form-item>
		</el-form>
	</div>
</template>

<script>
import { externalRegister } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			form: {
				email: this.$route.query.email,
				userName: this.$route.query.userName,
				returnUrl: this.$route.query.returnUrl,
			},
		}
	},
	computed: {},
	watch: {},
	methods: {
		async externalRegister() {
			await externalRegister({
				email: this.form.email,
				userName: this.form.userName,
				returnUrl: this.form.returnUrl,
			}).then(res => {
				if (res.route == 1) {
					let redirectUrl = res.data
					window.location.href = redirectUrl
				}
			})
		},
	},
}
</script>
<style scoped></style>
