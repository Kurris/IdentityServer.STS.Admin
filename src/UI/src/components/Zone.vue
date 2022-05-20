<template>
	<div id="zone" v-if="show">
		<template v-if="user == null"> 404 USER </template>
		<template v-else>
			<template v-if="isCurrentUser">
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
				</div>
			</template>
			<template v-else> 介绍界面 </template>
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
import { getUserByName } from './../net/api.js'

export default {
	data() {
		return {
			show: false,
			status: null,
			user: null,
			userName: this.$route.params.userName,
			isCurrentUser: this.$route.params.userName != undefined,
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
		go2fa() {
			this.$router.push('/twoFactorAuthentication')
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
	},
}
</script>
<style scoped>
.el-card {
	width: auto;
}

.container {
	display: flex;
	justify-content: space-around;
	padding: 50px 50px 0 50px;
}
</style>
