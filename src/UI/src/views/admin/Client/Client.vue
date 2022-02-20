<template>
    <div id='client'>
        <div style="text-align:left">
            <el-button type="primary" @click="addClient">添加新的客户端</el-button>
        </div>
        <el-table :data="pagination.items" style="width: 100%" stripe>
            <el-table-column label="客户端标识" prop="clientId">
            </el-table-column>
            <el-table-column label="客户端名称" prop="clientName">
            </el-table-column>
            <el-table-column label="操作" fixed='right'>
                <template slot-scope="scope">
                    <el-button size="mini" @click="editClient(scope.row.id)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>

        <el-drawer title="客户端管理" :visible.sync="clientDrawer" :with-header="true" :size="800">
            <div id="clientContainer">
                <template v-if="operateType==0">
                    <div>
                        <el-form ref="client" label-position="right" :model="form" label-width="150px">
                            <el-form-item label="客户端标识">
                                <el-input v-model="form.clientId"></el-input>
                            </el-form-item>
                            <el-form-item label="客户端名称">
                                <el-input v-model="form.clientName"></el-input>
                            </el-form-item>
                            <el-form-item label="客户端类型">
                                <el-select v-model="clientType" placeholder="请选择">
                                    <el-option v-for="item in clientTypes" :key="item.id" :label="item.text" :value="item.id">
                                    </el-option>
                                </el-select>
                            </el-form-item>
                        </el-form>
                    </div>
                </template>
                <template v-else>
                    <div>
                        <el-form ref="apiResourceForm" label-position="right" :model="form" label-width="200px">
                            <el-tabs type="border-card" v-model="tabName">
                                <el-tab-pane label="名称" name="name">
                                    <el-form-item label="名称">
                                        <el-input v-model="form.clientId"></el-input>
                                    </el-form-item>
                                    <el-form-item label="规范化名称">
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
                                            <el-option v-for="item in protocolTypes" :key="item.id" :label="item.text" :value="item.id">
                                            </el-option>
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
                                            <el-tag :key="item.id">{{item.redirectUri}}</el-tag>
                                        </template>
                                    </el-form-item>
                                    <el-form-item label="允许的作用域">
                                        <el-select v-model="scope">
                                            <el-option v-for="(item,index) in scopes" :key="index" :value="item" :label="item">
                                            </el-option>
                                        </el-select>
                                        <br>
                                        <template v-for="item in form.allowedScopes">
                                            <el-tag :key="item.id">{{item.scope}}</el-tag>
                                        </template>
                                    </el-form-item>
                                    <el-form-item label="允许的授权类型">
                                        <el-select v-model="grantType">
                                            <el-option v-for="(item,index) in grantTypes" :key="index" :value="item" :label="item"></el-option>
                                        </el-select>
                                        <el-button style="margin-left:5px" type="primary">添加授权类型</el-button>
                                        <br>
                                        <template v-for="item in form.allowedGrantTypes">
                                            <el-tag :key="item.id">{{item.grantType}}</el-tag>
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
                                            <el-tag :key="item.id">{{item.postLogoutRedirectUri}}</el-tag>
                                        </template>
                                    </el-form-item>
                                    <el-form-item label="外部身份提供程序限制">
                                        <el-input v-model="identityProviderRestriction">
                                            <el-button slot="append" icon="el-icon-more-outline"></el-button>
                                        </el-input>
                                        <template v-for="item in form.identityProviderRestrictions">
                                            <el-tag :key="item.id">{{item.provider}}</el-tag>
                                        </template>
                                    </el-form-item>
                                    <el-form-item label=" 用户 SSO 生命周期">
                                        <el-input-number v-model="form.userSsoLifetime">
                                        </el-input-number>
                                    </el-form-item>

                                </el-tab-pane>
                                <el-tab-pane label="令牌" name="token">
                                    <el-form-item label="身份令牌生命周期">
                                        <el-input-number v-model="form.identityTokenLifetime">
                                        </el-input-number>
                                    </el-form-item>
                                    <el-form-item label="允许的身份令牌签名算法">
                                        <el-input v-model="form.allowedIdentityTokenSigningAlgorithms">
                                        </el-input>
                                    </el-form-item>
                                    <el-form-item label="访问令牌生命周期">
                                        <el-input-number v-model="form.accessTokenLifetime">
                                        </el-input-number>
                                    </el-form-item>
                                    <el-form-item label="访问令牌类型">
                                        <el-select v-model="form.accessTokenType">
                                            <el-option v-for="item in accessTokenTypes" :key="item.id" :value="item.id" :label="item.text">
                                            </el-option>
                                        </el-select>
                                    </el-form-item>
                                    <el-form-item label="授权码生命周期">
                                        <el-input v-model="form.authorizationCodeLifetime">
                                        </el-input>
                                    </el-form-item>
                                    <el-form-item label="绝对刷新令牌生命周期">
                                        <el-input v-model="form.absoluteRefreshTokenLifetime">
                                        </el-input>
                                    </el-form-item>
                                    <el-form-item label="滚动刷新令牌生命周期">
                                        <el-input v-model="form.slidingRefreshTokenLifetime">
                                        </el-input>
                                    </el-form-item>
                                    <el-form-item label="刷新令牌使用情况">
                                        <el-input v-model="form.slidingRefreshTokenLifetime">
                                        </el-input>
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
                                            <el-tag :key="item.id">{{item.origin}}</el-tag>
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
                                    <el-form-item label="配对主体盐">
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
                                    <el-form-item label="徽标 Uri">
                                        <el-input v-model="form.logoUri"></el-input>
                                    </el-form-item>
                                </el-tab-pane>
                                <el-tab-pane label="设备流程" name="deviceFlow">
                                    <el-form-item label="用户代码类型">
                                        <el-input v-model="form.userCodeType"></el-input>
                                    </el-form-item>
                                    <el-form-item label="设备代码生命周期">
                                        <el-input-number v-model="form.deviceCodeLifetime">
                                        </el-input-number>
                                    </el-form-item>
                                </el-tab-pane>
                            </el-tabs>

                        </el-form>
                    </div>
                </template>

                <el-button type="primary" @click="save">保存</el-button>
            </div>
        </el-drawer>
    </div>
