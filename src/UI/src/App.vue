<template>
    <div id="app">
        <div id="header">
            <el-button type="text" @click="goHome()">{{title}}</el-button>
            <el-button type="primary" v-show="status">注销</el-button>
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
import { isAuthenticated } from './net/api.js'

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
            // /home
            if (this.$route.path != '/home') {
                this.$nextTick(() => {
                    this.$router.push('home')
                })
            }
        }
    },
    async beforeMount() {
        let res = await isAuthenticated()
        if (res.data.code == 401) {
            // this.$router.replace('/signIn')
        } else if (res.data.code == 200) {
            this.status = res.data.data
        }

    }
}
</script>

<style>
#app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;

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
