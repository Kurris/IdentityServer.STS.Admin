<template>
    <div id="app">
        <div id="header">
            <el-button type="text" @click="goHome()">{{title}}</el-button>
            <el-button type="primary" v-show="status.isAdmin" @click="$router.push('/admin')">管理员</el-button>
            <el-button type="primary" v-show="status.isLogin" @click="logout">注销</el-button>
            <el-avatar icon="el-icon-user-solid"></el-avatar>
        </div>
        <router-view />
        <div id="footer">

            {{title}}
            <p>
                © 2020- {{new Date().getFullYear()}}
            </p>
        </div>
    </div>
</template>

<script>
import { getLoginStatus, logout } from './net/api.js'

export default {
    name: 'App',
    data() {
        return {
            title: 'Ligy.IdentityServer.STS',
            status: false
        }
    },
    methods: {
        goHome() {
            if (this.$route.path != '/home') {
                this.$nextTick(() => {
                    this.$router.push('home')
                })
            }
        },
        async logout() {
            var res = await logout()
            if (res.route == 9) {
                this.$router.push({
                    path: "/logout",
                    query: res.data
                })
            }
        }
    },
    async beforeMount() {
        let res = await getLoginStatus()
        if (res != null) {
            if (res.data.code == 401) {
                // this.$router.replace('/signIn')
            } else if (res.code == 200) {
                this.status = res.data
            }
        }

    }
}
</script>

<style>
#app {
    /* font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale; */
    text-align: center;
    /* color: #2c3e50; */

    overflow-y: hidden;
    overflow-x: hidden;
}
#header {
    margin-bottom: 60px;
    border-bottom: 0.5px;
    box-shadow: 0px 15px 10px -15px rgb(211, 209, 209);
    height: 70px;
}

#footer {
    margin-top: 60px;
}
</style>
