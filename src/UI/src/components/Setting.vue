<template>
	<div id="setting">
		<el-menu default-active="/setting/profile" router active-text-color="#24292f" text-color="#afafbe">
			<!-- <el-menu-item index="4">
				<i class="el-icon-setting"></i>
				<span slot="title">账号</span>
			</el-menu-item> -->
			<el-menu-item index="/setting/profile">
				<i class="el-icon-setting"></i>
				<span slot="title">个人概要</span>
			</el-menu-item>
			<!-- <el-menu-item index="4">
				<i class="el-icon-setting"></i>
				<span slot="title">通知</span>
			</el-menu-item> -->
			<el-menu-item-group>
				<template slot="title">访问</template>
				<!-- <el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">邮件</span>
				</el-menu-item> -->
				<el-menu-item index="/setting/security">
					<i class="el-icon-setting"></i>
					<span slot="title">Password and authentication</span>
				</el-menu-item>
				<!-- <el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">机构</span>
				</el-menu-item>
				<el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">黑名单</span>
				</el-menu-item> -->
			</el-menu-item-group>
			<el-menu-item index="/setting/development">
				<i class="el-icon-setting"></i>
				<span slot="title">Development Setting</span>
			</el-menu-item>
		</el-menu>
		<div class="view">
			<router-view></router-view>
		</div>
	</div>
</template>

<script>
export default {}
</script>
<style scoped>
#setting {
	display: flex;
	padding-left: 10%;
	padding: 2% 10%;
}
.view {
	padding-left: 8%;
	width: 50%;
}

.el-menu {
	border-right: unset;
}
</style>
