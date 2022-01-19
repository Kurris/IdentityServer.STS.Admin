<template>
    <div id='signinWith2fa'>
        signinWith2fa
    </div>
</template>

<script>
import { checkTwoFactorAuthenticationUser } from '../net/api.js'
export default {
    components: {},
    data() {
        return {
            model: {}
        };
    },
    computed: {},
    watch: {},
    methods: {

    },
    async beforeMount() {
        let res = await checkTwoFactorAuthenticationUser()
        console.log(res);
        this.model = res.data
    },

}
</script>
<style scoped>
</style>