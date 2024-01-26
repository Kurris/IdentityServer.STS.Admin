<template>
	<div id="zone" v-if="show">

		<div id="header">
			<div class="headerLeft">
				<!-- <a href="http://docs.identityserver.io/en/latest/" title="跳转到identityserver4 document">
					<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="32" />
				</a> -->
				<el-link style="color: white; margin-left: 10px" @click="$router.push('/')">首页</el-link>
				<el-link style="color: white; margin-left: 10px"
					href="https://github.com/Kurris/IdentityServer.STS.Admin">Github</el-link>
				<el-link style="color: white; margin-left: 10px" @click="getDocument()">发现文档</el-link>
			</div>
			<div class="headerRight">
				<template v-if="status.isLogin">
					<el-dropdown trigger="click" @command="handleCommand" style="margin-left: 30px">
						<span class="el-dropdown-link">
							<!-- <el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="20" /> -->
							<i class="el-icon-caret-bottom" style="color: white"></i>
						</span>
						<el-dropdown-menu slot="dropdown">
							<el-dropdown-item>
								登录为: <strong>{{ status.user.userName }}</strong>
							</el-dropdown-item>
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

		<template v-if="user == null"> 404 USER </template>
		<template v-else>
			<template v-if="isCurrentUser">
				<div class="container">
					<!-- <h1>{{ user.userName }}的个人空间</h1> -->
				</div>
			</template>
		</template>

		<!-- <el-row justify="space-around" :gutter="30">
			<el-col :span="colSpan" v-show="isAuthenticated">
				<el-card class="box-card">
					<div slot="header" class="clearfix">
						<span><b>持久授权</b></span>
					</div>
					<el-button type="primary" @click="$router.push('/grants')">持久授权</el-button>
				</el-card>
			</el-col>
			<el-col :span="colSpan">
				<el-card class="box-card">
					<div slot="header" class="clearfix">
						<span><b>我的个人资料</b></span>
					</div>
					<el-button type="primary" @click="$router.push('/profile')">我的个人资料</el-button>
				</el-card>
			</el-col>

			<el-col :span="colSpan">
				<el-card class="box-card">
					<div slot="header" class="clearfix">
						<span><b>我的个人数据</b></span>
					</div>
					<el-button type="primary" @click="$router.push('/personalData')">我的个人数据</el-button>
				</el-card>
			</el-col>

			<el-col :span="colSpan">
				<el-card class="box-card">
					<div slot="header" class="clearfix">
						<span><b>更改密码</b></span>
					</div>
					<el-button type="primary" @click="$router.push('/password')">更改密码</el-button>
				</el-card>
			</el-col>

			<el-col :span="colSpan">
				<el-card class="box-card">
					<div slot="header" class="clearfix">
						<span><b>外部登录</b></span>
					</div>
					<el-button type="primary" @click="$router.push('/externalLogins')">外部登录</el-button>
				</el-card>
			</el-col>
		</el-row> -->
	</div>
</template>

<script>
import { getUserByName, getLoginStatus, logout } from './../net/api.js'

export default {
	data() {
		return {
			show: false,
			status: {},
			user: null,
			userName: this.$route.params.userName,
			isCurrentUser: this.$route.params.userName != undefined,
		}
	},
	components: {},
	methods: {
		getDocument() {
			this.$router.push('/discoveryDocument')
		},
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
		if (this.isCurrentUser) {
			let userRes = await getUserByName({
				userName: this.userName,
			})

			this.user = userRes.data
		}

		this.show = true

		getLoginStatus().then(res => {
			this.status = res.data
		})
	},
}
</script>
<style scoped>
.container {
	display: flex;
	justify-content: center;
	align-items: center;
	height: 80vh;
}
</style>
