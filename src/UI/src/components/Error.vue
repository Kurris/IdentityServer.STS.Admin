<template>
    <div id='error'>

        错误消息:{{error}}
        <br>
        ErrorId: {{$route.query.errorId}}
    </div>
</template>

<script>

import { getError } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            error: null
        };
    },
    computed: {},
    watch: {},
    methods: {

    },
    //生命周期 - 创建完成（可以访问当前this实例）
    created() {

    },
    //生命周期 - 挂载完成（可以访问DOM元素）
    async mounted() {
        this.error = await getError({ errorId: this.$route.query.errorId })

    },
    beforeCreate() { }, //生命周期 - 创建之前

}
</script>
<style scoped>
</style>