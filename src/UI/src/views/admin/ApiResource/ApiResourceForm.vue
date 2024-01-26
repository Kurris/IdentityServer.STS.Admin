<template>
    <div id='apiResourceForm'>
        <el-form ref="apiResourceForm" label-position="right" :model="form" label-width="150px">
            <el-form-item label="是否启用">
                <el-switch v-model="form.enabled" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="身份资源名">
                <el-input v-model="form.name"></el-input>
            </el-form-item>
            <el-form-item label="身份资源显示名称">
                <el-input v-model="form.displayName"></el-input>
            </el-form-item>
            <el-form-item label="描述">
                <el-input v-model="form.description"></el-input>
            </el-form-item>
            <el-form-item label="显示在发现文档">
                <el-switch v-model="form.showInDiscoveryDocument" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="不可修改">
                <el-switch v-model="form.nonEditable" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="save">保存</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>

<script>
import { getApiResourceById, saveApiResource } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            form: {},
        };
    },
    computed: {},
    watch: {},
    methods: {
        load(id) {
            getApiResourceById({ id }).then(res => {
                this.form = res.data
            })
        },
        async save() {
            await saveApiResource(this.form)
            this.$emit('onSave')
        }
    },
}
</script>
<style scoped>
#apiResourceForm {
    text-align: left;
}
</style>