<template>
    <div class='userCodeConfirmation'>
        <div class="consent-container">
            <div class="row page-header">
                <div class="col-sm-10">
                    <template v-if="model.clientLogoUrl!=null">
                        <div class="client-logo"><img :src="model.clientLogoUrl"></div>
                    </template>
                    <h1>
                        {{model.clientName}}
                        <small>正在请求您的许可</small>
                    </h1>
                </div>
            </div>

            <template v-if="model.confirmUserCode">
                <div class="row">
                    <div class="col-sm-8">
                        <p>
                            请确认授权请求引用代码: {{model.userCode}}.
                        </p>
                    </div>
                </div>
            </template>

            <div class="row">
                <div class="col-sm-12">

                    <form asp-action="Callback" class="consent-form">
                        <input type="hidden" asp-for="UserCode" />

                        <div>取消选中您不希望授予的权限</div>

                        <template v-if="model.identityScopes!=null && model.identityScopes.length>0">
                            <div class="col-sm-12">
                                <div class="card mt-3">
                                    <h5 class="card-header">
                                        <i class="fa fa-user"></i>
                                        个人信息
                                    </h5>
                                    <div class="card-body">
                                        <ul class="list-group">

                                            <template v-for="(item,index) in model.identityScopes">
                                                <div :key="index">
                                                    <ScopeItem :scope="item" />
                                                </div>
                                            </template>

                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </template>
                        <template v-if="model.ApiScopes!=null && model.ApiScopes.length > 0">
                            <div class="col-sm-12">
                                <div class="card mt-3">
                                    <h5 class="card-header">
                                        <i class="fa fa-lock"></i>
                                        应用访问
                                    </h5>
                                    <div class="card-body">

                                        <ul class="list-group">
                                            <template v-for="(item,index) in model.ApiScopes">
                                                <div :key="index">
                                                    <ScopeItem :scope="item" />
                                                </div>
                                            </template>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </template>

                        <!-- 允许记住同意屏幕的设置 -->
                        <template v-if="model.allowRememberConsent">
                            <div>
                                <div class="row m-4">
                                    <div class="col-sm-12">
                                        <div class="toggle-button__input">
                                            <el-switch v-model="model.rememberConsent" active-color="#13ce66" inactive-color="#ff4949">
                                            </el-switch>
                                        </div>
                                        <div class="toggle-button__text">
                                            <strong>记住</strong>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </template>

                        <div class="row ml-4 mr-4">
                            <div class="col-sm-9 mt-3">
                                <el-button type="primary" autofocus @click="process('yes')">许可</el-button>
                                <el-button type="warning" @click="process('no')">不许可</el-button>
                            </div>

                            <div class="col-sm-3 mt-3">
                                <template v-if="model.clientUrl != null">
                                    <a class="btn btn-outline-primary" target="_blank" :href="model.clientUrl">
                                        <i class="fa fa-info-circle"></i>
                                        <strong>{{model.ClientName}}</strong>
                                    </a>
                                </template>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import ScopeItem from './ScopeItem.vue'
import { processDevice } from '../net/api.js'

export default {
    components: {
        ScopeItem
    },
    data() {
        return {
            model: {}
        };
    },
    computed: {},
    watch: {},
    methods: {
        async process(btn) {
            this.model.button = btn
            let idScopes = this.model.identityScopes.filter(x => x.checked);
            this.model.scopesConsented = idScopes.map(x => x.value)

            await processDevice(this.model);
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
</style>