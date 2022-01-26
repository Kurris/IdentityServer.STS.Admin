<template>
    <div id='admin'>
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
                    <el-button type="primary">管理</el-button>
                </el-card>
            </el-col>
            <el-col :span="colSpan">
                <el-card class="box-card" v-show="isAdmin">
                    <div slot="header" class="clearfix">
                        <span><b>Api 资源</b></span>
                    </div>
                    <el-button type="primary">发现文档</el-button>
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
                    <el-button type="primary">管理</el-button>
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
        <el-result v-if="!isAdmin" icon="error" title="权限限制" subTitle="您当前没有权限访问">
            <template slot="extra">
                <el-button type="primary" size="medium" @click="goToLogin()">返回</el-button>
            </template>
        </el-result>
    </div>
</template>

<script>

export default {
    components: {},
    data() {
        return {
            colSpan: this.isAuthenticated ? 8 : 12,
            isAdmin: false,
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
    //生命周期 - 创建完成（可以访问当前this实例）
    created() {

    },
    //生命周期 - 挂载完成（可以访问DOM元素）
    mounted() {

    },
    beforeCreate() { }, //生命周期 - 创建之前
    beforeMount() { }, //生命周期 - 挂载之前
    beforeUpdate() { }, //生命周期 - 更新之前
    updated() { }, //生命周期 - 更新之后
    beforeDestroy() { }, //生命周期 - 销毁之前
    destroyed() { }, //生命周期 - 销毁完成
    activated() { }, //如果页面有keep-alive缓存功能，这个函数会触发
}
</script>
<style scoped>
.el-card {
    width: auto;
}
</style>