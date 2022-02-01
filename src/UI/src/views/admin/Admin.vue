<template>
    <div id='admin'>
        <div v-show="$route.path=='/admin'">
            <el-row justify="space-around" :gutter="20">
                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>客户端</b></span>
                        </div>
                        <el-button type="primary">管理</el-button>
                    </el-card>
                </el-col>
                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>身份资源</b></span>
                        </div>
                        <el-button type="primary" @click="$router.push('/admin/identityResource')">管理</el-button>
                    </el-card>
                </el-col>
                <el-col :span="colSpan">
                    <el-card class="box-card" v-show="isAdmin">
                        <div slot="header" class="clearfix">
                            <span><b>Api 资源</b></span>
                        </div>
                        <el-button type="primary">管理</el-button>
                    </el-card>
                </el-col>
                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>Api 作用域</b></span>
                        </div>
                        <el-button type="primary">管理</el-button>
                    </el-card>
                </el-col>

                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>持久授权</b></span>
                        </div>
                        <el-button type="primary">管理</el-button>
                    </el-card>
                </el-col>

                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>用户</b></span>
                        </div>
                        <el-button type="primary" @click="$router.push('/admin/user')">管理</el-button>
                    </el-card>
                </el-col>

                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>角色</b></span>
                        </div>
                        <el-button type="primary">管理</el-button>
                    </el-card>
                </el-col>

                <el-col :span="colSpan" v-show="isAdmin">
                    <el-card class="box-card">
                        <div slot="header" class="clearfix">
                            <span><b>审计</b></span>
                        </div>
                        <el-button type="primary">管理</el-button>
                    </el-card>
                </el-col>
            </el-row>
            <el-result v-if="!isAdmin&&status" icon="error" title="权限限制" subTitle="您当前没有权限访问">
                <template slot="extra">
                    <el-button type="primary" size="medium" @click="goToLogin()">返回</el-button>
                </template>
            </el-result>
        </div>
        <div v-show="$route.path.indexOf('/admin')>-1 && $route.path!='/admin'">
            <router-view />
        </div>
    </div>
</template>

<script>
import { getLoginStatus } from '../../net/api.js'

export default {
    components: {},
    data() {
        return {
            colSpan: this.isAuthenticated ? 8 : 12,
            isAdmin: false,
            isAuthenticated: false,
            status: null,

        };
    },
    computed: {},
    watch: {},
    methods: {
        goToLogin() {
            this.$router.push({
                path: '/signIn',
                query: {
                    returnUrl: window.location.href
                }
            })
        }
    },
    async beforeMount() {

        let res = await getLoginStatus();
        if (res.code == 200) {
            this.status = res.data
            this.isAuthenticated = res.data.isLogin
            this.isAdmin = res.data.isAdmin
        } else {
            this.status = {}
        }
    },
}
</script>
<style scoped>
.el-card {
    width: auto;
}
</style>