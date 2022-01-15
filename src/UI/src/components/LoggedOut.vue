<template>
    <div id='loggedOut'>
        你现在可以安全退出
        <template v-if="$route.query.postLogoutRedirectUri != null">
            <div>
                <el-link type="primary" :href="$route.query.postLogoutRedirectUri">返回</el-link>
                <span>{{$route.query.clientName}}</span>
            </div>
        </template>

        <template v-if="$route.query.signOutIframeUrl!=null">
            <div>
                <iframe width="0" height="0" :src="$route.query.signOutIframeUrl"></iframe>
            </div>
        </template>

    </div>
</template>

<script>

export default {
    components: {},
    data() {
        return {

        };
    },
    beforeCreate() {
        if (this.$route.query.automaticRedirectAfterSignOut === true) {
            window.location = this.$route.query.postLogoutRedirectUri
        }
    },
}
</script>
<style scoped>
</style>