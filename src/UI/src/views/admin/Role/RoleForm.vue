<template>
    <div id='roleForm'>
        <el-form ref="apiResourceForm" label-position="right" :model="form" label-width="150px">
            <el-form-item label="名称">
                <el-input v-model="form.name"></el-input>
            </el-form-item>
            <el-form-item label="规范化名称">
                <el-input v-model="form.normalizedName"></el-input>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="save">保存</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>

<script>
import { getRoleById, saveRole } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            form: {}
        };
    },
    computed: {},
    watch: {},
    methods: {
        load(id) {
            getRoleById({ id }).then(res => {
                this.form = res.data
            })
        },
        async save() {
            await saveRole(this.form)
            this.$emit('onSave')
        }
    },
}
</script>
<style scoped>
#roleForm {
    text-align: left;
}
</style>