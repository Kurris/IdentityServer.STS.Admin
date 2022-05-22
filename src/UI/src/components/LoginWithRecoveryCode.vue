<template>
    <div id='loginWithRecoveryCode'>
        <div>
            <div class="title">
                <el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="65"></el-avatar>
                <div style="margin-top: 40px;">
                    <h2>
                        恢复码验证
                    </h2>
                </div>
            </div>

            <div class="panel">
                <div class="notice">
                    <div style="text-align: center;">
                        <i class="el-icon-key" style="font-size: 40px;color: #555b65;" />
                    </div>

                    <span style="font-size: 13px;">恢复码:</span>
                    <el-input v-model="code" style="margin-bottom: 20px;margin-top: 10px;" placeholder="请输入恢复码"
                        autofocus />

                    <el-button class="green" @click="login">恢复码验证</el-button>

                    <p style="font-size: 13px;color: #636d74;">
                        您已请求使用恢复码登录,当您无法访问双重验证器所在的设备,输入一组恢复码来验证您的身份。
                    </p>
                    <el-divider></el-divider>

                </div>

            </div>

            <div class="footer">
                账号已被锁定?<el-link type="primary" @click="loginWithCode()">尝试恢复您的账号</el-link>
            </div>

        </div>
    </div>
</template>

<script>
import { signInWithCode, getLoginStatus, goSignInWithCode } from '../net/api.js'
import NProgress from 'nprogress'

export default {
    components: {},
    data() {
        return {
            code: ''
        };
    },
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
                let res = await getLoginStatus()
                let userName = res.data.user.userName
                this.$router.push('/zone/' + userName)
            }
        }
    },
    async beforeMount() {
        await goSignInWithCode()
    }
}
</script>
<style scoped>
#loginWithRecoveryCode {
    display: flex;
    height: 100vh;
    justify-content: center;
    background: #f6f8fa;
}


.title {
    text-align: center;
    margin-top: 30px;
    margin-bottom: 30px;
}

.panel {
    width: 350px;
    border: 1px solid #d0d7dd;
    border-radius: 7px;
    background-color: white;
}

.notice {
    padding: 30px;

}

.el-button.green {
    background-color: #2aa44c;
    color: white;
    width: 290px;
}


.footer {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 350px;
    height: 65px;
    border: 1px solid #d0d7dd;
    border-radius: 7px;
    margin-top: 20px;
    font-size: 14px;
    color: #92969a;
}
</style>