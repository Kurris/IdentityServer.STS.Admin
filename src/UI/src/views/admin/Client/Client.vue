<template>
    <div id='client'>
        <div style="text-align:left">
            <el-button type="primary" @click="addClient">添加新的客户端</el-button>
        </div>
        <el-table :data="pagination.items" style="width: 100%" stripe>
            <el-table-column label="客户端标识" prop="clientId">
            </el-table-column>
            <el-table-column label="客户端名称" prop="clientName">
            </el-table-column>
            <el-table-column label="操作" fixed='right'>
                <template slot-scope="scope">
                    <el-button size="mini" @click="editClient(scope.row.id)">编辑</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination background @size-change="handleSizeChange"
            @current-change="handleCurrentChange" :current-page="pagination.pageIndex"
            :page-sizes="[20, 40, 50, 100]" :page-size="pagination.pageSize"
            layout="total, sizes, prev, pager, next, jumper" :total="pagination.totalCount">
        </el-pagination>

        <el-drawer title="客户端管理" :visible.sync="clientDrawer" :with-header="true" :size="800">
            <div id="clientContainer">
                <template v-if="operateType==0">
                    <div>
                        <el-form ref="client" label-position="right" :model="form"
                            label-width="150px">
                            <el-form-item label="客户端标识">
                                <el-input v-model="form.clientId"></el-input>
                            </el-form-item>
                            <el-form-item label="客户端名称">
                                <el-input v-model="form.clientName"></el-input>
                            </el-form-item>
                            <el-form-item label="客户端类型">
                                <el-select v-model="clientType" placeholder="请选择">
                                    <el-option v-for="item in clientTypes" :key="item.id"
                                        :label="item.text" :value="item.id">
                                    </el-option>
                                </el-select>
                            </el-form-item>
                        </el-form>
                    </div>
                </template>
                <template v-else>
                    <div>
                        <el-tabs type="border-card" v-model="tabName" @tab-click="tabClick">
                            <el-tab-pane label="名称" name="name">

                            </el-tab-pane>
                            <el-tab-pane label="基本" name="claims">

                            </el-tab-pane>
                            <el-tab-pane label="认证/注销" name="identity">

                            </el-tab-pane>
                            <el-tab-pane label="令牌" name="token">

                            </el-tab-pane>
                            <el-tab-pane label="同意屏幕" name="consentScreen">

                            </el-tab-pane>
                            <el-tab-pane label="设备流程" name="deviceFlow">

                            </el-tab-pane>
                        </el-tabs>
                    </div>
                </template>

                <el-button type="primary" @click="save">保存</el-button>
            </div>
        </el-drawer>
    </div>
</template>

<script>
import { getClientPage, getClientType, saveClient } from '../../../net/admin.js'

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
            clientDrawer: false,
            clientTypes: [],
            clientType: 0,
            form: {},
            operateType: 0
        };
    },
    methods: {
        handleCurrentChange(page) {
            this.pagination.pageIndex = page
            this.getClientPage();
        },
        handleSizeChange(size) {
            this.pagination.pageSize = size
            this.getClientPage();
        },
        getClientPage() {
            getClientPage(this.pagination).then(res => {
                this.pagination = res.data
                console.log(this.pagination);
            })
        },
        addClient() {
            this.operateType = 0
            getClientType().then(res => {
                this.clientTypes = res.data
            })
            this.clientDrawer = true
        },
        editClient(id) {
            this.operateType = 1
            console.log(id);
            this.clientDrawer = true
        },
        async save() {
            await saveClient({
                clientId: this.form.clientId,
                clientName: this.form.clientName,
                clientType: this.clientType
            })
        }
    },
    beforeMount() {
        this.getClientPage()
    },
}
</script>
<style scoped>
#clientContainer {
    text-align: left;
}
</style>