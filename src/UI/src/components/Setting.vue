<template>
	<div id="setting">
		<el-menu :default-active="activeRoute" router text-color="black">
			<el-menu-item index="/setting/profile">
				<i class="el-icon-user"></i>
				<span slot="title">Public Profile</span>
			</el-menu-item>
			<el-menu-item index="/setting/account">
				<i class="el-icon-setting"></i>
				<span slot="title">Account</span>
			</el-menu-item>
			<el-menu-item-group>
				<template slot="title">
					<el-divider></el-divider>
					<span>访问</span>
				</template>
				<el-menu-item index="/setting/security">
					<i class="el-icon-key"></i>
					<span slot="title">Password and Authentication</span>
				</el-menu-item>
				<el-menu-item index="/setting/externalLogins">
					<i class="el-icon-connection"></i>
					<span slot="title">External Logins</span>
				</el-menu-item>
			</el-menu-item-group>
			<el-menu-item-group>
				<template slot="title">
					<el-divider></el-divider>
					<span>集成</span>
				</template>
				<el-menu-item index="/setting/grants">
					<i class="el-icon-mobile-phone"></i>
					<span slot="title">Granted OAuth Apps</span>
				</el-menu-item>
			</el-menu-item-group>
			<el-menu-item index="/setting/development">
				<i class="el-icon-s-operation"></i>
				<span slot="title">Development Setting</span>
			</el-menu-item>
		</el-menu>
		<div class="view">
			<router-view />
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			activeRoute: '/setting/profile',
		}
	},
	beforeMount() {
		this.activeRoute = this.$route.path
	},
}
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

.el-menu-item {
	border-radius: 5px;
	height: 40px;
	line-height: 40px;
}

.el-menu-item.is-active {
	border-left: 4px solid #005cc5;
	background-color: #eef5fe;
}

.el-divider {
	margin: 0;
	margin-bottom: 10px;
}
</style>
