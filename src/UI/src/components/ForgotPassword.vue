<template>
    <div id='forgotPassword'>
        <div class="row">
            <h4>选择密码找回策略</h4>
            <div class="form-group">
                <el-radio v-model="useType" :label='0' @change="changeType">邮件</el-radio>
                <el-radio v-model="useType" :label='1' @change="changeType">用户名</el-radio>
            </div>
            <el-input v-model="uniqueMessage" />
        </div>

        <el-button type="primary">提交</el-button>
    </div>
</template>

<script>

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