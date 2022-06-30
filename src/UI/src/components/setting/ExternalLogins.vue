<template>
	<div id="externalLogins">
		<h1>外部登录关联(External Logins)</h1>
		<el-divider></el-divider>
		<template v-if="model != null">
			<template v-if="model.otherLogins.length > 0">
				<h4>添加其他可用的外部登录</h4>
				<template v-for="(item, index) in model.otherLogins">
					<!-- <el-button  type="success" >{{  }}</el-button> -->
					<AuthorizeItem divider :title="item.displayName" :key="index">
						<ExternalAvatar :loginProvider="item.name" :size="32" slot="img" />
						<template slot="operation" v-if="model.ableRemove">
							<el-button type="primary" @click="linkExternalLogin(item.name)">关联账号</el-button>
						</template>
					</AuthorizeItem>
				</template>
			</template>

			<template v-if="model.currentLogins.length">
				<h4>已绑定的外部登录</h4>
				<template v-for="(item, index) in model.currentLogins">
					<AuthorizeItem divider :title="item.providerDisplayName" :key="index">
						<ExternalAvatar :loginProvider="item.loginProvider" :size="32" slot="img" />
						<template slot="operation" v-if="model.ableRemove">
							<el-button type="danger" plain @click="removeLogin(item.loginProvider, item.providerKey)">解除关联</el-button>
						</template>
					</AuthorizeItem>
				</template>
			</template>
		</template>
	</div>
</template>

<script>
import { getExternalLogins, deleteExternalLogin } from '../../net/api.js'
import AuthorizeItem from '../AuthorizeItem.vue'
import ExternalAvatar from '../ExternalAvatar.vue'

export default {
	components: {
		AuthorizeItem,
		ExternalAvatar,
	},
	data() {
		return {
			model: null,
		}
	},
	computed: {},
	watch: {},
	methods: {
		linkExternalLogin(provider) {
			let url = 'http://localhost:5000/api/manager/linkLogin'

			document.write('<form action=' + url + " method=post name=form1 style='display:none'>")
			document.write("<input type=hidden name=provider value='" + provider + "'/>")
			document.write('</form>')
			document.form1.submit()
		},
		async removeLogin(provider, key) {
			await deleteExternalLogin({
				loginProvider: provider,
				providerKey: key,
			})
			let res = await getExternalLogins()
			this.model = res.data
		},
	},
	async beforeMount() {
		let res = await getExternalLogins()
		this.model = res.data
	},
}
</script>
<style scoped></style>
