<template>
    <div id='loginWithRecoveryCode'>
        <h1>恢复码验证</h1>
        <h3>您已请求使用恢复码登录。 在您登录时提供身份验证器应用程序代码或禁用2FA并再次登录之前，将不会记住此登录信息。</h3>
        <p>
            <span>恢复码</span>
            <el-input type="text" v-model="code" autocomplete="off" autofocus />
        </p>
        <el-button type="primary" @click="login()">登录</el-button>
    </div>
</template>

<script>
import { signInWithCode } from '../net/api.js'
import NProgress from 'nprogress'

export default {
    components: {},
    data() {
        return {
            code: ''
        };
    },
    computed: {},
    watch: {},
    methods: {
        async login() {
            let resp = await signInWithCode({
                recoveryCode: this.code,
                returnUrl: this.$route.query.returnUrl
            })
            if (resp.route == 1) {
                NProgress.start()
                window.location.href = resp.data
            } else if (resp.route == 2) {
                this.$router.push('/home')
            }
        }
    },
}
</script>
<style scoped>
</style>