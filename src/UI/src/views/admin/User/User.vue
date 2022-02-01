<template>
    <div id='user'>
        <el-table :data="pagination.items" style="width: 100%">
            <el-table-column label="用户标识" prop="id">
            </el-table-column>
            <el-table-column label="登录名" prop="username">
            </el-table-column>
            <el-table-column label="邮件地址" prop="email">
            </el-table-column>
            <el-table-column label="操作">
                <template slot-scope="scope">
                    <el-button size="mini" @click="editUser(scope.row.id)">编辑</el-button>
                    <el-button size="mini" type="danger">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>

        <el-drawer title="用户信息管理" :visible.sync="userDrawer" :with-header="true" :size="800">
            <el-tabs type="border-card" v-model="tabName" @tab-click="tabClick">
                <el-tab-pane label="用户管理" name="detail">
                    <UserDetail ref="detail" />
                </el-tab-pane>
                <el-tab-pane label="声明" name="claims">
                    <UserClaims ref="claims" />
                </el-tab-pane>
                <el-tab-pane label="用户角色" name="roles">
                    <UserRoles ref="roles" />
                </el-tab-pane>
                <el-tab-pane label="外部提供程序" name="externalLogins">
                    <ExternalLogins ref="externalLogins" />
                </el-tab-pane>
            </el-tabs>
        </el-drawer>
    </div>
</template>

<script>
import { getUserPage } from '../../../net/admin.js'


import UserDetail from './UserDetail.vue'
import UserRoles from './UserRoles.vue'
import UserClaims from './UserClaims.vue'
import ExternalLogins from './ExternalLogins.vue'

export default {

    components: { UserDetail, UserRoles, UserClaims, ExternalLogins },
    data() {
        return {
            pagination: {
                pageIndex: 1,
                pageSize: 20,
                totalCount: 0,
                totalPagee: 0,
                items: []
            },
            userDrawer: false,
            currentId: '',
            tabName: 'detail'
        };
    },
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getUserPage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getUserPage();
        },
        editUser(id) {
            this.currentId = id
            this.userDrawer = true
            this.$nextTick(() => {
                this.$refs[this.tabName].load(this.currentId)
            })
        },
        tabClick(tab) {
            this.$nextTick(() => {
                this.$refs[tab.name].load(this.currentId)
            })
        },
        getUserPage() {
            getUserPage(this.pagination).then(res => {
                this.pagination = res.data
                if (this.pagination.items == null) {
                    this.pagination.items = []
                }
            })
        }

    },
    async beforeMount() {
        this.getUserPage();
    },
}
</script>
<style scoped>
</style>