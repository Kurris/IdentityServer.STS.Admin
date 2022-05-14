<template>
	<div class="consent">
		<div>
			<div class="logo">
				<!-- <template v-if="setting.clientLogoUrl != null"> -->
				<el-avatar :src="setting.ClientLogoUrl" :size="70"></el-avatar>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<i class="el-icon-success" style="font-size: 32px"></i>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<div class="dashed" style="height: 2px; width: 4px; background-color: #ced5db"></div>
				<el-avatar :src="setting.ClientLogoUrl" :size="70"></el-avatar>
				<!-- </template> -->
			</div>

			<div class="title">
				<h2>{{ setting.clientName }}正在请求额外的权限</h2>
			</div>

			<div class="panel">
				<div class="notice">
					<AuthorizeItem :title="setting.clientName" description="个人的信息(只读)或者其他应用级别的权限(读写)">
						<template slot="img">
							<el-avatar></el-avatar>
						</template>
						<span>来自于 <el-link type="primary">Kurris</el-link> </span>
					</AuthorizeItem>

					<AuthorizeItem :title="setting.clientName" description="个人的信息(只读)或者其他应用级别的权限(读写)">
						<template slot="img">
							<i class="el-icon-user" style="font-size: 40px"></i>
						</template>
						<span>来自于 <el-link type="primary">Kurris</el-link> </span>
					</AuthorizeItem>

					<div class="userData">
						<template slot="title">
							<i class="el-icon-user" style="font-size: 32px"></i>
							<div>个人的用户数据</div>
						</template>
						<template v-if="setting.identityScopes != null && setting.identityScopes.length > 0">
							<div class="col-sm-12">
								<div>
									<h5>
										<i>个人信息</i>
									</h5>
									<div class="card-body">
										<ul class="list-group">
											<template v-for="(item, index) in setting.identityScopes">
												<div :key="index">
													<ScopeItem :scope="item" />
												</div>
											</template>
										</ul>
									</div>
								</div>
							</div>
						</template>
						<template v-if="setting.ApiScopes != null && setting.ApiScopes.length > 0">
							<div class="col-sm-12">
								<div class="card mt-3">
									<h5 class="card-header">
										<i class="fa fa-lock"></i>
										应用访问
									</h5>
									<div class="card-body">
										<ul class="list-group">
											<template v-for="(item, index) in setting.ApiScopes">
												<div :key="index">
													<ScopeItem :scope="item" />
												</div>
											</template>
										</ul>
									</div>
								</div>
							</div>
						</template>
						<i class="el-icon-question"></i>
						了解更多
					</div>
					<el-divider></el-divider>
					<div>
						<span style="font-weight: bold; font-size: 14px"> 存在的许可 </span>
						<div style="font-size: 10px; color: #91969b">
							<strong>
								<i class="el-icon-success"></i>
							</strong>
							更新用户所有数据
						</div>
					</div>
				</div>
				<div class="btn">
					<div style="text-align: center; margin-bottom: 30px">
						<el-divider></el-divider>
						<!-- <template v-if="setting.allowRememberConsent">
							<el-switch v-model="setting.rememberConsent" active-color="#13ce66" inactive-color="#ff4949"> </el-switch>
							<strong>记住</strong>
						</template> -->
						<div style="margin-bottom: 10px">
							<el-button size="small" @click="process('no')">不许可</el-button>
							<el-button type="success" size="small" autofocus @click="process('yes')">许可</el-button>
						</div>

						<div style="font-size: 10px">
							<template v-if="setting.clientUrl != null">
								<span style="color: #91969b"> 许可将会重定向到 </span>

								<div>
									<strong>
										{{ setting.clientUrl }}
									</strong>
								</div>
							</template>
						</div>
					</div>
				</div>
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
			setting: {},
		}
	},
	methods: {
		process(btn) {
			let idScopes = this.setting.identityScopes.filter(x => x.checked)

			NProgress.start()
			let url = 'http://localhost:5000/api/consent/setting/process'

			document.write('<form action=' + url + " method=post name=form1 style='display:none'>")
			document.write("<input type=hidden name=button value='" + btn + "'/>")
			document.write("<input type=hidden name=rememberConsent value='" + this.setting.rememberConsent + "'/>")
			document.write("<input type=hidden name=returnUrl value='" + this.setting.returnUrl + "'/>")

			let scopeNames = idScopes.map(x => x.value)
			for (let i = 0; i < scopeNames.length; i++) {
				const element = scopeNames[i]
				document.write("<input type=hidden name=scopesConsented value='" + element + "'/>")
			}
			document.write('</form>')
			document.form1.submit()
		},
	},
	async beforeMount() {
		let returnUrl = this.$url.getValueFromQuery('returnUrl')
		let response = await getConsentSetting({ returnUrl })
		console.log(response)
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
	/* height: 500px; */
	border: 1px solid #d0d7dd;
	border-radius: 7px;
	background-color: white;
}

.logo {
	text-align: center;
	margin-top: 20px;
}

.el-button {
	width: 220px;
}

.notice {
	padding: 30px;
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

.authorizeItem + .authorizeItem {
	margin-bottom: 20px;
}

.title {
	text-align: center;
	margin-top: 20px;
	margin-bottom: 20px;
}
</style>
