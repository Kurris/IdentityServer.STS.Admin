<template>
	<div id="account">
		<h1>账号信息(account)</h1>
		<el-divider></el-divider>
		您的帐户包含您提供给我们的个人数据,允许您下载或删除该数据
		<el-button @click="downloadData">下载个人数据</el-button>
		<div>
			删除此数据会永久删除您的帐户，并无法恢复。
			<el-button @click="deleteData">删除个人数据</el-button>
		</div>
	</div>
</template>

<script>
import { downloadProfile, deleteProfile } from '../../net/api.js'

export default {
	components: {},
	data() {
		return {}
	},
	methods: {
		async deleteData() {
			await deleteProfile({
				password: '',
			})
		},
		async downloadData() {
			await downloadProfile()
		},
	},
}
</script>
<style scoped></style>
