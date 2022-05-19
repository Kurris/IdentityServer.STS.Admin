<template>
	<div id="zone">
		<div id="header">
			<div class="left">
				<a href="http://docs.identityserver.io/en/latest/" title="跳转到identityserver4 document">
					<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="32" />
				</a>

				<el-link style="color: white; margin-left: 10px" href="https://github.com/Kurris/IdentityServer.STS.Admin">Github</el-link>
				<el-link style="color: white; margin-left: 10px" @click="getDocument()">发现文档</el-link>
			</div>
			<div class="right">
				<el-badge is-dot><i class="el-icon-bell bell"></i></el-badge>
				<el-dropdown trigger="click" @command="handleCommand" style="margin-left: 30px">
					<span class="el-dropdown-link">
						<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="20" />
						<i class="el-icon-caret-bottom" style="color: white"></i>
					</span>
					<el-dropdown-menu slot="dropdown">
						<el-dropdown-item>
							登录为: <strong v-if="status != null">{{ status.user.userName }}</strong>
						</el-dropdown-item>
						<el-dropdown-item divided>
							<button style="width: 150px; height: 30px; background-color: white; border-radius: 4px; border: 1px solid #eceef4">
								<i class="el-icon-star-off">状态</i>
							</button>
						</el-dropdown-item>
						<el-dropdown-item divided>个人概要</el-dropdown-item>
						<el-dropdown-item>设置</el-dropdown-item>
						<el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
					</el-dropdown-menu>
				</el-dropdown>
			</div>
		</div>
		<div class="container">
			<el-card style="width: 500px; text-align: center" shadow="never">
				<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="250" />
			</el-card>
			<el-card style="width: 800px" shadow="never">
				<el-tabs v-model="activeName">
					<el-tab-pane label="更新内容" name="first">
						<el-timeline>
							<el-timeline-item v-for="(activity, index) in activities" :key="index" :timestamp="activity.timestamp">
								{{ activity.content }}
							</el-timeline-item>
						</el-timeline>
					</el-tab-pane>
				</el-tabs>
			</el-card>
			<router-view />
		</div>
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
						<span><b>双因素身份验证</b></span>
					</div>
					<el-button type="primary" @click="go2fa">双因素身份验证</el-button>
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
import { getLoginStatus, logout } from '../net/api.js'

export default {
	data() {
		return {
			colSpan: this.isAuthenticated ? 8 : 12,
			status: null,
			activeName: 'first',
			activities: [
				{
					content: '活动按期开始',
					timestamp: '2018-04-15',
				},
				{
					content: '通过审核',
					timestamp: '2018-04-13',
				},
				{
					content: '创建成功',
					timestamp: '2018-04-11',
				},
			],
		}
	},
	components: {},
	methods: {
		getDocument() {
			this.$router.push('/discoveryDocument')
		},
		go2fa() {
			this.$router.push('/twoFactorAuthentication')
		},
		handleCommand(cmd) {
			if (cmd == 'logout') {
				this.logout()
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
		let res = await getLoginStatus()
		if (res != null) {
			if (res.data.code == 401) {
				// this.$router.replace('/signIn')
			} else if (res.code == 200) {
				this.status = res.data
				console.log(this.status)
			}
		}
	},
}
</script>
<style scoped>
.el-card {
	width: auto;
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

.container {
	display: flex;
	justify-content: space-around;
	padding: 50px 50px 0 50px;
}

.bell:hover {
	cursor: pointer;
}
</style>
