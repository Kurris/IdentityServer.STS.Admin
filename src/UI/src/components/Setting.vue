<template>
	<div id="setting">
		<el-menu default-active="2">
			<el-menu-item index="4">
				<i class="el-icon-setting"></i>
				<span slot="title">账号</span>
			</el-menu-item>
			<el-menu-item index="4">
				<i class="el-icon-setting"></i>
				<span slot="title">个人概要</span>
			</el-menu-item>
			<el-menu-item index="4">
				<i class="el-icon-setting"></i>
				<span slot="title">通知</span>
			</el-menu-item>
			<el-menu-item-group>
				<template slot="title">访问</template>
				<el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">邮件</span>
				</el-menu-item>
				<el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">密码和授权</span>
				</el-menu-item>
				<el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">机构</span>
				</el-menu-item>
				<el-menu-item index="4">
					<i class="el-icon-setting"></i>
					<span slot="title">黑名单</span>
				</el-menu-item>
			</el-menu-item-group>
			<el-menu-item index="4">
				<i class="el-icon-setting"></i>
				<span slot="title">开发者设置</span>
			</el-menu-item>
		</el-menu>
		<router-view></router-view>
	</div>
</template>

<script>
// import { getLoginStatus, logout } from '../net/api.js'

export default {
	data() {
		return {}
	},
	components: {},
	methods: {},
	async beforeMount() {},
}
</script>
<style scoped></style>
