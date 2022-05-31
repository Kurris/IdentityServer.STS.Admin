<template>
	<div id="externalLoginConfirmation">
		<div class="container">
			<el-form ref="form" :model="form" label-width="80px">
				<el-form-item>
					<el-input v-model="form.userName" placeholder="用户名"></el-input>
				</el-form-item>
				<el-form-item>
					<el-input v-model="form.email" placeholder="邮件地址"></el-input>
				</el-form-item>
				<el-form-item>
					<el-input v-model="form.password" placeholder="密码"></el-input>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" @click="externalRegister()">注册</el-button>
				</el-form-item>
			</el-form>
		</div>
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
				password:'',
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
<style scoped>
#externalLoginConfirmation {
	height: 100vh;
	width: 100%;
	display: flex;
	justify-content: center;
	align-items: center;
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
</style>
