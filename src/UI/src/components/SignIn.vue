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
                    <div class="button">
                        <button @click="signIn">Login</button>
                        <br>
                        <button style="margin-top:10px" @click="getDocument()">Discovery Document</button>
                        <br>
                        <button style="margin-top:10px">OAuth2.0</button>

                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'signIn',
    data() {
        return {
            form: {
                userName: '',
                password: '',
                remember: false,
            }
        }
    },
    methods: {
        signIn() {
            console.log(this.form.userName);
        },
        getDocument() {
            window.location = 'https://' + location.host + '/.well-known/openid-configuration'
        }
    },
}


</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
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
