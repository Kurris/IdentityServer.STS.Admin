<template>
    <div id='twoFactorAuthentication'>
        <div>
            <h1>双因素身份验证 (2FA)</h1>
            <template v-if="setting.is2faEnabled">
                <template v-if="setting.recoveryCodesLeft">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <strong>@Localizer["NoCodes"]</strong>
                            <p>@Localizer["YouMust"] <a asp-action="GenerateRecoveryCodes">@Localizer["GenerateNewCodes"]</a> @Localizer["BeforeLogin"]</p>
                        </div>
                    </div>
                </template>
                <template v-else-if="setting.recoveryCodesLeft==1">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <strong>@Localizer["OneCode"]</strong>
                            <p>@Localizer["YouCanGenerateCodes"] <a asp-action="GenerateRecoveryCodes">@Localizer["GenerateNewCodes"]</a></p>
                        </div>
                    </div>
                </template>
                <template v-else-if="setting.recoveryCodesLeft <= 3">

                    <div class="alert alert-warning">
                        <strong>@Localizer["YouHave"] @Model.RecoveryCodesLeft.ToString() @Localizer["RecoveryCodeLeft"]</strong>
                        <p>@Localizer["YouShould"] <a asp-action="GenerateRecoveryCodes">@Localizer["GenerateNewCodes"]</a></p>
                    </div>
                </template>

                <template v-if="setting.isMachineRemembered">
                    <div>
                        <div class="col-12 mb-3">
                            <form method="post" asp-controller="Manage" asp-action="ForgetTwoFactorClient">
                                <button type="submit" class="btn btn-info">@Localizer["ForgetBrowser"]</button>
                            </form>
                        </div>
                    </div>
                </template>

                <div class="col-12">
                    <a asp-action="Disable2faWarning" class="btn btn-dark">禁用 2FA</a>
                    <a asp-action="GenerateRecoveryCodesWarning" class="btn btn-danger">重置恢复码</a>
                </div>

            </template>
        </div>
        <div>
            <h3>身份验证器应用</h3>
            <div class="col-12">
                <template v-if="setting.hasAuthenticator">
                    <div>
                        <el-button type="primary" @click="setup()">添加身份验证器应用</el-button>
                    </div>
                </template>
                <template v-else>
                    <div>
                        <el-button type="primary" @click="setup()">设置身份验证器应用</el-button>
                        <el-button type="danger">重置身份验证器应用</el-button>
                    </div>
                </template>
            </div>

        </div>
    </div>
</template>

<script>
import { getTwofactorSetting } from '../net/api.js'

export default {
    components: {},
    data() {
        return {
            setting: {}
        };
    },
    computed: {},
    watch: {},
    methods: {
        setup() {
            this.$router.push('/enableAuthenticator')
        }
    },

    async beforeMount() {
        let res = await getTwofactorSetting()
        this.setting = res.data
    },
}
</script>
<style scoped>
</style>