<template>
	<div id="externalLogins">
		<template v-if="model != null">
			<template v-if="model.currentLogins.length">
				<h4>已注册的登录</h4>
				<table class="table">
					<tbody>
						<template v-for="(item, index) in model.currentLogins">
							<div :key="index">
								<tr>
									<td>{{ item.loginProvider }}</td>
									<td>
										<template v-if="model.ableRemove">
											<div>
												<el-button type="warning" @click="removeLogin(item.loginProvider, item.providerKey)">解除</el-button>
											</div>
										</template>
										&nbsp;
									</td>
								</tr>
							</div>
						</template>
					</tbody>
				</table>
			</template>

			<template v-if="model.otherLogins.length > 0">
				<div>
					<h4>添加其他服务以登录</h4>
					<hr />
					<form asp-action="LinkLogin" method="post" class="form-horizontal">
						<div id="socialLoginList">
							<template v-for="(item, index) in model.otherLogins">
								<p :key="index">
									<el-button type="primary" @click="linkExternalLogin(item.name)">{{ item.displayName }}</el-button>
								</p>
							</template>
						</div>
					</form>
				</div>
			</template>
		</template>
	</div>
</template>

<script>
import { getExternalLogins, deleteExternalLogin } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			model: null,
		}
	},
	computed: {},
	watch: {},
	methods: {
		linkExternalLogin(provider) {
			let url = 'http://192.168.1.4:5000/api/manager/linkLogin'

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
