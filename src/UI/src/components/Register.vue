<template>
	<div id="register">
		<div class="panel">
			<div class="container">
				<h4>注册一个新账户。</h4>
				<el-form ref="form" :model="form">
					<el-form-item>
						<el-input v-model="form.userName" autocomplete="off" placeholder="用户名" />
					</el-form-item>
					<el-form-item>
						<el-input type="email" v-model="form.email" autocomplete="off" placeholder="邮件" />
					</el-form-item>
					<el-form-item>
						<el-input type="password" v-model="form.password" autocomplete="off" placeholder="密码" />
					</el-form-item>
					<el-form-item>
						<el-input type="password" v-model="form.confirmPassword" autocomplete="off" placeholder="确认密码" />
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="register">注册新用户</el-button>
					</el-form-item>
				</el-form>
			</div>
		</div>
	</div>
</template>

<script>
import { register } from '../net/api'

export default {
	components: {},
	data() {
		return {
			form: {
				userName: '',
				email: '',
				password: '',
				confirmPassword: '',
			},
		}
	},
	computed: {},
	watch: {},
	methods: {
		async register() {
			await register({
				userName: this.form.userName,
				email: this.form.email,
				password: this.form.password,
				confirmPassword: this.form.confirmPassword,
			})
		},
	},
}
</script>
<style scoped>
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
	display: flex;
	flex-direction: column;
	justify-content: center;
	align-items: center;
	box-shadow: 10px 1px 50px 21px #d9d9d9;
	background-color: #ffffff;
}

>>> .el-input__inner {
	width: 300px !important;
}
.el-button {
	width: 300px;
}
</style>
