<template>
    <div id="myOAuthApp">
        <div v-loading="isLoading">
            <Client />
        </div>
    </div>
</template>

<script>
import Client from '../../views/admin/Client/Client.vue'


export default {
    components: {
        Client
    },
    data() {
        return {
            isLoading: false,
        }
    },
    methods: {

    },

}
</script>
<style scoped>
</style>
