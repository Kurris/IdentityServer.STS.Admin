<template>
	<div id="client">
		<p style="text-align: right">
			<el-button type="primary" @click="addClient">添加新的客户端</el-button>
		</p>

		<template v-for="item in pagination.items">
			<AuthorizeItem :key="item.id" :title="item.clientName" divider :description="description">
				<template slot="img">
					<el-avatar :src="item.logoUri" style="box-shadow: 1.2px 1.5px 4px 1px #e6e6e6" />
				</template>
				<template slot="operation">
					<el-button type="primary" @click="editClient(item.id)" plain>编辑</el-button>
					<el-button type="danger" @click="removeClient(item.id, item.clientName)" plain>移除</el-button>
				</template>
			</AuthorizeItem>
		</template>

		<el-pagination
			background
			@size-change="handleSizeChange"
			@current-change="handleCurrentChange"
			:current-page="pagination.pageIndex"
			:page-sizes="[5, 10]"
			:page-size="pagination.pageSize"
			layout="total, sizes, prev, pager, next"
			:total="pagination.totalCount"
		>
		</el-pagination>

		<el-drawer title="客户端管理" :visible.sync="clientDrawer" :with-header="true" size="800px">
			<p>
				<el-button type="primary" style="width: 40%" @click="save">保存</el-button>
			</p>
			<div id="clientContainer">
				<template v-if="operateType == 0">
					<el-form ref="client" :model="form" label-width="150px">
						<el-form-item label="客户端标识">
							<el-input v-model="form.clientId"></el-input>
						</el-form-item>
						<el-form-item label="客户端名称">
							<el-input v-model="form.clientName"></el-input>
						</el-form-item>
						<el-form-item label="客户端类型">
							<el-select v-model="clientType" placeholder="请选择">
								<el-option v-for="item in clientTypes" :key="item.id" :label="item.text" :value="item.id"> </el-option>
							</el-select>
						</el-form-item>
					</el-form>
				</template>
				<template v-else>
					<el-form ref="apiResourceForm" label-position="right" :model="form" label-width="180px">
						<el-tabs type="border-card" v-model="tabName">
							<el-tab-pane label="名称" name="name">
								<el-form-item label="clientId">
									<el-input v-model="form.clientId"></el-input>
								</el-form-item>
								<el-form-item label="显示名称">
									<el-input v-model="form.clientName"></el-input>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="基本" name="claims">
								<el-form-item label="启用">
									<el-switch v-model="form.enabled"></el-switch>
								</el-form-item>
								<el-form-item label="描述">
									<el-input v-model="form.description"></el-input>
								</el-form-item>
								<el-form-item label="协议类型">
									<el-select v-model="form.protocolType" placeholder="请选择">
										<el-option v-for="item in protocolTypes" :key="item.id" :label="item.text" :value="item.id"> </el-option>
									</el-select>
								</el-form-item>
								<el-form-item label="需要客户端密钥">
									<el-switch v-model="form.requireClientSecret"></el-switch>
								</el-form-item>
								<el-form-item label="需要授权参数冗余到jwt中">
									<el-switch v-model="form.requireRequestObject"></el-switch>
								</el-form-item>
								<el-form-item label="需要 Pkce">
									<el-switch v-model="form.requirePkce"></el-switch>
								</el-form-item>
								<el-form-item label="允许纯文本 Pkce">
									<el-switch v-model="form.allowPlainTextPkce"></el-switch>
								</el-form-item>
								<el-form-item label="允许离线访问">
									<el-switch v-model="form.allowOfflineAccess"></el-switch>
								</el-form-item>
								<el-form-item label="允许通过浏览器访问令牌">
									<el-switch v-model="form.allowAccessTokensViaBrowser"></el-switch>
								</el-form-item>
								<el-form-item label="允许的重定向Uris">
									<el-input v-model="redirectUri">
										<el-button slot="append" icon="el-icon-more-outline"></el-button>
									</el-input>
									<template v-for="item in form.redirectUris">
										<el-tag :key="item.id">{{ item.redirectUri }}</el-tag>
									</template>
								</el-form-item>
								<el-form-item label="允许的作用域">
									<el-select v-model="scope">
										<el-option v-for="(item, index) in scopes" :key="index" :value="item" :label="item"> </el-option>
									</el-select>
									<br />
									<template v-for="item in form.allowedScopes">
										<el-tag :key="item.id">{{ item.scope }}</el-tag>
									</template>
								</el-form-item>
								<el-form-item label="允许的授权类型">
									<el-select v-model="grantType">
										<el-option v-for="(item, index) in grantTypes" :key="index" :value="item" :label="item"></el-option>
									</el-select>
									<el-button style="margin-left: 5px" type="primary">添加授权类型</el-button>
									<br />
									<template v-for="item in form.allowedGrantTypes">
										<el-tag :key="item.id">{{ item.grantType }}</el-tag>
									</template>
								</el-form-item>
								<el-form-item label="客户端密钥">
									<el-button type="primary">管理客户端密钥</el-button>
								</el-form-item>
								<el-form-item label="客户端属性">
									<el-button type="primary">客户端属性</el-button>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="认证/注销" name="identity">
								<el-form-item label="启用本地登录">
									<el-switch v-model="form.enableLocalLogin"></el-switch>
								</el-form-item>
								<el-form-item label="需要前端通道注销会话">
									<el-switch v-model="form.frontChannelLogoutSessionRequired"></el-switch>
								</el-form-item>
								<el-form-item v-show="form.frontChannelLogoutSessionRequired" label="前端通道注销 Uri">
									<el-input v-model="form.frontChannelLogoutUri"></el-input>
								</el-form-item>
								<el-form-item label="需要后端通道注销会话">
									<el-switch v-model="form.backChannelLogoutSessionRequired"></el-switch>
								</el-form-item>
								<el-form-item v-show="form.backChannelLogoutSessionRequired" label="后端通道注销 Uri">
									<el-input v-model="form.backChannelLogoutUri"></el-input>
								</el-form-item>
								<el-form-item label="注销重定向 Uri">
									<el-input v-model="postLogoutRedirectUri">
										<el-button slot="append" icon="el-icon-more-outline"></el-button>
									</el-input>
									<template v-for="item in form.postLogoutRedirectUris">
										<el-tag :key="item.id">{{ item.postLogoutRedirectUri }}</el-tag>
									</template>
								</el-form-item>
								<el-form-item label="外部身份提供程序限制">
									<el-input v-model="identityProviderRestriction">
										<el-button slot="append" icon="el-icon-more-outline"></el-button>
									</el-input>
									<template v-for="item in form.identityProviderRestrictions">
										<el-tag :key="item.id">{{ item.provider }}</el-tag>
									</template>
								</el-form-item>
								<el-form-item label=" 用户 SSO 生命周期">
									<el-input-number v-model="form.userSsoLifetime"> </el-input-number>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="令牌" name="token">
								<el-form-item label="身份令牌生命周期">
									<el-input-number v-model="form.identityTokenLifetime"> </el-input-number>
								</el-form-item>
								<el-form-item label="允许的身份令牌签名算法">
									<el-input v-model="form.allowedIdentityTokenSigningAlgorithms"> </el-input>
								</el-form-item>
								<el-form-item label="访问令牌生命周期">
									<el-input-number v-model="form.accessTokenLifetime"> </el-input-number>
								</el-form-item>
								<el-form-item label="访问令牌类型">
									<el-select v-model="form.accessTokenType">
										<el-option v-for="item in accessTokenTypes" :key="item.id" :value="item.id" :label="item.text"> </el-option>
									</el-select>
								</el-form-item>
								<el-form-item label="授权码生命周期">
									<el-input v-model="form.authorizationCodeLifetime"> </el-input>
								</el-form-item>
								<el-form-item label="绝对刷新令牌生命周期">
									<el-input v-model="form.absoluteRefreshTokenLifetime"> </el-input>
								</el-form-item>
								<el-form-item label="滚动刷新令牌生命周期">
									<el-input v-model="form.slidingRefreshTokenLifetime"> </el-input>
								</el-form-item>
								<el-form-item label="刷新令牌使用情况">
									<el-input v-model="form.slidingRefreshTokenLifetime"> </el-input>
								</el-form-item>
								<el-form-item label="刷新令牌过期类型">
									<el-select v-model="form.refreshTokenExpiration">
										<el-option v-for="item in tokenExpirations" :key="item.id" :label="item.text" :value="item.id"></el-option>
									</el-select>
								</el-form-item>
								<el-form-item label="允许跨域来源">
									<el-input v-model="origin">
										<el-button slot="append" icon="el-icon-more-outline"></el-button>
									</el-input>
									<template v-for="item in form.allowedCorsOrigins">
										<el-tag :key="item.id">{{ item.origin }}</el-tag>
									</template>
								</el-form-item>
								<el-form-item label="刷新时更新访问令牌声明">
									<el-switch v-model="form.updateAccessTokenClaimsOnRefresh"></el-switch>
								</el-form-item>
								<el-form-item label=" 包括 Jwt 标识">
									<el-switch v-model="form.includeJwtId"></el-switch>
								</el-form-item>
								<el-form-item label="始终发送客户端声明">
									<el-switch v-model="form.alwaysSendClientClaims"></el-switch>
								</el-form-item>
								<el-form-item label="始终在身份令牌中包含用户声明">
									<el-switch v-model="form.alwaysIncludeUserClaimsInIdToken"></el-switch>
								</el-form-item>
								<el-form-item label="客户端声明前缀">
									<el-input v-model="form.clientClaimsPrefix"></el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="Salt value used in pair-wise subjectId generation for users of this client." placement="top">
											<span>配对主体盐 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model="form.pairWiseSubjectSalt"></el-input>
								</el-form-item>
								<el-form-item label="客户端声明">
									<el-button>客户端声明</el-button>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="同意屏幕" name="consentScreen">
								<el-form-item label="需要同意">
									<el-switch v-model="form.requireConsent"></el-switch>
								</el-form-item>
								<el-form-item label="允许记住同意">
									<el-switch v-model="form.allowRememberConsent"></el-switch>
								</el-form-item>
								<el-form-item label="客户端Uri">
									<el-input v-model="form.clientUri"></el-input>
								</el-form-item>
								<el-form-item label="展示头像Uri">
									<el-input v-model="form.logoUri"></el-input>
									<p v-if="form.logoUri">
										<el-avatar :src="form.logoUri"></el-avatar>
									</p>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="设备流程" name="deviceFlow">
								<el-form-item label="用户代码类型">
									<el-input v-model="form.userCodeType"></el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认300秒,5分钟" placement="top">
											<span>设备code生命周期 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input-number v-model="form.deviceCodeLifetime"> </el-input-number>
								</el-form-item>
							</el-tab-pane>
						</el-tabs>
					</el-form>
				</template>
			</div>
		</el-drawer>
	</div>
