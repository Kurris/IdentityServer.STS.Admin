<template>
    <div id='device'>

    </div>
</template>

<script>

export default {
    components: {},
    data() {
        return {

        };
    },
    computed: {},
    watch: {},
    methods: {

    },
    beforeMount() {
        if (this.$route.query.userCode === undefined) {
            this.$router.push('/userCodeCapture')
        } else {
            this.$router.push({
                path: '/userCodeConfirmation',
                query: {
                    userCode: this.$route.query.userCode
                }
            })
        }
    }, //生命周期 - 挂载之前
}
</script>
<style scoped>
</style>