<template>
    <div id='signinWith2fa'>
        <h1>双因素身份验证</h1>
        <p>您的登录使用身份验证器应用程序进行保护。 在下面输入您的验证码。</p>
        <p>
            验证码
            <el-input v-model="model.twoFactorCode" />
            记住浏览器 <el-switch v-model="model.rememberMachine" active-color="#13ce66" inactive-color="#ff4949"> </el-switch>

            <el-button type="primary" @click="login">登录</el-button>
        </p>
    </div>
</template>

<script>
import { checkTwoFactorAuthenticationUser, siginTwoFactorAuthenticationUser } from '../net/api.js'

import NProgress from 'nprogress'

export default {
    components: {},
    data() {
        return {
            model: {}
        };
    },
    computed: {},
    watch: {},
    methods: {
        async login() {
            let resp = await siginTwoFactorAuthenticationUser(this.model)
            console.log(resp);


            if (resp.route == 1) {
                NProgress.start()
                window.location.href = resp.data
            } else if (resp.route == 2) {
                this.$router.push('/home')
            }
        }
    },
    async beforeMount() {
        let res = await checkTwoFactorAuthenticationUser({
            rememberMe: this.$route.query.rememberMe,
            returnUrl: this.$route.query.returnUrl,
        })
        this.model = res.data
    },

}
</script>
<style scoped>
</style>