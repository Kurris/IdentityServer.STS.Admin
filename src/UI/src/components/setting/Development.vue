<template>
	<div id="development">
		<div>
			<div class="flex">
				<b style="font-size: 30px">开发者设置 (waiting)</b>
				<template>
					<!-- <el-button v-if="!setting.isDeveloper" type="success" @click="enable2Fa(true)">申请</el-button> -->
				</template>
			</div>
			<el-divider></el-divider>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			setting: {},
		}
	},
}
</script>
<style scoped>
.flex {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
</style>
