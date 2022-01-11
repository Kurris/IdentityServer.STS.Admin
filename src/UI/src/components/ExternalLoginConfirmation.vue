<template>
    <div id='externalLoginConfirmation'>
        <el-form ref="form" :model="form" label-width="80px">
            <el-form-item label="用户名称">
                <el-input v-model="form.userName"></el-input>
            </el-form-item>
            <el-form-item label="邮件">
                <el-input v-model="form.email"></el-input>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="externalRegister()">注册</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>

<script>
import { externalRegister } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            form: {
                email: this.$route.query.email,
                userName: this.$route.query.userName
            }
        };
    },
    computed: {},
    watch: {},
    methods: {
        async externalRegister() {
            await externalRegister({
                email: this.form.email,
                userName: this.form.userName
            })
        }
    },
    //生命周期 - 创建完成（可以访问当前this实例）
    created() {

    },
    //生命周期 - 挂载完成（可以访问DOM元素）
    mounted() {

    },
    beforeCreate() { }, //生命周期 - 创建之前
    beforeMount() { }, //生命周期 - 挂载之前
    beforeUpdate() { }, //生命周期 - 更新之前
    updated() { }, //生命周期 - 更新之后
    beforeDestroy() { }, //生命周期 - 销毁之前
    destroyed() { }, //生命周期 - 销毁完成
    activated() { }, //如果页面有keep-alive缓存功能，这个函数会触发
}
</script>
<style scoped>
</style>