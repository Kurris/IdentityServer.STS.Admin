<template>
    <div id='logout'>
        是否要注销登录?
        <el-button type="primary" @click="yes">是</el-button>
    </div>
</template>

<script>

import { loggedOut } from '../net/api.js'

export default {
    components: {},
    data() {
        return {

        };
    },
    computed: {},
    watch: {},
    methods: {
        async yes() {
            let logoutId = this.$route.query.logoutId

            let res = await loggedOut({ logoutId })
            console.log(res);
            if (res.route == 10) {
                this.$router.push({
                    path: '/loggedOut',
                    query: res.data
                })
            }
        }
    },
}
</script>
<style scoped>
</style>