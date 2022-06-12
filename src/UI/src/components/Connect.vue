<template>
	<div class="connect">
		<div class="logo">
			<img src="../assets/auth2logo/GitHub-Mark-64px.png" :size="70" />
			<div v-for="n in 7" :key="'left' + n" class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
			<i class="el-icon-success" style="font-size: 32px"></i>
			<div v-for="n in 7" :key="'right' + n" class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
			<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="64"></el-avatar>
		</div>
	</div>
</template>

<script>
export default {}
</script>
<style scoped>
.connect {
	display: flex;
	justify-content: center;
}
.el-icon-success {
	color: #53a258;
}

.logo {
	display: flex;
	justify-content: center;
	align-items: center;
}

div.dashed + div.dashed {
	margin-left: 1px;
}
</style>
