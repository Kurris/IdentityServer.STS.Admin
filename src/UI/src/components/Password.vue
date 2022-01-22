<template>
    <div id='password'>
        <template v-if="status">
            旧密码:<el-input v-model="form.oldPassword"></el-input>
        </template>

        <p v-if="!status">
            你还没有设置密码，设置一个密码，以便无需外部登录即可登录
        </p>

        新密码:<el-input v-model="form.newPassword"></el-input>
        确定密码:<el-input v-model="form.confirmPassword"></el-input>
        <el-button type="primary" @click="confirm()">{{status?'重置密码':'设置密码'}}</el-button>
    </div>
</template>

<script>
import { checkPassword, savePassword } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            form: {
                oldPassword: '',
                newPassword: '',
                confirmPassword: ''
            },
            status: false
        };
    },
    computed: {},
    watch: {},
    methods: {
        async confirm() {
            if (!this.status)
                this.form.oldPassword = null

            await savePassword(this.form)
            this.$router.push('/home')
        }
    },
    async beforeMount() {
        let res = await checkPassword()
        this.status = res.data;
    }
}
</script>
<style scoped>
</style>