<template>
    <div id='logout'>
        <template v-if="$route.query.logoutId==null">
            是否要注销登录?
            <el-button type="primary" @click="yes">是</el-button>
        </template>
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
            if (res.route == 10) {
                this.$router.push({
                    path: '/loggedOut',
                    query: res.data
                })
            }
        }
    },
    async beforeMount() {
        if (this.$route.query.logoutId)
            await this.yes()
    }
}
</script>
<style scoped>
</style>