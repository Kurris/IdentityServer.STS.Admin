<template>
	<div id="personalData">
		<div class="row">
			<div class="col-md-6">
				您的帐户包含您提供给我们的个人数据。 此页面允许您下载或删除该数据
				<el-button @click="downloadData">下载个人数据</el-button>
				<div>
					删除此数据会永久删除您的帐户，并无法恢复。
					<a asp-action="" class="btn btn-primary"></a>
					<el-button @click="deleteData">删除个人数据</el-button>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
import { downloadProfile, deleteProfile } from '../net/api.js'

export default {
	components: {},
	data() {
		return {}
	},
	computed: {},
	watch: {},
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
	//生命周期 - 创建完成（可以访问当前this实例）
	created() {},
	//生命周期 - 挂载完成（可以访问DOM元素）
	mounted() {},
	beforeCreate() {}, //生命周期 - 创建之前
	beforeMount() {}, //生命周期 - 挂载之前
	beforeUpdate() {}, //生命周期 - 更新之前
	updated() {}, //生命周期 - 更新之后
	beforeDestroy() {}, //生命周期 - 销毁之前
	destroyed() {}, //生命周期 - 销毁完成
	activated() {}, //如果页面有keep-alive缓存功能，这个函数会触发
}
</script>
<style scoped></style>
