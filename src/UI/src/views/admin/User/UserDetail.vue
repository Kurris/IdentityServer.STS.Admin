<template>
    <div class=''>
        <el-descriptions class="margin-top" title="用户信息" :column="2" border>
            <!-- <template slot="extra">
                <el-button type="primary" size="small">保存</el-button>
            </template> -->
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-user"></i>
                    用户标识
                </template>
                {{user.id}}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-user"></i>
                    用户名
                </template>
                {{user.username}}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-mobile-phone"></i>
                    手机号
                </template>
                {{user.phone}}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    手机号码已确认
                </template>
                <el-switch v-model="user.isConfirmPhone" active-color="#13ce66" inactive-color="#ff4949">
                </el-switch>
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    Email
                </template>
                {{user.email}}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    Email已确认
                </template>
                <el-switch v-model="user.isConfirmEmail" active-color="#13ce66" inactive-color="#ff4949">
                </el-switch>
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    用户锁定已启用
                </template>
                <el-switch v-model="user.isEnableLockout" active-color="#13ce66" inactive-color="#ff4949">
                </el-switch>
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    锁定结束时间
                </template>
                <el-date-picker readonly v-model="user.lockoutLocalExpire" type="datetime" placeholder="选择日期时间" default-time="12:00:00">
                </el-date-picker>
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    二次验证已启用
                </template>
                <el-switch v-model="user.isEnableTwoFactor" active-color="#13ce66" inactive-color="#ff4949">
                </el-switch>
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    <i class="el-icon-connection"></i>
                    访问失败计数
                </template>
                <el-input-number v-model="user.loginFailCount" disabled />
            </el-descriptions-item>
        </el-descriptions>
    </div>
</template>

<script>
import { getUserById } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            user: {}
        };
    }, methods: {
        load(id) {
            getUserById({ id: id }).then(res => {
                this.user = res.data
            })
        }
    },
}
</script>
<style scoped>
</style>