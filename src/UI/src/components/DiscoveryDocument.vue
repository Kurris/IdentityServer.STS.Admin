<template>
    <div id='discoveryDocument'>
        <jsonView :data="discoveryDocument" />
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
        this.discoveryDocument = res
    },
}
</script>
<style scope>
#discoveryDocument {
    text-align: left;
}
</style>