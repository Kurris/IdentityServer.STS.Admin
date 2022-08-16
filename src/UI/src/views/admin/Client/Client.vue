<template>
	<div id="client">
		<p style="text-align: right">
			<el-button type="primary" @click="addClient">添加新的应用</el-button>
		</p>

		<template v-if="pagination.items.length > 0">
			<template v-for="item in pagination.items">
				<AuthorizeItem :key="item.id" divider :description="item.description">
					<template> {{ item.clientName }} | {{ item.clientId }} </template>
					<template slot="img">
						<el-avatar :src="item.logoUri" style="box-shadow: 1.2px 1.5px 4px 1px #e6e6e6" />
					</template>
					<template slot="operation" v-if="item.nonEditable == 0">
						<el-button type="primary" @click="editClient(item.id)" plain>编辑</el-button>
						<el-button type="danger" @click="removeClient(item.id, item.clientName)" plain>移除</el-button>
					</template>
				</AuthorizeItem>
			</template>

			<el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange"
				:current-page="pagination.pageIndex" :page-sizes="[5, 10]" :page-size="pagination.pageSize"
				layout="total, sizes, prev, pager, next" :total="pagination.totalCount">
			</el-pagination>
		</template>

		<el-drawer title="客户端管理" :visible.sync="clientDrawer" :with-header="true" size="800px">
			<el-divider></el-divider>
			<div class="clientContainer" v-if="enums != null">
				<el-button type="primary" @click="save" style="margin-left: 20px">保存应用配置</el-button>
				<template v-if="operateType == 0">
					<el-card style="margin: 20px" shadow="hover">
						<el-form ref="client" label-position="left" :model="form" label-width="240px">
							<el-form-item label="应用类型">
								<el-select v-model="clientType" placeholder="请选择" style="width: 100%">
									<el-option v-for="item in enums.clientType" :key="item.id" :label="item.text"
										:value="item.id"> </el-option>
								</el-select>
							</el-form-item>
							<el-form-item label="应用名称">
								<el-input v-model="form.clientName"></el-input>
							</el-form-item>
							<el-form-item label="描述">
								<el-input v-model="form.description" type="textarea"
									:autosize="{ minRows: 4, maxRows: 4 }"></el-input>
							</el-form-item>
							<el-form-item label="应用Uri">
								<el-input v-model="form.clientUri"></el-input>
							</el-form-item>
							<el-form-item label="展示图标Uri">
								<el-input v-model="form.logoUri"></el-input>
								<p v-if="form.logoUri">
									<el-avatar :src="form.logoUri"></el-avatar>
								</p>
							</el-form-item>
							<el-form-item label="允许跨域来源">
								<template v-for="item in form.allowedCorsOrigins">
									<div :key="item.origin">
										<el-tag closable
											@close="form.allowedCorsOrigins = form.allowedCorsOrigins.filter(x => x.origin != item.origin)">
											{{ item.origin }}</el-tag>
									</div>
								</template>
								<el-input v-model="origin">
									<el-button slot="append" @click="addOrigin">添加允许跨域来源</el-button>
								</el-input>
							</el-form-item>
							<el-form-item label="授权后重定向Uri">
								<template v-for="item in form.redirectUris">
									<div :key="item.redirectUri">
										<el-tag closable
											@close="form.redirectUris = form.redirectUris.filter(x => x.redirectUri != item.redirectUri)">
											{{ item.redirectUri }}</el-tag>
									</div>
								</template>
								<el-input v-model="redirectUri">
									<el-button slot="append" @click="addRedirectUri()"> 添加重定向Uri </el-button>
								</el-input>
								<!-- </div> -->
							</el-form-item>
							<el-form-item label="注销后重定向Uri">
								<template v-for="item in form.postLogoutRedirectUris">
									<div :key="item.postLogoutRedirectUri">
										<el-tag closable
											@close="form.postLogoutRedirectUris = form.postLogoutRedirectUris.filter(x => x.postLogoutRedirectUri != item.postLogoutRedirectUri)">
											{{ item.postLogoutRedirectUri }}
										</el-tag>
									</div>
								</template>
								<el-input v-model="postLogoutRedirectUri">
									<el-button slot="append" type="primary" @click="addPostLogoutRedirectUri">
										添加注销后重定向Uri </el-button>
								</el-input>
							</el-form-item>
						</el-form>
					</el-card>
				</template>
				<template v-else>
					<el-form ref="apiResourceForm" label-position="left" :model="form" label-width="240px">
						<el-tabs type="border-card" v-model="tabName" style="margin: 20px">
							<el-tab-pane label="名称" name="name">
								<el-form-item label="ClientId">
									<el-input v-model="form.clientId" readonly></el-input>
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
									<el-input v-model="form.description" type="textarea" autosize></el-input>
								</el-form-item>
								<el-form-item label="需要应用密钥">
									<el-switch v-model="form.requireClientSecret"></el-switch>
								</el-form-item>
								<el-form-item label="应用密钥" v-if="form.requireClientSecret">
									<el-button type="primary" @click="secretVisible = true">管理应用密钥</el-button>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认为 false" placement="top">
											<span>需要请求授权参数附加到JWT Claims中, <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.requireRequestObject"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认为 true" placement="top">
											<span>需要Pkce <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.requirePkce"></el-switch>
								</el-form-item>
								<el-form-item label="允许纯文本Pkce进行challenge">
									<el-switch v-model="form.allowPlainTextPkce"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark"
											content="当前client需要有offline_access作用域,并且授权请求时scope必须带上offline_access;(oidc-client-js automaticSilentRenew: true)"
											placement="top">
											<span>允许离线访问 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.allowOfflineAccess"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="如果使用implicit flow,必须为true"
											placement="top">
											<span>允许通过浏览器访问令牌 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.allowAccessTokensViaBrowser"></el-switch>
								</el-form-item>
								<el-form-item label="授权后重定向Uri">
									<template v-for="item in form.redirectUris">
										<div :key="item.redirectUri">
											<el-tag closable
												@close="form.redirectUris = form.redirectUris.filter(x => x.redirectUri != item.redirectUri)">
												{{ item.redirectUri }}</el-tag>
										</div>
									</template>
									<el-input v-model="redirectUri">
										<el-button slot="append" @click="addRedirectUri()"> 添加重定向Uri </el-button>
									</el-input>
									<!-- </div> -->
								</el-form-item>
								<el-form-item label="允许的作用域">
									<template v-for="item in form.allowedScopes">
										<div :key="item.scope">
											<el-tag closable
												@close="form.allowedScopes = form.allowedScopes.filter(x => x.scope != item.scope)">
												{{ item.scope }}</el-tag>
										</div>
									</template>
									<div>
										<el-select v-model="scope" v-if="scopes.length > 0 && form.allowedScopes">
											<el-option
												v-for="(item, index) in scopes.filter(x => form.allowedScopes.map(x => x.scope).indexOf(x) < 0)"
												:key="index" :value="item" :label="item">
											</el-option>
										</el-select>
										<el-button style="margin-left: 5px" type="primary" @click="addScope">添加作用域
										</el-button>
									</div>
								</el-form-item>
								<el-form-item label="允许的授权类型" v-if="grantTypes.length > 0 && form.allowedGrantTypes">
									<template v-for="item in form.allowedGrantTypes">
										<div :key="item.grantType">
											<el-tag closable
												@close="form.allowedGrantTypes = form.allowedGrantTypes.filter(x => x.grantType != item.grantType)">
												{{ item.grantType }}</el-tag>
										</div>
									</template>
									<div>
										<el-select v-model="grantType">
											<el-option
												v-for="(item, index) in grantTypes.filter(x => form.allowedGrantTypes.map(x => x.grantType).indexOf(x) < 0)"
												:key="index" :value="item" :label="item"></el-option>
										</el-select>
										<el-button style="margin-left: 5px" type="primary" @click="addGrantType">添加授权类型
										</el-button>
									</div>
								</el-form-item>
								<el-form-item label="应用属性">
									<el-button type="primary">管理应用属性(未实现)</el-button>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="认证/注销" name="identity">
								<el-form-item label="启用本地登录">
									<el-switch v-model="form.enableLocalLogin"></el-switch>
								</el-form-item>
								<el-card style="margin-top: 10px; margin-bottom: 10px" shadow="hover">
									<el-form-item label="前端通道注销携带session id">
										<el-switch v-model="form.frontChannelLogoutSessionRequired"></el-switch>
									</el-form-item>
									<el-form-item label="前端通道注销 Uri">
										<el-input v-model="form.frontChannelLogoutUri"></el-input>
									</el-form-item>
								</el-card>
								<el-card style="margin-top: 10px; margin-bottom: 10px" shadow="hover">
									<el-form-item label="后端通道注销携带session id">
										<el-switch v-model="form.backChannelLogoutSessionRequired"></el-switch>
									</el-form-item>
									<el-form-item label="后端通道注销 Uri">
										<el-input v-model="form.backChannelLogoutUri"></el-input>
									</el-form-item>
								</el-card>
								<el-form-item label="注销后重定向Uri">
									<template v-for="item in form.postLogoutRedirectUris">
										<div :key="item.postLogoutRedirectUri">
											<el-tag closable
												@close="form.postLogoutRedirectUris = form.postLogoutRedirectUris.filter(x => x.postLogoutRedirectUri != item.postLogoutRedirectUri)">
												{{ item.postLogoutRedirectUri }}
											</el-tag>
										</div>
									</template>
									<el-input v-model="postLogoutRedirectUri">
										<el-button slot="append" type="primary" @click="addPostLogoutRedirectUri">
											添加注销后重定向Uri </el-button>
									</el-input>
								</el-form-item>
								<el-form-item v-if="form.identityProviderRestrictions">
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="设置允许的外部登录,如果为空则允许所有,默认为空"
											placement="top">
											<span>外部身份提供程序限制 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<template v-for="item in form.identityProviderRestrictions">
										<div :key="item.provider">
											<el-tag closable
												@close="form.identityProviderRestrictions = form.identityProviderRestrictions.filter(x => x.provider != item.provider)">
												{{ item.provider }}</el-tag>
										</div>
									</template>
									<div>
										<el-select v-model="identityProviderRestriction">
											<el-option
												v-for="item in externalProviders.filter(x => form.identityProviderRestrictions.map(p => p.provider).indexOf(x.authenticationScheme) < 0)"
												:key="item.authenticationScheme" :value="item.authenticationScheme"
												:label="item.displayName">
											</el-option>
										</el-select>
										<el-button style="margin-left: 5px" type="primary"
											@click="addIdentityProviderRestriction">添加外部登录限制 </el-button>
									</div>
								</el-form-item>
								<el-form-item label=" 用户SSO生命周期">
									<template slot="label">
										<el-tooltip class="item" effect="dark"
											content="自上次用户进行身份验证以来的最大持续时间(second).默认值为null.您可以调整会话令牌的生存期,以控制在使用 Web 应用程序时，要求用户重新输入凭据的时间和频率,而不是以静默方式进行身份验证."
											placement="top">
											<span>用户SSO生命周期 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>

									<el-input v-model.number="form.userSsoLifetime"> </el-input>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="令牌" name="token">
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认300秒,5分钟" placement="top">
											<span>身份令牌生命周期 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model.number="form.identityTokenLifetime"> </el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="id令牌的签名算法,如果为空则使用服务端的默认算法"
											placement="top">
											<span>允许的身份令牌签名算法 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model="form.allowedIdentityTokenSigningAlgorithms"> </el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认3600秒,一小时" placement="top">
											<span>访问令牌生命周期 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model.number="form.accessTokenLifetime"> </el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认300秒,5分钟" placement="top">
											<span>授权码生命周期 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model.number="form.authorizationCodeLifetime"></el-input>
								</el-form-item>
								<el-card style="margin-top: 10px; margin-bottom: 10px" shadow="hover">
									<el-form-item label="刷新令牌使用方式">
										<el-select v-model="form.refreshTokenUsage">
											<el-option v-for="item in enums.tokenUsage" :key="item.id"
												:label="item.text" :value="item.id" />
										</el-select>
									</el-form-item>
									<el-form-item label="刷新令牌过期类型">
										<el-select v-model="form.refreshTokenExpiration">
											<el-option v-for="item in enums.tokenExpiration" :key="item.id"
												:label="item.text" :value="item.id" />
										</el-select>
									</el-form-item>
									<el-form-item label="绝对刷新令牌生命周期" v-if="form.refreshTokenExpiration == 1">
										<el-input v-model="form.absoluteRefreshTokenLifetime"> </el-input>
									</el-form-item>
									<el-form-item label="滑动刷新令牌生命周期" v-else>
										<el-input v-model="form.slidingRefreshTokenLifetime"> </el-input>
									</el-form-item>
								</el-card>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="是否应在刷新令牌请求上更新访问令牌(及其声明)"
											placement="top">
											<span>刷新时更新访问令牌声明 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.updateAccessTokenClaimsOnRefresh"></el-switch>
								</el-form-item>
								<el-form-item label="访问令牌类型">
									<el-select v-model="form.accessTokenType">
										<el-option v-for="item in enums.accessTokenType" :key="item.id" :value="item.id"
											:label="item.text"> </el-option>
									</el-select>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark"
											content="指定JWT访问令牌是否携带唯一ID(通过claims.jti)。默认值为 true" placement="top">
											<span>携带JWT唯一ID <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.includeJwtId"></el-switch>
								</el-form-item>
								<el-form-item label="允许跨域来源">
									<template v-for="item in form.allowedCorsOrigins">
										<div :key="item.origin">
											<el-tag closable
												@close="form.allowedCorsOrigins = form.allowedCorsOrigins.filter(x => x.origin != item.origin)">
												{{ item.origin }}</el-tag>
										</div>
									</template>
									<el-input v-model="origin">
										<el-button slot="append" @click="addOrigin">添加允许跨域来源</el-button>
									</el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark"
											content="如果设置了,在所有授权flow中客户端声明会被携带,否则只有客户端凭证授权flow才会携带,默认为 false"
											placement="top">
											<span>携带客户端声明<i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.alwaysSendClientClaims"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark"
											content="请求id令牌和访问令牌时,是否应始终将用户声明添加到 id 令牌，而不是要求客户端使用 userinfo 终结点。默认值为 false"
											placement="top">
											<span>始终在身份令牌中携带用户声明<i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.alwaysIncludeUserClaimsInIdToken"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark"
											content="Salt value used in pair-wise subjectId generation for users of this client."
											placement="top">
											<span>配对主体盐 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model="form.pairWiseSubjectSalt"></el-input>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="默认 client_" placement="top">
											<span>客户端声明前缀,在ClientCliams定义的type在JWT都以前缀+type的方式显示(除了client_id)<i
													class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model="form.clientClaimsPrefix"></el-input>
								</el-form-item>
								<el-form-item label="客户端声明">
									<el-button>客户端声明</el-button>
								</el-form-item>
							</el-tab-pane>
							<el-tab-pane label="同意屏幕" name="consentScreen">
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="定义是否展示同意屏幕,默认为 false"
											placement="top">
											<span>启用同意屏幕授权流程 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.requireConsent"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="用户是否可以记住同意屏幕的授权操作,默认为 true"
											placement="top">
											<span>允许记住授权操作 <i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-switch v-model="form.allowRememberConsent"></el-switch>
								</el-form-item>
								<el-form-item>
									<template slot="label">
										<el-tooltip class="item" effect="dark" content="授权的生命时间,默认为 null,永不过期"
											placement="top">
											<span>同意屏幕授权生命周期<i class="el-icon-info"></i></span>
										</el-tooltip>
									</template>
									<el-input v-model.number="form.consentLifetime"></el-input>
								</el-form-item>
								<el-form-item label="应用Uri">
									<el-input v-model="form.clientUri"></el-input>
								</el-form-item>
								<el-form-item label="展示图标Uri">
									<el-input v-model="form.logoUri"></el-input>
									<p v-if="form.logoUri">
										<el-avatar :src="form.logoUri"></el-avatar>
									</p>
								</el-form-item>
								<el-form-item>
									<el-button>预览同意屏幕</el-button>
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
									<el-input v-model.number="form.deviceCodeLifetime"> </el-input>
								</el-form-item>
							</el-tab-pane>
						</el-tabs>
					</el-form>
				</template>
			</div>
		</el-drawer>
		<el-dialog :title="form.clientName + '密钥管理'" :visible.sync="secretVisible">
			<div style="display: flex; align-items: center; justify-content: space-around">
				<div style="display: flex; align-items: center; justify-content: space-around">
					<span> 加密方式: </span>
					<el-select v-model="hashType" placeholder="请选择" v-if="enums != null">
						<el-option v-for="item in enums.hashType" :key="item.id" :label="item.text" :value="item.id">
						</el-option>
					</el-select>
				</div>
				<el-button type="primary" @click="generateSecret">生成新的应用密钥</el-button>
			</div>
			<el-alert v-if="secret != null" :title="secret" description="确保您已经复制当前的密钥,这将不会再次显示" type="warning" show-icon
				:closable="false"> </el-alert>
			<el-divider></el-divider>

			<div v-loading="secretLoading">
				<template v-for="item in clientSecrets">
					<AuthorizeItem :key="item.id" divider :title="'secret: ' + item.description"
						:description="'创建时间: ' + item.created">
						<template slot="img">
							<i class="el-icon-key" style="font-size: 32px"></i>
						</template>
						<template slot="operation">
							<el-button type="danger" @click="removeClientSecret(item.id)" plain>移除</el-button>
						</template>
					</AuthorizeItem>
				</template>
				<el-empty description="暂无数据" v-if="clientSecrets.length == 0"></el-empty>
			</div>
		</el-dialog>
	</div>
