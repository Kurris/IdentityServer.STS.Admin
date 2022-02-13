<template>
    <div id='identityResourceForm'>
        <el-form ref="identityResourceForm" label-position="right" :model="form" label-width="150px">
            <el-form-item label="是否启用">
                <el-switch v-model="form.enabled" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="身份资源名">
                <el-input v-model="form.name"></el-input>
            </el-form-item>
            <el-form-item label="身份资源显示名称">
                <el-input v-model="form.displayName"></el-input>
            </el-form-item>
            <el-form-item label="描述">
                <el-input v-model="form.description"></el-input>
            </el-form-item>
            <el-form-item label="是否必备">
                <el-switch v-model="form.required" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="是否强调">
                <el-switch v-model="form.emphasize" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="显示在发现文档">
                <el-switch v-model="form.showInDiscoveryDocument" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="不可修改">
                <el-switch v-model="form.nonEditable" :active-value="true" :inactive-value="false" />
            </el-form-item>
            <el-form-item label="身份声明">
                <div class="userClaims">
                    <template v-for="item in form.userClaims">
                        <el-tag :key="item.type" closable @close="removeClaim(0,item.type)">
                            {{item.type}}
                        </el-tag>
                    </template>
                    <!-- newClaims -->
                    <template v-for="item in newClaims">
                        <el-tag :key="item.type" closable @close="removeClaim(1,item.type)">
                            {{item.type}}
                        </el-tag>
                    </template>

                    <el-select v-focus v-if="newClaimsVisible" v-model="newClaimsValue" filterable @keyup.enter.native="handleInputConfirm" allow-create default-first-option placeholder="请选择身份声明" @change="handleInputConfirm" @focus="getStandardClaims">

                        <el-option v-for="item in standardClaims" :key="item" :label="item" :value="item">
                        </el-option>
                    </el-select>
                    <el-button v-else class="button-new-tag" size="small" @click="showInput">
                        + 新声明
                    </el-button>
                </div>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="save">保存</el-button>
            </el-form-item>
        </el-form>
    </div>
</template>

<script>
import { getIdentityResourceById, saveIdentityResource, getStandardClaims } from '../../../net/admin.js'

export default {
    components: {},
    data() {
        return {
            currentId: '',
            standardClaims: [],
            form: {},
            newClaimsVisible: false,
            newClaimsValue: '',
            newClaims: [],
        };
    },
    methods: {
        load(id) {
            this.currentId = id
            getIdentityResourceById({ id: this.currentId }).then(res => {
                this.form = res.data
                console.log(this.form);
            })
        },
        async save() {
            let form = this.form
            this.newClaims.forEach(element => {
                form.userClaims.push(element)
            });
            this.newClaims = []
            await saveIdentityResource(form)
            this.$emit('onSave')
        },
        handleClose(claim) {
            this.newClaims.splice(this.newClaims.indexOf(claim), 1);
        },
        showInput() {
            this.newClaimsVisible = true;
        },
        handleInputConfirm() {
            let newClaimsValue = this.newClaimsValue;

            var exists = this.form.userClaims.find(x => x.type == newClaimsValue);
            if (!exists) {
                exists = this.newClaims.find(x => x.type == newClaimsValue);
            }

            if (exists) {
                this.$notify.warning('已存在' + newClaimsValue)
            } else {
                if (newClaimsValue) {
                    this.newClaims.push({
                        id: 0,
                        identityResourceId: this.currentId,
                        type: newClaimsValue
                    });
                }
            }
            console.log(this.newClaims);
            this.newClaimsVisible = false;
            this.newClaimsValue = '';
        },
        getStandardClaims() {
            if (!this.standardClaims || this.standardClaims.length == 0) {
                getStandardClaims().then(res => {
                    this.standardClaims = res.data
                })
            }
        },
        removeClaim(type, typeName) {
            if (type == 1) {
                let claim = this.newClaims.find(x => x.type == typeName);
                this.newClaims.splice(this.newClaims.indexOf(claim), 1);
                console.log(this.newClaims);
            } else if (type == 0) {
                let claim = this.form.userClaims.find(x => x.type == typeName);
                this.form.userClaims.splice(this.form.userClaims.indexOf(claim), 1);
                console.log(this.form.userClaims);
            }
        }
    }
}
</script>
<style scoped>
#identityResourceForm {
    text-align: left;
}

.el-tag + .el-tag {
    margin-left: 10px;
}
.button-new-tag {
    margin-left: 10px;
    height: 32px;
    line-height: 30px;
    padding-top: 0;
    padding-bottom: 0;
}
.input-new-tag {
    width: 90px;
    margin-left: 10px;
    vertical-align: bottom;
}
</style>