<template>
    <div class='userCodeCapture'>
        <el-input v-model="userCode" placeholder="请输入您设备上的代码" />
        <el-button type="primary">提交</el-button>
    </div>
</template>

<script>

export default {
    components: {},
    data() {
        return {
            userCode: ''
        };
    },
    computed: {},
    watch: {},
    methods: {
        subbmi() {
            this.$router.push({
                paht: '/userCodeConfirmation',
                query: {
                    userCode: this.userCode
                }
            })
        }
    },
    //生命周期 - 创建完成（可以访问当前this实例）
    created() {

    }
}
</script>
<style scoped>
</style>