</template>

<script>
import {
    getClientPage, getClientType, saveClient, getClientById, getProtocolTypes, getGrantTypes
    , getScopes, getAccessTokenTypes, getTokenExpirations
} from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            pagination: {
                pageIndex: 1,
                pageSize: 20,
                totalCount: 0,
                totalPagee: 0,
                items: []
            },
            tabName: "name",
            clientDrawer: false,
            clientTypes: [],
            clientType: 0,
            form: {},
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
            origin: ''
        };
    },
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getClientPage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getClientPage();
        },
        getClientPage() {
            getClientPage(this.pagination).then(res => {
                this.pagination = res.data
                console.log(this.pagination);
            })
        },
        addClient() {
            this.operateType = 0
            getClientType().then(res => {
                this.clientTypes = res.data
            })
            this.clientDrawer = true
        },
        editClient(id) {
            this.operateType = 1
            this.clientDrawer = true
            getProtocolTypes().then(res => {
                this.protocolTypes = res.data
            })
            getGrantTypes().then(res => {
                this.grantTypes = res.data
            })
            getScopes().then(res => {
                this.scopes = res.data
            })
            getAccessTokenTypes().then(res => {
                this.accessTokenTypes = res.data
            })
            getTokenExpirations().then(res => {
                this.tokenExpirations = res.data
            })
            getClientById({ id }).then(res => {
                this.form = res.data
                console.log(this.form);
            })

        },
        async save() {
            await saveClient({
                clientId: this.form.clientId,
                clientName: this.form.clientName,
                clientType: this.clientType
            })
        }
    },
    beforeMount() {
        this.getClientPage()
    },
}
</script>
<style scoped>
#clientContainer {
    text-align: left;
}
</style>