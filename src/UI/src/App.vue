<template>
	<div id="app">
		<!--  -->
		<div
			id="header"
			v-if="
				status != null &&
				$route.path != '/signIn' &&
				$route.path != '/signinWith2fa' &&
				$route.path != '/consent' &&
				$route.path != '/loginWithRecoveryCode' &&
				$route.path != '/externalLoginConfirmation'
			"
		>
			<div class="left">
				<a href="http://docs.identityserver.io/en/latest/" title="跳转到identityserver4 document">
					<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="32" />
				</a>
				<el-link style="color: white; margin-left: 10px" @click="$router.push('/')">首页</el-link>
				<el-link style="color: white; margin-left: 10px" href="https://github.com/Kurris/IdentityServer.STS.Admin">Github</el-link>
				<el-link style="color: white; margin-left: 10px" @click="getDocument()">发现文档</el-link>
			</div>
			<div class="right">
				<template v-if="status.isLogin">
					<el-badge is-dot><i class="el-icon-bell bell"></i></el-badge>
					<el-dropdown trigger="click" @command="handleCommand" style="margin-left: 30px">
						<span class="el-dropdown-link">
							<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="20" />
							<i class="el-icon-caret-bottom" style="color: white"></i>
						</span>
						<el-dropdown-menu slot="dropdown">
							<el-dropdown-item>
								登录为: <strong>{{ status.user.userName }}</strong>
							</el-dropdown-item>
							<el-dropdown-item divided>
								<button style="width: 150px; height: 30px; background-color: white; border-radius: 4px; border: 1px solid #eceef4">
									<i class="el-icon-star-off">状态</i>
								</button>
							</el-dropdown-item>
							<el-dropdown-item divided command="profile">个人概要</el-dropdown-item>
							<el-dropdown-item command="setting">设置</el-dropdown-item>
							<el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
						</el-dropdown-menu>
					</el-dropdown>
				</template>
				<template v-else>
					<el-link style="color: white" @click="$router.push('/signIn')">登录</el-link>
					<el-link style="color: white; margin-left: 20px">注册</el-link>
				</template>
			</div>
		</div>

		<router-view />
	</div>
</template>

<script>
//
import { logout, getLoginStatus } from './net/api.js'

export default {
	name: 'app',
	data() {
		return {
			status: null,
		}
	},
	methods: {
		handleCommand(cmd) {
			if (cmd == 'logout') {
				this.logout()
			} else if (cmd == 'setting') {
				this.$router.push({
					path: '/setting',
				})
			} else if (cmd == 'profile') {
				this.$router.push({
					path: '/zone/' + this.status.user.userName,
				})
			}
		},
		getDocument() {
			this.$router.push('/discoveryDocument')
		},
		async logout() {
			var res = await logout()
			if (res.route == 9) {
				this.$router.push({
					path: '/logout',
					query: res.data,
				})
			}
		},
	},
	async beforeMount() {
		let res = await getLoginStatus()
		if (res != null) {
			this.status = res.data
		}
	},
}
</script>

<style>
body {
	margin: 0;
}

#app {
	height: 100%;
	width: 100%;
	overflow-x: hidden;
	overflow-y: hidden;
	margin: 0;
	padding: 0;
}

#header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 0 50px 0 50px;
	height: 60px;
	color: white;
	background-color: #25292e;
	border-bottom: 0.5px;
}

.left {
	display: flex;
	justify-content: center;
	align-items: center;
}

.right {
	display: flex;
	justify-content: center;
	align-items: center;
}

.el-dropdown-link {
	cursor: pointer;
}

.bell:hover {
	cursor: pointer;
}
</style>
