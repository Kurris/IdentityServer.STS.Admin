<template>
    <div id='twoFactorAuthentication'>
        <div>
            <h1>双因素身份验证 (2FA)</h1>
            <template v-if="setting.is2faEnabled">
                <template v-if="setting.recoveryCodesLeft==0">
                    <div class="col-12">
                        <strong>您还没有恢复码</strong>
                        <p>你必须 <el-link type="primary" @click="$router.push('/showRecoveryCodes')">生成一组新的恢复码</el-link> 在您使用恢复码登录之前</p>
                    </div>
                </template>
                <template v-else-if="setting.recoveryCodesLeft==1">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <strong>您还剩 1 个恢复码</strong>
                            <p>您可以生成一组新的恢复码 </p>
                            <el-link type="primary" @click="$router.push('/showRecoveryCodes')">生成一组新的恢复码</el-link>
                        </div>
                    </div>
                </template>
                <template v-else-if="setting.recoveryCodesLeft <= 3">

                    <div class="alert alert-warning">
                        <strong>你有 {{setting.recoveryCodesLeft}}个恢复码剩下</strong>
                        <p>你应该<el-link type="primary">生成一组新的恢复码</el-link>
                        </p>
                    </div>
                </template>

                <template v-if="setting.isMachineRemembered">
                    <div>
                        <div style="padding:3px">
                            <el-button type="success" @click="forget2faBrowser">忘记这个浏览器</el-button>

                        </div>
                    </div>
                </template>

                <div class="col-12">
                    <el-button type="warning" @click="disable2fa()">禁用2FA</el-button>
                    <el-button type="danger" @click="resetRecoveryCode()">重置恢复码</el-button>
                </div>

            </template>
        </div>
        <div>
            <h3>身份验证器应用</h3>
            <div class="col-12">
                <template v-if="!setting.hasAuthenticator">
                    <div>
                        <el-button type="primary" @click="setup()">添加身份验证器应用</el-button>
                    </div>
                </template>
                <template v-else>
                    <div>
                        <el-button type="primary" @click="setup()">设置身份验证器应用</el-button>
                        <el-button type="danger" @click="reset()">重置身份验证器应用</el-button>
                    </div>
                </template>
            </div>

        </div>
    </div>
</template>

<script>
import { getTwofactorSetting, forget2faClient, diable2fa, resetAuthenticator } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            setting: {}
        };
    },
    computed: {},
    watch: {},
    methods: {
        setup() {
            this.$router.push('/enableAuthenticator')
        },
        async forget2faBrowser() {
            await forget2faClient()
            let res = await getTwofactorSetting()
            this.setting = res.data
        },
        async disable2fa() {
            await diable2fa();
            let res = await getTwofactorSetting()
            this.setting = res.data
        },
        resetRecoveryCode() {
            this.$confirm('如果您丢失了设备并且没有恢复码，您将无法访问自己的帐户。生成新的恢复码不会更改身份验证器应用程序中使用的密钥。 如果您希望更改身份验证器应用程序中使用的密钥，则应重置身份验证器密钥。', '警告', {
                confirmButtonText: '生成恢复码',
                cancelButtonText: '取消',
                type: 'warning'
            }).then(() => {
                this.$router.push('/showRecoveryCodes')
            }).catch(() => {
                this.$message({
                    type: 'info',
                    message: '已取消充值恢复码'
                });
            });
        },
        reset() {
            resetAuthenticator().then(() =>
                this.$router.push('/enableAuthenticator'))
        }
    },

    async beforeMount() {
        let res = await getTwofactorSetting()
        this.setting = res.data
    },
}
</script>
<style scoped>
</style>