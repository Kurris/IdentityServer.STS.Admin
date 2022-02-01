<template>
    <div id='userClaims'>
        <el-card style="margin-bottom:10px">
            <div slot="header" style="text-align:left">
                编辑
            </div>
            <div>
                <span>声明</span>
                <el-select v-model="newClaim.type" filterable allow-create default-first-option placeholder="请选择用户声明或者创建声明">
                    <el-option v-for="(item,index) in standardClaims" :key="index" :label="item" :value="item">
                    </el-option>
                </el-select>
            </div>
            <div>
                <span>值</span>
                <el-input v-model="newClaim.value" />
            </div>
            <el-button type="primary">保存</el-button>
        </el-card>
        <el-card>
            <div slot="header" style="text-align:left">
                声明
            </div>
            <el-table :data="pagination.items" style="width: 100%">
                <el-table-column label="类型" prop="claimType">
                </el-table-column>
                <el-table-column label="值" prop="claimValue">
                </el-table-column>
            </el-table>
            <el-pagination background @size-change="handleSizeChange" @current-change="handleCurrentChange" :current-page="pagination.pageIndex" :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize" layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
            </el-pagination>
        </el-card>
    </div>
</template>

<script>
import { getUserClaimsPage, getStandardClaims } from '../../../net/admin.js'

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
            currentId: '',
            standardClaims: [],
            newClaim: {
                type: '',
                value: ''
            }
        };
    },
    computed: {},
    watch: {},
    methods: {
        load(id) {
            this.pagination.userId = id
            this.getUserClaimsPage()
            getStandardClaims().then(res => {
                this.standardClaims = res.data
            })
        },
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getUserClaimsPage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getUserClaimsPage();
        },
        getUserClaimsPage() {
            getUserClaimsPage(this.pagination).then(res => {
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
</style>