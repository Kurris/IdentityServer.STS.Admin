<template>
    <div id='identityResource'>
        <el-table :data="pagination.items" style="width: 100%">
            <!-- <el-table-column label="身份资源标识" prop="id">
            </el-table-column> -->
            <el-table-column label="是否启用" prop="enabled">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.enabled" size="medium">是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="身份资源名" prop="name">
            </el-table-column>
            <el-table-column label="身份资源显示名称" prop="displayName">
            </el-table-column>
            <el-table-column label="描述" prop="description">
            </el-table-column>
            <el-table-column label="是否必备" prop="required">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.required" size="medium">是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="是否强调" prop="emphasize">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.emphasize" size="medium">是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="显示在发现文档" prop="showInDiscoveryDocument">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.showInDiscoveryDocument" size="medium">是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="不可修改" prop="nonEditable">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.nonEditable" size="medium">是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="操作" fixed='right'>
                <template slot-scope="scope">
                    <el-button size="mini" @click="editIdentityResource(scope.row.id)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>

        <el-drawer title="身份资源管理" :visible.sync="identityResourceDrawer" :with-header="true" :size="800">
            <IdentityResourceForm ref="IdentityResourceForm" @onSave="afterSave" />
        </el-drawer>
    </div>
</template>

<script>
import { getIdentityResourcePage } from '../../../net/admin.js'
import IdentityResourceForm from './IdentityResourceForm.vue'

export default {
    components: {
        IdentityResourceForm
    },
    data() {
        return {
            pagination: {
                pageIndex: 1,
                pageSize: 20,
                totalCount: 0,
                totalPagee: 0,
                items: []
            },
            currentId: '',
            identityResourceDrawer: false
        };
    },
    computed: {},
    watch: {},
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getIdentityResourcePage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getIdentityResourcePage();
        },
        getIdentityResourcePage() {
            getIdentityResourcePage(this.pagination).then(res => {
                this.pagination = res.data
            })
        }, editIdentityResource(id) {
            this.currentId = id
            this.identityResourceDrawer = true
            this.$nextTick(() => {
                this.$refs["IdentityResourceForm"].load(this.currentId)
            })
        },
        afterSave() {
            this.identityResourceDrawer = false
        }
    },
    beforeMount() {
        this.getIdentityResourcePage()
    },
}
</script>
<style scoped>
</style>