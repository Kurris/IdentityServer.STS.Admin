<template>
    <div id='forgotPassword'>
        <div class="row">
            <h4>选择密码找回策略</h4>
            <div class="form-group">

                <el-radio v-model="useType" :label='0' @change="changeType">用户名</el-radio>
                <el-radio v-model="useType" :label='1' @change="changeType">邮件</el-radio>
            </div>
            <el-input v-model="uniqueMessage" />
        </div>

        <el-button type="primary" @click="commit()">提交</el-button>
    </div>
</template>

<script>
import { forgetPasswordAndSendEmail } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            useType: 0,
            uniqueMessage: ''
        };
    },
    computed: {},
    watch: {},
    methods: {
        changeType() {
            this.uniqueMessage = ''
        },
        async commit() {
            await forgetPasswordAndSendEmail({
                policy: useType,
                content: uniqueMessage
            })
        }
    }
}
</script>
<style scoped>
</style>