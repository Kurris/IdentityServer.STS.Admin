<template>
    <div id='discoveryDocument'>
        <jsonView :data="discoveryDocument" :theme="one-dark" />
    </div>
</template>

<script>

import jsonView from 'vue-json-views'
import { getDocument } from '../net/api.js'
export default {
    data() {
        return {
            discoveryDocument: {}
        };
    },
    components: {
        jsonView
    },
    async beforeMount() {
        let res = await getDocument()
        this.discoveryDocument = res.data
    }, //生命周期 - 挂载之前
}
</script>
<style scoped>
.jsonView {
    left: 0%;
}
</style>