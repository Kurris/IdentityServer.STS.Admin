<template>
    <div id='consent'>

        <div class="consent-container">
            <div>
                <div>
                    <template v-if="setting.clientLogoUrl!=null">
                        <div>
                            <img :src="setting.ClientLogoUrl">
                        </div>
                    </template>
                    <h1>
                        {{setting.clientName}}
                        <small>正在请求你的许可</small>
                    </h1>
                </div>
            </div>

            <div class="row">
                <div>

                    <div>取消选中您不希望授予的权限</div>

                    <template v-if="setting.identityScopes!=null && setting.identityScopes.length>0">
                        <div class="col-sm-12">
                            <div class="card mt-3">
                                <h5 class="card-header">
                                    <i class="fa fa-user"></i>
                                    个人信息
                                </h5>
                                <div class="card-body">
                                    <ul class="list-group">

                                        <template v-for="(item,index) in setting.identityScopes">
                                            <div :key="index">
                                                <ScopeItem :scope="item" />
                                            </div>
                                        </template>

                                    </ul>
                                </div>
                            </div>
                        </div>
                    </template>
                    <template v-if="setting.ApiScopes!=null && setting.ApiScopes.length > 0">
                        <div class="col-sm-12">
                            <div class="card mt-3">
                                <h5 class="card-header">
                                    <i class="fa fa-lock"></i>
                                    应用访问
                                </h5>
                                <div class="card-body">

                                    <ul class="list-group">
                                        <template v-for="(item,index) in setting.ApiScopes">
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
                    <template v-if="setting.allowRememberConsent">
                        <div>
                            <div class="row m-4">
                                <div class="col-sm-12">
                                    <div class="toggle-button__input">
                                        <el-switch v-model="setting.rememberConsent" active-color="#13ce66" inactive-color="#ff4949">
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
                            <template v-if="setting.clientUrl!=null">
                                <a class="btn btn-outline-primary" target="_blank" :href="setting.clientUrl">
                                    <i class="fa fa-info-circle"></i>
                                    <strong>{{setting.clientName}}</strong>
                                </a>
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
import NProgress from 'nprogress'

export default {
    components: {
        ScopeItem
    },
    data() {
        return {
            setting: {}
        };
    },
    methods: {
        process(btn) {


            let idScopes = this.setting.identityScopes.filter(x => x.checked);


            NProgress.start()
            let url = "http://localhost:5000/api/consent/setting/process"

            document.write("<form action=" + url + " method=post name=form1 style='display:none'>");
            document.write("<input type=hidden name=button value='" + btn + "'/>");
            document.write("<input type=hidden name=rememberConsent value='" + this.setting.rememberConsent + "'/>");
            document.write("<input type=hidden name=returnUrl value='" + this.setting.returnUrl + "'/>");


            let scopeNames = idScopes.map(x => x.value)
            for (let i = 0; i < scopeNames.length; i++) {
                const element = scopeNames[i];
                document.write("<input type=hidden name=scopesConsented value='" + element + "'/>");
            }
            document.write("</form>");
            document.form1.submit();
        }
    },
    async beforeMount() {
        let returnUrl = this.$url.getValueFromQuery('returnUrl')
        let response = await getConsentSetting({ returnUrl })
        this.setting = response.data
    },
}
</script>
<style scoped>
</style>