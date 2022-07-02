<template>
	<div id="account">
		<div>
			<h2>下载个人数据(Download account)</h2>
			<el-divider></el-divider>
			<p>下载您的个人数据</p>
			<el-button @click="downloadData">开始下载</el-button>
		</div>

		<div>
			<h2 style="color: red">删除您的账号(Delete account)</h2>
			<el-divider></el-divider>
			<p>一旦删除您的账户,这将不可恢复,请确保您当前的操作</p>
			<el-button type="danger" plain @click="deleteData">删除您的账户</el-button>
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
