<template>
    <div id='externalLogins'>
        <el-table :data="pagination.items" style="width: 100%">
            <el-table-column label="登录提供程序" prop="loginProvider">
            </el-table-column>
            <el-table-column label="提供程序显示名称" prop="providerDisplayName">
            </el-table-column>
            <el-table-column label="提供程序密钥" prop="providerKey">
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>
    </div>
</template>

<script>
import { getUserProviderPage } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            pagination: {
                id: '',
                pageIndex: 1,
                pageSize: 20,
                totalCount: 0,
                totalPagee: 0,
                items: []
            },
            currentId: '',
        };
    },
    computed: {},
    watch: {},
    methods: {
        load(id) {
            this.pagination.id = id
            this.getUserProviderPage()
        },
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getUserProviderPage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getUserProviderPage();
        },
        getUserProviderPage() {
            getUserProviderPage(this.pagination).then(res => {
                this.pagination = res.data
                if (this.pagination.items == null) {
                    this.pagination.items = []
                }
            })
        }
    }
}
</script>
<style scoped>
#externalLogins {
    text-align: left;
}
</style>