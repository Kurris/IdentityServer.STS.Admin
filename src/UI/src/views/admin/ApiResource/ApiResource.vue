<template>
    <div id='apiResource'>
        <div style="text-align:left">
            <el-button type="primary" @click="addApiResource">添加新的api资源</el-button>
        </div>
        <el-table :data="pagination.items" style="width: 100%" stripe>
            <el-table-column type="expand">
                <template slot-scope="props">
                    <el-form label-position="left">
                        <el-form-item label="身份声明">
                            <template v-for="item in props.row.userClaims">
                                <el-tag :key="item.type">{{item.type}}</el-tag>
                            </template>
                        </el-form-item>
                    </el-form>
                </template>
            </el-table-column>
            <el-table-column label="是否启用" prop="enabled">
                <template slot-scope="scope">
                    <el-tag v-if="scope.row.enabled" size="medium">是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="api资源名" prop="name">
            </el-table-column>
            <el-table-column label="api资源显示名称" prop="displayName">
            </el-table-column>
            <el-table-column label="允许access_token使用的签名算法" prop="allowedAccessTokenSigningAlgorithms">
            </el-table-column>
            <el-table-column label="描述" prop="description">
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
            <el-table-column label="上一次访问" prop="lastAccessed">
            </el-table-column>
            <el-table-column label="操作" fixed='right'>
                <template slot-scope="scope">
                    <el-button size="mini" @click="editApiResource(scope.row.id)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>

        <el-drawer title="api资源管理" :visible.sync="apiResourceDrawer" :with-header="true" :size="800">
            <ApiResourceForm ref="ApiResourceForm" @onSave="afterSave" />
        </el-drawer>
    </div>
</template>

<script>

import { getApiResourcePage } from '../../../net/admin.js'
import ApiResourceForm from './ApiResourceForm'

export default {
    components: {
        ApiResourceForm
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
            apiResourceDrawer: false
        };
    },
    computed: {},
    watch: {},
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getApiResourcePage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getApiResourcePage();
        },
        getApiResourcePage() {
            getApiResourcePage(this.pagination).then(res => {
                this.pagination = res.data
                console.log(this.pagination);
            })
        },
        editApiResource(id) {
            this.currentId = id
            this.apiResourceDrawer = true
            this.$nextTick(() => {
                this.$refs["ApiResourceForm"].load(this.currentId)
            })
        },
        afterSave() {
            this.apiResourceDrawer = false
            this.getApiResourcePage()
        },
        addApiResource() {
            this.apiResourceDrawer = true
        }
    },
    beforeMount() {
        this.getApiResourcePage()
    }
}
</script>
<style scoped>
</style>