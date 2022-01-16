<template>
    <div id='scopeItem'>
        <li>
            <div>
                <el-switch v-model="scope.checked" active-color="#13ce66" inactive-color="#ff4949" :disabled="scope.required" />
            </div>
            <div>
                <template v-if="scope.required">
                    <input type="hidden" name="ScopesConsented" :value="scope.Value" />
                </template>
                <strong>{{scope.displayName}}</strong>

                <template v-if="scope.emphasize">
                    <!-- 星号标记 -->
                </template>
            </div>

            <template v-if="scope.required">
                <span><em>需要</em></span>
            </template>

            <template v-if="scope.description != null">
                <div class="consent-description">
                    <label for="scopes_@Model.Value">{{scope.description}}</label>
                </div>
            </template>

        </li>
    </div>
</template>

<script>

export default {
    props: {
        scope: {
            required: true
        }
    }
}
</script>
<style scoped>
</style>