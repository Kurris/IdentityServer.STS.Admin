<template>
	<div id="zone" v-if="show">
		<template v-if="user == null"> 404 USER </template>
		<template v-else>
			<template v-if="isCurrentUser">
				<div class="container">
					<h1>{{ user.userName }}的个人空间</h1>
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
import { getUserByName } from './../net/api.js'

export default {
	data() {
		return {
			show: false,
			status: null,
			user: null,
			userName: this.$route.params.userName,
			isCurrentUser: this.$route.params.userName != undefined,
		}
	},
	components: {},
	methods: {},
	async beforeMount() {
		if (this.isCurrentUser) {
			let userRes = await getUserByName({
				userName: this.userName,
			})

			this.user = userRes.data
		}

		this.show = true
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
