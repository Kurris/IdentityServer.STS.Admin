<template>
    <div id='roleUser'>
        <el-table :data="pagination.items" style="width: 100%" stripe>
            <el-table-column label="角色标识" prop="id" width="320px">
            </el-table-column>
            <el-table-column label="用户名称" prop="userName">
            </el-table-column>
            <el-table-column label="邮件地址" prop="email">
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>
    </div>
</template>

<script>
import { getRoleUserPage } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            pagination: {
                pageIndex: 1,
                pageSize: 20,
                totalCount: 0,
                totalPagee: 0,
                items: []
            },
            currentId: ''
        };
    },
    computed: {},
    watch: {},
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getRoleUserPage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getRoleUserPage();
        },
        getRoleUserPage() {
            this.pagination.roleId = this.currentId
            getRoleUserPage(this.pagination).then(res => {
                this.pagination = res.data
                console.log(this.pagination);
            })
        },
        load(id) {
            this.currentId = id
            this.getRoleUserPage()
        }
    },
}
</script>
<style scoped>
</style>