<template>
    <div class=''>

        <div>
            <h1>
                客户端应用访问
            </h1>

            <h2> 以下是您可以访问的应用程序列表以及他们有权访问的资源的名称。</h2>
        </div>

        <template v-if="grants.length == 0">
            没有授权应用
        </template>
        <template v-else>
            <template v-for="grant in grants">
                <div :key="grant.clientId">
                    <div class="row grant">
                        <div class="col-sm-2">
                            <template v-if="grant.ClientLogoUrl != null">
                                <img :src="grant.clientLogoUrl">
                            </template>
                        </div>
                        <div class="col-sm-8">
                            <div class="clientname">{{grant.clientName}}</div>
                            <div>
                                <span class="created">创建:</span> {{grant.created}}
                            </div>
                            <template v-if="grant.expires!=null">
                                <div>
                                    <span class="expires">过期:</span>{{grant.expires}}
                                </div>
                            </template>
                            <template v-if="grant.identityGrantNames.length!=0">
                                <div>
                                    <div>
                                        <div class="granttype">身份授权</div>
                                        <ul>
                                            <template v-for="(name,index) in grant.identityGrantNames">
                                                <li :key="index">{{name}}</li>
                                            </template>
                                        </ul>
                                    </div>
                                </div>
                            </template>
                            <template v-if="grant.apiGrantNames.length==0">
                                <div>
                                    <div>
                                        <div class="granttype">api授权</div>
                                        <ul>
                                            <template v-for="(name,index) in grant.apiGrantNames">
                                                <li :key="index">{{name}}</li>
                                            </template>
                                        </ul>
                                    </div>
                                </div>
                            </template>

                        </div>
                        <div class="col-sm-2">
                            <el-button @click="deleteById(grant.clientId)">撤销访问授权</el-button>
                        </div>
                    </div>
                </div>
            </template>
        </template>

    </div>
</template>

<script>
import { getGrants, deleteGrant } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            grants: []
        };
    },
    methods: {
        async deleteById(id) {
            await deleteGrant({
                clientId: id
            })

            let res = await getGrants()
            this.grants = res.data
        }
    },
    async beforeMount() {
        let res = await getGrants()
        this.grants = res.data
    },
}
</script>
<style scoped>
</style>