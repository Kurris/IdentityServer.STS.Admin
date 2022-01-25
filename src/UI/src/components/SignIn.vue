<template>
    <div class="container">
        <div class="welcome">
            <div class="pinkbox">
                <!-- 登录 -->
                <div class="signin">
                    <h1>Sign In</h1>
                    <input type="text" v-model="form.userName" placeholder="Username">
                    <input type="password" v-model="form.password" placeholder="Password">
                    <div class="checkbox">
                        <input type="checkbox" v-model="form.remember" id="remember" />
                        <label for="remember">Remember Me</label>
                    </div>
                    <div>
                        <button @click="login()">登录</button>
                    </div>
                    <div>
                        <button @click="cancel()">取消</button>
                    </div>
                    <div>
                        <el-link type="primary" @click="$router.push('/register')">注册</el-link>
                        <el-link type="primary" @click="$router.push('/forgotPassword')">忘记密码</el-link>
                    </div>
                    <div>
                        <span>OAuth2.0</span>
                        <template v-for="(item,i) in externalProviders">
                            <div :key="i">
                                <el-button @click="externalLogin(item.authenticationScheme)">{{item.displayName}}</el-button>
                            </div>
                        </template>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

import { signIn, checkLogin } from '../net/api.js'

import NProgress from 'nprogress'

export default {
    name: 'signIn',
    data() {
        return {
            form: {
                userName: '',
                password: '',
                remember: false,
            },
            externalProviders: []
        }
    },
    methods: {
        async login() {
            const returnUrl = this.$url.getValueFromQuery('ReturnUrl');
            const username = this.form.userName
            const password = this.form.password

            let response = await signIn({
                username,
                password,
                returnUrl,
                rememberLogin: this.form.remember,
                requestType: 'login'
            })

            if (response.route == 2) {
                this.$router.push('/home')
            } else if (response.route == 1) {
                window.location = response.data
            } else if (response.route == 4) {
                this.$router.push({
                    path: '/signinWith2fa',
                    query: {
                        rememberMe: response.data.rememberLogin,
                        returnUrl: response.data.returnUrl,
                    }
                })
            }
        },
        cancel() {
            let returnUrl = this.$url.getValueFromQuery('ReturnUrl')
            if (returnUrl === undefined) {
                this.$router.push('/home')
            } else {
                window.location.href = returnUrl
            }
        },
        async externalLogin(provider) {
            let returnUrl = this.$url.getValueFromQuery('ReturnUrl')

            if (returnUrl === undefined) {
                returnUrl = location.protocol + "//" + location.host + "/home"
            }
            NProgress.start()
            let url = "http://localhost:5000/api/authenticate/externalLogin"
            // window.location = url

            document.write("<form action=" + url + " method=post name=form1 style='display:none'>");
            document.write("<input type=hidden name=provider value='" + provider + "'/>");
            document.write("<input type=hidden name=returnUrl value='" + returnUrl + "'/>");
            document.write("</form>");
            document.form1.submit();
        }
    },
    beforeMount() {
        let returnUrl = this.$url.getValueFromQuery("ReturnUrl")
        checkLogin({ returnUrl: returnUrl })
            .then(res => {
                this.externalProviders = res.data.externalProviders
            })
    }
}


</script>

<style scoped>
body {
    background: #cbc0d3;
}

/* 容器的样式 */
.container {
    margin: auto;
    width: 650px;
    height: 550px;
    position: relative;
}

.welcome {
    background: #f6f6f6;
    width: 650px;
    height: 415px;
    position: absolute;
    top: 25%;
    border-radius: 5px;
    box-shadow: 5px 5px 5px rgba(0, 0, 0, 0.1);
}

.pinkbox {
    position: absolute;
    top: -10%;
    left: 45%;
    background: #dfe4cd;
    width: 320px;
    height: 500px;
    border-radius: 5px;
    box-shadow: 2px 0 10px rgba(0, 0, 0, 0.1);
    transition: all 0.5s ease-in-out;
    z-index: 2;
}

.nodisplay {
    display: none;
    transition: all 0.5s ease;
}

.leftbox,
.rightbox {
    position: absolute;
    width: 50%;
    transition: 1s all ease;
}

.leftbox {
    left: -2%;
}

.rightbox {
    right: -2%;
}

/* 字体和按钮的样式 */
h1 {
    font-family: "Open Sans", sans-serif;
    text-align: center;
    margin-top: 95px;
    text-transform: uppercase;
    color: #f6f6f6;
    font-size: 2em;
    letter-spacing: 8px;
}

.title {
    font-family: "Lora", serif;
    color: #8e9aaf;
    font-size: 1.8em;
    line-height: 1.1em;
    letter-spacing: 3px;
    text-align: center;
    font-weight: 300;
    margin-top: 20%;
}

.desc {
    margin-top: -8px;
}

.account {
    margin-top: 45%;
    font-size: 10px;
}

p {
    font-family: "Open Sans", sans-serif;
    font-size: 0.7em;
    letter-spacing: 2px;
    color: #8e9aaf;
    text-align: center;
}

span {
    color: #eac7cc;
}

.smaller {
    width: 130px;
    height: 130px;
    top: 48%;
    left: 30%;
    opacity: 0.9;
}

.button {
    padding: 6px;
    text-transform: uppercase;
    letter-spacing: 3px;
    font-size: 11px;
    margin: auto;
    outline: none;
    display: block;
    margin-top: 60px;
}

button:hover {
    cursor: pointer;
}

/* 表单样式 */
form {
    display: flex;
    align-items: center;
    flex-direction: column;
    padding-top: 7px;
}

input {
    width: 65%;
    border: none;
    /* border-bottom: 1px solid rgba(246, 246, 246, 0.5); */
    padding: 9px;
    font-weight: 100;
}

input::placeholder {
    /* color: #f6f6f6; */
    letter-spacing: 2px;
    font-size: 1em;
    font-weight: 100;
}

input:focus::placeholder {
    opacity: 0;
}

label {
    font-size: 0.8em;
    letter-spacing: 1px;
}

.checkbox {
    display: inline;
    white-space: nowrap;
    position: relative;
    left: -52px;
    top: 25px;
}

input[type="checkbox"] {
    width: 15px;
    background: #ce7d88;
}

.checkbox input[type="checkbox"]:checked + label {
    color: #ce7d88;
    transition: 0.5s all ease;
}
</style>