</template>

<script>
import {
	getClientPage,
	saveClient,
	removeClient,
	getClientById,
	getGrantTypes,
	getScopes,
	getEnums,
	getExternalProviders,
	getClientSecrets,
	addClientSecret,
	removeClientSecret,
} from '../../../net/admin.js'
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
			secretVisible: false,
			tabName: 'name',
			clientDrawer: false,
			clientType: 0,
			form: {
				clientId: null,
				clientName: null,
			},
			operateType: 0,
			grantTypes: [],
			grantType: '',
			redirectUri: '',
			scopes: [],
			scope: '',
			postLogoutRedirectUri: '',
			identityProviderRestriction: '',
			origin: '',
			currentId: 0,
			enums: null,
			externalProviders: [],
			clientSecrets: [],
			hashType: 0,
			secret: null,
			secretLoading: false,
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
				.catch(() => { })
		},
		async editClient(id) {
			this.currentId = id
			this.operateType = 1

			this.editLoading = true

			await getClientById({ id }).then(res => {
				this.form = res.data
				console.log(this.form)
			})

			await getGrantTypes().then(res => {
				this.grantTypes = res.data
			})
			await getScopes().then(res => {
				this.scopes = res.data
			})

			this.editLoading = false
			this.clientDrawer = true
		},
		async save() {
			this.form.id = this.currentId
			this.form.ClientType = this.clientType
			this.form.protocolType = 'oidc'

			await saveClient(this.form)
			this.clientDrawer = false

			this.$notify({
				type: 'success',
				title: '成功',
				message: '保存成功',
			})

			await this.getClientPage()
		},
		addRedirectUri() {
			if (this.redirectUri == '') {
				return
			}

			this.form.redirectUris.push({
				redirectUri: this.redirectUri,
			})
			this.redirectUri = ''
		},
		addScope() {
			if (!this.scope || this.scope == '') {
				return
			}
			this.form.allowedScopes.push({
				scope: this.scope,
			})
			this.scope = ''
		},
		addGrantType() {
			if (!this.grantType || this.grantType == '') {
				return
			}

			this.form.allowedGrantTypes.push({
				grantType: this.grantType,
			})
			this.grantType = ''
		},
		addPostLogoutRedirectUri() {
			if (!this.postLogoutRedirectUri || this.postLogoutRedirectUri == '') {
				return
			}

			this.form.postLogoutRedirectUris.push({
				postLogoutRedirectUri: this.postLogoutRedirectUri,
			})
			this.postLogoutRedirectUri = ''
		},
		addIdentityProviderRestriction() {
			if (!this.identityProviderRestriction || this.identityProviderRestriction == '') {
				return
			}

			this.form.identityProviderRestrictions.push({
				provider: this.identityProviderRestriction,
			})
			this.identityProviderRestriction = ''
		},
		addOrigin() {
			if (!this.origin || this.origin == '') {
				return
			}

			this.form.allowedCorsOrigins.push({
				origin: this.origin,
			})
			this.origin = ''
		},
		async generateSecret() {
			let res = await addClientSecret({
				hashType: this.hashType,
				clientId: this.form.id,
			})

			this.secret = res.data
			await this.getClientSecrets()
		},
		async getClientSecrets() {
			this.secretLoading = true
			let clientSecretsRes = await getClientSecrets({
				clientId: this.form.id,
			}).finally(() => {
				this.secretLoading = false
			})
			this.clientSecrets = clientSecretsRes.data
			console.log(this.clientSecrets)
		},
		async removeClientSecret(id) {
			await removeClientSecret({
				id: id,
			})

			await this.getClientSecrets()
		},
	},
	watch: {
		async secretVisible() {
			if (this.secretVisible == true) {
				await this.getClientSecrets()
			} else {
				this.secret = null
				this.clientSecrets = []
			}
		},
	},
	async beforeMount() {
		await this.getClientPage()
		let enumsRes = await getEnums()
		this.enums = enumsRes.data

		let externalProviderRes = await getExternalProviders()
		this.externalProviders = externalProviderRes.data
	},
}
</script>
<style scoped>
</style>
