<template>
	<div id="forgotPassword">
		<div class="panel">
			<div class="container">
				<h4>密码找回</h4>
				<el-form ref="form" :form="form">
					<el-form-item>
						<el-input v-model="form.content" placeholder="用户名/邮件" />
					</el-form-item>
					<el-form-item>
						<el-button type="primary" @click="commit()">发送邮件验证</el-button>
					</el-form-item>
				</el-form>
				<TipLink tipText="想起密码?" href="/signIn" hrefText="前往登录" />
			</div>
		</div>
	</div>
</template>

<script>
import { forgetPasswordAndSendEmail } from '../net/api.js'
import TipLink from './TipLink.vue'

export default {
	components: { TipLink },
	data() {
		return {
			form: {
				content: '',
			},
		}
	},
	methods: {
		changeType() {
			this.content = ''
		},
		async commit() {
			try {
				await forgetPasswordAndSendEmail({
					content: this.form.content,
				})

				this.$router.push({
					path: '/successed',
					query: {
						title: '已发送验证邮件',
						subTitle: '请您在邮件中进行验证,并且重置密码',
						returnUrl: '/signIn',
					},
				})
			} catch (err) {
				console.log(err)
			}
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
