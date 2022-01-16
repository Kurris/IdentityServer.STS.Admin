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
                在您的双因素验证器应用程序中扫描二维码或输入此密钥<kbd>123213131dsczcxzc</kbd>空格和大小写无关紧要。
                <qriously :value="qrcode" :size="300" style="padding:6px" />

            </li>

            <li>
                扫描完二维码或输入上述密钥后，您的双因素身份验证应用程序将为您提供唯一的代码。 在下面的确认框中输入代码
                <p>验证码:</p>
                <el-input />
                <el-button type="primary">验证</el-button>
            </li>
        </ol>
    </div>
</template>

<script>

export default {
    components: {},
    data() {
        return {
            qrcode: '231'
        };
    },
    computed: {},
    watch: {},
    methods: {

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