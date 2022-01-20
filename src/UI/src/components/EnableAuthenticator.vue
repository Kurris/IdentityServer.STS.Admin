<template>
    <div class='enableAuthenticator'>
        配置身份验证器应用

        <p>要使用身份验证器应用程序，请执行以下步骤:</p>
        <ol>
            <li>
                <p>
                    下载双因素验证器应用程序，如适用于
                    <a href="https://go.microsoft.com/fwlink/?Linkid=825071">Windows Phone</a>,
                    <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a>,
                    <a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a>.
                    Google Authenticator 适用于
                    <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">Android</a>,
                    <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">iOS</a>.
                </p>

            </li>
            <li>
                在您的双因素验证器应用程序中扫描二维码或输入此密钥<kbd>{{setting.sharedKey}}</kbd>空格和大小写无关紧要。
                <template v-if="setting.authenticatorUri!=null">
                    <div>
                        <qriously :value="setting.authenticatorUri" :size="300" style="padding:6px" />
                        <div id="qrCodeData" v-html="setting.authenticatorUri"></div>
                    </div>
                </template>

            </li>

            <li>
                扫描完二维码或输入上述密钥后，您的双因素身份验证应用程序将为您提供唯一的代码。 在下面的确认框中输入代码
                <p>验证码:</p>
                <el-input v-model="setting.code" />
                <el-button type="primary" @click="verify()">验证</el-button>
            </li>
        </ol>
    </div>
</template>

<script>

import { getAuthenticatorSetting, verifyAuthode } from '../net/api.js'

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
        async verify() {
            let res = await verifyAuthode(this.setting)
            if (res.route == 13) {
                this.$router.push('/showRecoveryCodes')
            }
            else if (res.route == 14) {
                this.$router.push('/twoFactorAuthentication')
            }
        }
    },

    async beforeMount() {
        let res = await getAuthenticatorSetting()
        if (res != null) {
            this.setting = res.data
        }
    },

}
</script>
<style scoped>
</style>