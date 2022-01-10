<template>
    <div id='home'>
        <el-row justify="space-around" :gutter="20">
            <el-col :span="colSpan" v-show="isAuthenticated">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>持久授权</b></span>
                    </div>
                    <el-button type="">持久授权</el-button>
                </el-card>
            </el-col>
            <el-col :span="colSpan" v-show="!isAuthenticated">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>登录</b></span>
                    </div>
                    <el-button @click="$router.push('signIn')">登录</el-button>
                </el-card>
            </el-col>
            <el-col :span="colSpan">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>发现文档</b></span>
                    </div>
                    <el-button type="" @click="getDocument()">发现文档</el-button>
                </el-card>
            </el-col>
            <el-col :span="colSpan" v-show="isAuthenticated">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>我的个人资料</b></span>
                    </div>
                    <el-button type="">我的个人资料</el-button>
                </el-card>
            </el-col>

            <el-col :span="colSpan" v-show="isAuthenticated">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>我的个人数据</b></span>
                    </div>
                    <el-button type="">我的个人数据</el-button>
                </el-card>
            </el-col>

            <el-col :span="colSpan" v-show="isAuthenticated">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>双因素身份验证</b></span>
                    </div>
                    <el-button type="">双因素身份验证</el-button>
                </el-card>
            </el-col>

            <el-col :span="colSpan" v-show="isAuthenticated">
                <el-card class="box-card">
                    <div slot="header" class="clearfix">
                        <span><b>更改密码</b></span>
                    </div>
                    <el-button type="">更改密码</el-button>
                </el-card>
            </el-col>
        </el-row>
    </div>
</template>

<script>

import { isAuthenticated } from '../net/api.js'

export default {

    data() {
        return {
            colSpan: this.isAuthenticated ? 8 : 12,
            isAuthenticated: false
        }
    },
    components: {},
    methods: {
        getDocument() {
            this.$router.push("/discoveryDocument")
        },
    },
    async beforeMount() {
        let res = await isAuthenticated();
        console.log(res);
        if (res.code == 401) {
            // this.$router.replace('/signIn')
        } else if (res.code == 200) {
            this.isAuthenticated = res.data
        }
    }
}
</script>
<style scoped >
.el-card {
    width: auto;
}
</style>