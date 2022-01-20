<template>
    <div id='showRecoveryCodes'>
        <template v-for="(item,index) in codes">
            <div :key="index">
                {{item}}
            </div>
        </template>
    </div>
</template>

<script>
import { getRecoveryCodes } from '../net/api.js'


export default {


    components: {},
    data() {
        return {
            codes: []
        };
    },
    computed: {},
    watch: {},
    methods: {

    },

    async beforeMount() {
        var res = await getRecoveryCodes()
        this.codes = res.data
    },

}
</script>
<style scoped>
</style>