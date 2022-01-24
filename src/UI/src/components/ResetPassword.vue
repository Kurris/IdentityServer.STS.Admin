<template>
    <div class='ResetPassword'>
        <h4></h4>
        <hr />
        邮件地址
        <el-input v-model="email"></el-input>
        新密码
        <el-input v-model="password"></el-input>
        确定密码:
        <el-input v-model="confirmPassword"></el-input>
        <el-button type="primary" @click="2">重置密码</el-button>
    </div>
</template>

<script>
import { resetPassword } from "../net/api.js";

export default {
    components: {},
    data() {
        return {
            confirmPassword: '',
            password: '',
            email: '',
            code: $route.query.code
        };
    },

    methods: {
        async resetPassword() {
            await resetPassword({
                code,
                email,
                password,
                confirmPassword
            });
        }
    }
}
</script>
<style scoped>
</style>