</template>

<script>
import { getClientPage, getClientType, saveClient, removeClient, getClientById, getProtocolTypes, getGrantTypes, getScopes, getAccessTokenTypes, getTokenExpirations } from '../../../net/admin.js'
import AuthorizeItem from '../../../components/AuthorizeItem.vue'
export default {
	components: {
		AuthorizeItem,
	},
	data() {
		return {
			pagination: {
				pageIndex: 1,
				pageSize: 5,
				totalCount: 0,
				items: [],
			},
			tabName: 'name',
			clientDrawer: false,
			clientTypes: [],
			clientType: 0,
			form: {
				clientId: null,
				clientName: null,
			},
			operateType: 0,
			protocolTypes: [],
			grantTypes: [],
			grantType: '',
			redirectUri: '',
			scopes: [],
			scope: '',
			postLogoutRedirectUri: '',
			identityProviderRestriction: '',
			accessTokenTypes: [],
			tokenExpirations: [],
			origin: '',
			currentId: 0,
		}
	},
	methods: {
		handleCurrentChange(page) {
			this.pagination.pageIndex = page
			this.getClientPage()
		},
		handleSizeChange(size) {
			this.pagination.pageSize = size
			this.getClientPage()
		},
		async getClientPage() {
			await getClientPage({
				pageIndex: this.pagination.pageIndex,
				pageSize: this.pagination.pageSize,
			}).then(res => {
				this.pagination = res.data
			})
		},
		addClient() {
			this.currentId = 0
			this.operateType = 0
			getClientType().then(res => {
				this.clientTypes = res.data
			})
			this.clientDrawer = true
			this.form = {
				clientId: null,
				clientName: null,
			}
		},
		async removeClient(id, name) {
			this.$confirm(`是否要删除 ${name}`, '提醒', {
				confirmButtonText: '删除',
				cancelButtonText: '取消',
				type: 'danger',
			})
				.then(async () => {
					await removeClient({
						id,
					})

					this.$notify({
						type: 'success',
						title: '成功',
						message: '移除成功',
					})
					await this.getClientPage()
				})
				.catch(() => {})
		},
		async editClient(id) {
			this.currentId = id
			this.operateType = 1

			this.editLoading = true
			await getProtocolTypes().then(res => {
				this.protocolTypes = res.data
			})
			await getGrantTypes().then(res => {
				this.grantTypes = res.data
			})
			await getScopes().then(res => {
				this.scopes = res.data
			})
			await getAccessTokenTypes().then(res => {
				this.accessTokenTypes = res.data
			})
			await getTokenExpirations().then(res => {
				this.tokenExpirations = res.data
			})
			await getClientById({ id }).then(res => {
				this.form = res.data
			})
			this.editLoading = false
			this.clientDrawer = true
		},
		async save() {
			this.form.id = this.currentId
			await saveClient(this.form)
			this.clientDrawer = false

			this.$notify({
				type: 'success',
				title: '成功',
				message: '保存成功',
			})

			await this.getClientPage()
		},
	},
	async beforeMount() {
		await this.getClientPage()
	},
}
</script>
<style scoped>
.clientContainer {
	display: flex;
}
</style>
