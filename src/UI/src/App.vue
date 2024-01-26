<template>
	<div id="app" v-if="status != null">

		<router-view />

		<div class="home" v-if="$route.path == '/'">认证中心首页</div>
		<!-- <div class="icp"
			v-if="$route.path == '/' || $route.path == '/signIn' || $route.path == '/register' || $route.path == '/forgotPassword' || $route.path == '/resetPassword'">
			<CookieTip v-if="cookieTipShow" @confirm="confirm" />
			<span>
				©️Copyright 2022 - {{ new Date().getFullYear().toString() == 2022 ? '至今' : new
					Date().getFullYear().toString() }}
				<el-link style="font-size: 12px; bottom: 1.5px" type="info" :underline="false"
					@click="goBeian()">粤ICP备2022078329号</el-link>
			</span>

			<el-link type="info" href="mailto:ligy.97@foxmail.com?subject=identityserver问题&body=我遇到问题"
				:underline="false">联系我们</el-link>
		</div> -->
	</div>
</template>

<script>
//
import { logout, getLoginStatus } from './net/api.js'

export default {
	name: 'app',
	components: {},
	data() {
		return {
			status: null,
			cookieTipShow: false,
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
		confirm() {
			this.cookieTipShow = false
			window.localStorage.setItem('cookieTipShow', '0')
		},
		getShow() {
			let s = window.localStorage.getItem('cookieTipShow')

			if (s == null || s == '') {
				return true
			}

			return s == '1'
		},
		goBeian() {
			window.open('https://beian.miit.gov.cn', '_blank')
		},
	},
	watch: {
		$route() {
			getLoginStatus().then(res => {
				this.status = res.data
			})
		},
	},
	async beforeMount() {
		this.cookieTipShow = this.getShow()

		let res = await getLoginStatus()
		this.status = res.data
	},
}
</script>

<style>
body {
	margin: 0;
}

#app {
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
	height: 7vh;
	color: white;
	background-color: #25292e;
	border-bottom: 0.5px;
}

.headerLeft {
	display: flex;
	justify-content: center;
	align-items: center;
}

.headerRight {
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

.icp {
	position: fixed;
	display: flex;
	justify-content: space-around;
	align-items: center;
	width: 100%;
	/* box-shadow: 0 1px 8px 2px #d5d5d5; */
	bottom: 0;
	margin: 0;
	padding: 0;
	min-height: 7vh;
	font-size: 12px;
	color: #909399;
	background-color: #303d4d;
}

.home {
	display: flex;
	justify-content: center;
	align-items: center;
}
</style>
