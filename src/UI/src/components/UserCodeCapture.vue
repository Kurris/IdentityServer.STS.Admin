<template>
	<div id="userCodeCapture">
		<div class="panel">
			<h3>设备验证</h3>
			<el-form>
				<el-form-item>
					<el-input v-focus v-model="userCode" placeholder="请输入您设备上的代码" />
				</el-form-item>
				<el-form-item>
					<el-button type="primary" @click="submit">提交</el-button>
				</el-form-item>
			</el-form>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			userCode: '',
		}
	},
	methods: {
		submit() {
			this.$router.push({
				path: '/userCodeConfirmation',
				query: {
					userCode: this.userCode,
				},
			})
		},
	},
}
</script>
<style scoped>
#userCodeCapture {
	display: flex;
	justify-content: center;
	align-items: center;
	height: 100vh;
}

.panel {
	width: 300px;
}

.el-button {
	width: 300px;
}
</style>
