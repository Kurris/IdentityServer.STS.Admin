<template>
	<div class="consent" v-if="model != null">
		<div class="logo">
			<el-avatar :src="model.clientLogoUrl" :size="64"></el-avatar>
			<div v-for="n in 7" :key="'left' + n" class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
			<i class="el-icon-success" style="font-size: 32px"></i>
			<div v-for="n in 7" :key="'right' + n" class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
			<AppAvatar />
		</div>

		<div class="title">
			<h3>
				<p>{{ model.clientName }}</p>
				<span>正在请求您的访问权限</span>
			</h3>
			<template v-if="model.confirmUserCode">
				<div>设备代码:{{ model.userCode }}</div>
			</template>
		</div>
		<div class="panel">
			<div class="notice">
				<AuthorizeItem :title="model.clientName" description="希望获取额外的访问权限">
					<template slot="img">
						<el-avatar :src="model.clientLogoUrl"></el-avatar>
					</template>
					<span v-if="model.clientOwner">
						来自于 <el-link type="primary">{{ model.clientOwner.userName }}</el-link>
					</span>
				</AuthorizeItem>

				<AuthorizeItem divider title="用户私人的数据" description="个人信息(只读)和应用资源权限(读写)">
					<template slot="img">
						<i class="el-icon-user" style="color: #555b65; font-size: 32px; padding: 4px"></i>
					</template>
					<template slot="container">
						<div v-if="dropdownStatus">
							<template v-if="model.identityScopes">
								<template v-for="item in model.identityScopes">
									<div :key="item.displayName">
										<ScopeItem :scope="item" />
									</div>
								</template>
							</template>
							<template v-if="model.apiScopes">
								<template v-for="item in model.apiScopes">
									<div :key="item.displayName">
										<ScopeItem :scope="item" />
									</div>
								</template>
							</template>
							<el-link>
								<i class="el-icon-question" style="font-size: 12px">了解更多</i>
							</el-link>
						</div>
					</template>
					<div slot="operation" style="cursor: pointer">
						<div @click="dropdownStatus = !dropdownStatus">
							<template v-if="dropdownStatus">
								<i class="el-icon-arrow-up" style="font-size: 15px; font-weight: bold"></i>
							</template>
							<template v-else>
								<i class="el-icon-arrow-down" style="font-size: 15px; font-weight: bold"></i>
							</template>
						</div>
					</div>
				</AuthorizeItem>

				<div class="extension">
					<div>
						<span style="font-weight: bold; font-size: 14px"> 存在的许可 </span>
						<div style="font-size: 10px; color: #91969b; margin-top: 2px">
							<i class="el-icon-success"></i>
							将会更新用户信息
						</div>
					</div>
					<div v-if="model.allowRememberConsent" style="font-size: 10px; color: #91969b; margin-top: 2px">
						<el-checkbox v-model="model.rememberConsent"> 允许记住授权操作</el-checkbox>
					</div>
				</div>
			</div>
			<div style="text-align: center; margin-bottom: 20px">
				<el-divider></el-divider>
				<div style="margin-top: 40px">
					<el-button type="warning" plain @click="process(false)">不许可</el-button>
					<el-button type="primary" @click="process(true)">许可</el-button>
				</div>
				<div style="font-size: 10px; margin-top: 20px">
					<template v-if="model.clientUrl && model.clientUrl != null">
						<span style="color: #91969b"> 许可授权将会重定向到 </span>
						<div style="margin-top: 10px">
							<strong>
								<el-link type="primary" :underline="false" :href="model.clientUrl">{{ model.clientUrl }} </el-link>
							</strong>
						</div>
					</template>
				</div>
			</div>
		</div>

		<div class="footer">
			<div>
				<i class="el-icon-location-outline" style="font-weight: bold; font-size: 14px"></i>
				拥有者
				<div style="font-weight: bold; margin-left: 18px">来自外部授权应用</div>
			</div>
			<div>
				<i class="el-icon-time" style="font-weight: bold; font-size: 14px"></i>
				过期
				<div style="font-weight: bold; margin-left: 18px">5分钟后</div>
			</div>
		</div>

		<div class="other">
			<el-link href="https://oauth.net/2/" :underline="false" icon="el-icon-question" target="_blank"> 了解更多关于OAuth2.0</el-link>
		</div>
	</div>
</template>

<script>
import { getUserGetUserCodeConfirmationModel } from '../net/api.js'
import ScopeItem from './ScopeItem.vue'
import AuthorizeItem from './AuthorizeItem.vue'
import NProgress from 'nprogress'
import AppAvatar from './AppAvatar.vue'

export default {
	components: {
		ScopeItem,
		AuthorizeItem,
		AppAvatar,
	},
	data() {
		return {
			dropdownStatus: false,
			model: null,
		}
	},
	methods: {
		async process(allow) {
			let idScopes = this.model.identityScopes.filter(x => x.checked)
			let apiScopes = this.model.apiScopes.filter(x => x.checked)

			NProgress.start()
			let url = 'http://localhost:5000/api/device'

			document.write('<form action=' + url + " method=post name=form1 style='display:none'>")
			document.write(`<input type=hidden name=allow value=${allow}></input>`)
			document.write(`<input type=hidden name=rememberConsent value=${this.model.rememberConsent}></input>`)
			document.write(`<input type=hidden name=userCode value=${this.$route.query.userCode}></input>`)

			let scopeNames = idScopes.map(x => x.value)
			for (let i = 0; i < scopeNames.length; i++) {
				const element = scopeNames[i]
				document.write("<input type=hidden name=scopesConsented value='" + element + "'/>")
			}

			let apiScopeNames = apiScopes.map(x => x.value)
			for (let i = 0; i < apiScopeNames.length; i++) {
				const element = apiScopeNames[i]
				document.write(`<input type=hidden name=scopesConsented value=${element}></input>`)
			}
			document.write('</form>')
			document.form1.submit()
		},
	},
	async beforeMount() {
		try {
			let res = await getUserGetUserCodeConfirmationModel({
				userCode: this.$route.query.userCode,
			})
			if (res != null) {
				this.model = res.data
			} else {
				this.model = null
			}
		} catch (error) {
			this.$router.push({
				path: '/error',
				query: {
					error: error,
				},
			})
		}
	},
}
</script>
<style scoped>
.consent {
	display: flex;
	flex-direction: column;
	align-items: center;
}

.panel {
	/* width: 600px; */
	width: 368px;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	border-radius: 7px;
}

.notice {
	padding: 30px;
}

.footer {
	display: flex;
	justify-content: space-around;
	align-items: center;
	/* width: 600px; */
	width: 368px;
	height: 65px;
	box-shadow: 1px 2px 4px 3px #d9d9d9;
	border-radius: 7px;
	margin-top: 20px;
	font-size: 12px;
	color: #92969a;
}

.logo {
	text-align: center;
	margin-top: 20px;
}

.other {
	font-size: 12px;
	color: #92969a;
	text-align: center;
	margin-top: 20px;
}

.el-button {
	width: 40%;
}

.extension {
	display: flex;
	justify-content: space-around;
	align-items: center;
}

.el-collapse {
	border-top: none;
	border-bottom: none;
}

>>> .el-collapse-item__header {
	border-bottom: none;
}

>>> .el-collapse-item__wrap {
	border-bottom: none;
}

.el-divider--horizontal {
	margin: 0;
	margin-bottom: 15px;
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

.title {
	text-align: center;
	margin-top: 10px;
	margin-bottom: 10px;
}
</style>
