<template>
	<div class="consent">
		<div v-if="setting != null">
			<div class="logo">
				<el-avatar :src="setting.ClientLogoUrl" :size="70"></el-avatar>
				<div v-for="n in 7" :key="'left' + n" class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<i class="el-icon-success" style="font-size: 32px"></i>
				<div v-for="n in 7" :key="'right' + n" class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="70"></el-avatar>
				<!-- </template> -->
			</div>

			<div class="title">
				<h2>{{ setting.clientName }} 正在请求额外的权限</h2>
			</div>

			<div class="panel">
				<div class="notice">
					<AuthorizeItem :title="setting.clientName" description="希望获取额外的访问权限">
						<template slot="img">
							<el-avatar src=""></el-avatar>
						</template>
						<span>来自于 <el-link type="primary">Kurris</el-link> </span>
					</AuthorizeItem>

					<AuthorizeItem title="用户私人的数据" :isDropdown="true" :scopeLength="scopeLength" description="个人的信息(只读)或者其他应用级别的权限(读写)">
						<template slot="img">
							<i class="el-icon-user" style="color: #555b65; font-size: 40px"></i>
						</template>
						<template slot="dropdown">
							<div class="userData">
								<template v-if="setting.identityScopes">
									<template v-for="item in setting.identityScopes">
										<div :key="item.displayName">
											<ScopeItem :scope="item" />
										</div>
									</template>
								</template>
								<template v-if="setting.apiScopes">
									<template v-for="item in setting.apiScopes">
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
					</AuthorizeItem>

					<el-divider></el-divider>
					<div class="extension">
						<div>
							<span style="font-weight: bold; font-size: 14px"> 存在的许可 </span>
							<div style="font-size: 10px; color: #91969b; margin-top: 2px">
								<i class="el-icon-success"></i>
								更新用户所有数据
							</div>
						</div>
						<div>
							<div style="font-size: 10px; color: #91969b; margin-top: 2px">
								<el-checkbox v-model="setting.rememberConsent"> 允许记住授权操作</el-checkbox>
							</div>
						</div>
					</div>
				</div>
				<div class="btn">
					<div style="text-align: center; margin-bottom: 30px">
						<el-divider></el-divider>
						<div style="margin-top: 40px">
							<el-button class="nohover" size="small" @click="process(false)">不许可</el-button>
							<el-button class="green" size="small" autofocus @click="process(true)">许可</el-button>
						</div>

						<div style="font-size: 10px; margin-top: 30px">
							<template v-if="setting.clientUrl != null">
								<span style="color: #91969b"> 许可授权将会重定向到 </span>
								<div style="margin-top: 10px">
									<strong>
										{{ setting.clientUrl }}
									</strong>
								</div>
							</template>
						</div>
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
				<el-link href="https://oauth.net/2/" :underline="false" icon="el-icon-question" target="_blank">了解更多关于OAuth2.0</el-link>
			</div>
		</div>
	</div>
</template>

<script>
import { getConsentSetting } from '../net/api.js'
import ScopeItem from './ScopeItem.vue'
import AuthorizeItem from './AuthorizeItem.vue'
import NProgress from 'nprogress'

export default {
	components: {
		ScopeItem,
		AuthorizeItem,
	},
	data() {
		return {
			setting: null,
		}
	},
	methods: {
		process(allow) {
			let idScopes = this.setting.identityScopes.filter(x => x.checked)
			let apiScopes = this.setting.apiScopes.filter(x => x.checked)

			NProgress.start()
			let url = 'http://101.35.47.169:5000/api/consent/setting/process'

			document.write(`<form action=${url}  method=post name=form1 style='display:none'>`)
			document.write(`<input type=hidden name=allow value=${allow}></input>`)
			document.write(`<input type=hidden name=rememberConsent value=${this.setting.rememberConsent}></input>`)
			document.write(`<input type=hidden name=returnUrl value=${this.setting.returnUrl}></input>`)

			let scopeNames = idScopes.map(x => x.value)
			for (let i = 0; i < scopeNames.length; i++) {
				const element = scopeNames[i]
				document.write(`<input type=hidden name=scopesConsented value=${element}></input>`)
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
	computed: {
		scopeLength() {
			if (this.setting.identityScopes && this.setting.apiScopes) {
				let res = this.setting.identityScopes.length + this.setting.apiScopes.length
				return res
			}
			return 0
		},
	},
	async beforeMount() {
		let returnUrl = this.$url.getValueFromQuery('returnUrl')
		let response = await getConsentSetting({ returnUrl })
		this.setting = response.data
	},
}
</script>
<style scoped>
.consent {
	display: flex;
	height: 100vh;
	justify-content: center;
	background: #f6f8fa;
}

.panel {
	width: 500px;
	border: 1px solid #d0d7dd;
	border-radius: 7px;
	background-color: white;
}

.footer {
	display: flex;
	justify-content: space-around;
	align-items: center;
	width: 500px;
	height: 65px;
	border: 1px solid #d0d7dd;
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
	width: 220px;
}

.notice {
	padding: 30px;
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
	margin-top: 20px;
	margin-bottom: 20px;
}

.userData {
	max-width: 420px;
}

.el-button.green {
	background-color: #53a258;
	color: white;
}

.el-button.nohover:hover {
	border-color: #dddfe5;
	background-color: unset;
	color: unset;
}
</style>
