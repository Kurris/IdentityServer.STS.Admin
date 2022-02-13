<template>
    <div id='role'>
        <div style="text-align:left">
            <el-button type="primary" @click="addRole">添加新的角色</el-button>
        </div>
        <el-table :data="pagination.items" style="width: 100%" stripe>
            <el-table-column label="角色标识" prop="id" width="320px">
            </el-table-column>
            <el-table-column label="名称" prop="name">
            </el-table-column>
            <el-table-column label="规范化名称" prop="normalizedName">
            </el-table-column>
            <el-table-column label="并发性戳" prop="concurrencyStamp">
            </el-table-column>
            <el-table-column label="操作" fixed='right'>
                <template slot-scope="scope">
                    <el-button size="mini" @click="showRoleUsers(scope.row.id)">用户</el-button>
                    <el-button size="mini" @click="editRole(scope.row.id)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>

        <el-drawer title="角色管理" :visible.sync="roleDrawer" :with-header="true" :size="800">
            <RoleForm ref="RoleForm" @onSave="afterSave" />
        </el-drawer>

        <el-drawer title="角色用户管理" :visible.sync="roleUserDrawer" :with-header="true" :size="800">
            <RoleUser ref="RoleUser" />
        </el-drawer>
    </div>
</template>

<script>
import { getRolePage } from '../../../net/admin.js'
import RoleForm from './RoleForm.vue'
import RoleUser from './RoleUser.vue'

export default {
    components: {
        RoleForm, RoleUser
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
            roleDrawer: false,
            roleUserDrawer: false,
        };
    },
    computed: {},
    watch: {},
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getRolePage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getRolePage();
        },
        getRolePage() {
            getRolePage(this.pagination).then(res => {
                this.pagination = res.data
                console.log(this.pagination);
            })
        },
        addRole() {
            this.roleDrawer = true;
        },
        showRoleUsers(id) {
            this.roleUserDrawer = true
            this.$nextTick(() => {
                this.$refs["RoleUser"].load(id)
            })
        },
        editRole(id) {
            this.currentId = id
            this.roleDrawer = true
            this.$nextTick(() => {
                this.$refs["RoleForm"].load(this.currentId)
            })
        },
        afterSave() {
            this.roleDrawer = false
            this.getRolePage();
        }
    },
    beforeMount() {
        this.getRolePage()
    },
}
</script>
<style scoped>
</style>