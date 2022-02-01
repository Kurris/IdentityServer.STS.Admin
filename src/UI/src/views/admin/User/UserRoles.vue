<template>
    <div id='userRoles'>
        <el-card>
            <div slot="header">
                <span>用户角色</span>
            </div>
            <el-select v-model="selectRole" placeholder="请选择">
                <el-option v-for="item in roles" :key="item.id" :label="item.name" :value="item.id" :disabled="selectDisabled(item.id)">
                </el-option>
            </el-select>
            <el-button :disabled="selectRole==null" style="margin-left:10px" type="primary">确认</el-button>
        </el-card>
        <el-card style="margin-top:10px">
            <div slot="header">
                <span>角色</span>
            </div>
            <el-tag style="margin-left:10px" v-for="item in userRoles" :key="item.id" type="primary" effect="dark">
                {{ item.name }}
            </el-tag>
            <el-empty v-if=" !userRoles || userRoles.length==0" :image-size="200"></el-empty>
        </el-card>
    </div>
</template>

<script>
import { getUserRoles, getRoles } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            selectRole: null,
            roles: [],
            userRoles: [],
        };
    },
    methods: {
        load(id) {
            getUserRoles({ id: id }).then(res => {
                this.userRoles = res.data

                getRoles().then(res => {
                    this.roles = res.data

                    this.$nextTick(() => {
                        if (this.roles && this.roles.length > 0) {

                            let item = this.roles.filter(x => !x.disabled)
                            if (item) {
                                this.selectRole = item.id
                            }
                        }
                    })
                })
            })
        },
        selectDisabled(id) {
            var item = this.userRoles.filter(x => x.id == id);
            return item.length != 0
        }
    },

}
</script>
<style scoped>
.el-card {
    text-align: left;
}
</style>