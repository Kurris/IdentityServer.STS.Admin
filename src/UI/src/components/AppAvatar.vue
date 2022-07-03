<template>
    <div id="appAvatar">
        <el-avatar src="http://docs.identityserver.io/en/latest/_images/logo.png" :size="size"></el-avatar>
    </div>
</template>
<script>
export default {
    props: {
        size: {
            type: Number,
            default: 64,
        }
    }
}
</script>
