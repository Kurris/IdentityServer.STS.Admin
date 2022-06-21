<template>
	<div class="enableAuthenticator">
		<el-dialog title="配置身份验证器应用,要使用身份验证器应用程序，请执行以下步骤:" :visible.sync="authenticatorVisible" :before-close="handleClose" top="10vh">
			<el-steps direction="vertical" :active="3" finish-status="process" space="5vh">
				<el-step>
					<template slot="icon">
						<i class="el-icon-download"></i>
					</template>
					<template slot="title">
						下载双重身份验证器应用程序，如适用于
						<a href="https://go.microsoft.com/fwlink/?Linkid=825071">Windows Phone</a>, <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a>,
						<a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a>
					</template>
				</el-step>
				<el-step>
					<template slot="icon">
						<i class="el-icon-s-grid"></i>
					</template>
					<template slot="title">
						在您的双重身份验证器应用程序中扫描二维码或输入此密钥,空格和大小写不影响。<kbd>{{ setting.sharedKey }}</kbd>
						<template v-if="setting.authenticatorUri != null">
							<qriously :value="setting.authenticatorUri" :size="300" />
						</template>
						<!-- {{ setting.authenticatorUri }} -->
					</template>
				</el-step>
				<el-step>
					<template slot="icon">
						<i class="el-icon-check"></i>
					</template>
					<template slot="title">
						<span> 扫描完二维码或输入上述密钥后，您的双因素身份验证应用程序将为您提供唯一的代码。 在下面的确认框中输入代码 </span>
						<p>
							<el-input v-model="setting.code" maxlength="6" @keyup.enter.native="verify" placeholder="身份验证码" autofocus />
						</p>
					</template>
				</el-step>
			</el-steps>
			<br />
			<el-button type="primary" @click="verify" :loading="isLoading" style="width: 100%">验证并确认</el-button>
		</el-dialog>
	</div>
</template>

<script>
import { getAuthenticatorSetting, verifyAuthode, enable2Fa } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			setting: {},
			isLoading: false,
		}
	},
	props: {
		authenticatorVisible: {
			type: Boolean,
			required: true,
		},
	},
	methods: {
		async verify() {
			this.isLoading = true
			await verifyAuthode(this.setting).finally(() => {
				this.isLoading = false
			})

			await enable2Fa({
				enable: true,
			})

			this.handleClose()
		},
		handleClose() {
			this.$emit('close')
		},
	},
	watch: {
		authenticatorVisible(val) {
			if (val) {
				getAuthenticatorSetting().then(res => {
					if (res != null) {
						this.setting = res.data
					}
				})
			}
		},
	},
}
</script>
<style scoped>
kbd {
	-webkit-border-radius: 4px;
	-moz-border-radius: 4px;
	-o-border-radius: 4px;
	-khtml-border-radius: 4px;
	border-radius: 3px;
	border-style: outset;
	border: 1px solid #aaa;
	background: #fafafa;
	padding: 0px 3px 1px 3px;
	margin: 0px 0px 0px 0px;
	vertical-align: baseline;
	line-height: 1.8em;
	white-space: nowrap;
}

>>> .el-dialog__header {
	border-bottom: 1px solid #dcdfe6;
}
</style>
