<template>
    <div id='profile'>
        <el-avatar shape="square" :size="100" :src="profile.picture"></el-avatar>

        <div class="form-group">
            <label>用户名</label>
            <el-input v-model="profile.username" disabled />
        </div>
        <div class="form-group">
            <label>Email</label>
            <el-input v-model="profile.email" />
            <template v-if="!profile.isEmailConfirmed">
                <el-link type="primary" @click="sendVerificationEmail">发送验证邮件</el-link>
            </template>
        </div>
        <div class="form-group">
            <label>电话号码</label>
            <el-input v-model="profile.phoneNumber" />
        </div>

        <div class="form-group">
            <label>全名</label>
            <el-input v-model="profile.name" />
        </div>

        <div class="form-group">
            <label>网站 URL</label>
            <el-input v-model="profile.website" />
        </div>

        <div class="form-group">
            <label>个人资料 URL</label>
            <el-input v-model="profile.profile" />
        </div>

        <div class="form-group">
            <label>街道地址</label>
            <el-input v-model="profile.streetAddress" />
        </div>

        <div class="form-group">
            <label>城市</label>
            <el-input v-model="profile.locality" />
        </div>

        <div class="form-group">
            <label>地区</label>
            <el-input v-model="profile.region" />
        </div>

        <div class="form-group">
            <label>邮政编码</label>
            <el-input v-model="profile.postalCode" />
        </div>

        <div class="form-group">
            <label>国家</label>
            <el-input v-model="profile.country" />
        </div>

        <el-button type="primary" @click="saveProfile">保存</el-button>

    </div>

</template>

<script>
import { getProfile, saveProfile } from '../net/api.js'
export default {
    data() {
        return {
            profile: {}
        }
    },
    methods: {
        sendVerificationEmail() {

        },
        async saveProfile() {
            await saveProfile(this.profile);
        }
    },
    async beforeMount() {
        let res = await getProfile()
        this.profile = res.data;
    },
}
</script>
<style scoped>
</style>