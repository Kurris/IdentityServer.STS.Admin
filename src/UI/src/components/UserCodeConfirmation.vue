<template>
	<div v-if="model != null">
		<div class="consent-container">
			<template v-if="model.clientLogoUrl != null">
				<div class="client-logo"><img :src="model.clientLogoUrl" /></div>
			</template>
			<h1>
				{{ model.clientName }}
				<small>正在请求您的许可</small>
			</h1>

			<template v-if="model.confirmUserCode">
				<div class="row">
					<div class="col-sm-8">
						<p>请确认授权请求引用代码: {{ model.userCode }}.</p>
					</div>
				</div>
			</template>

			<div>取消选中您不希望授予的权限</div>

			<template v-if="model.identityScopes != null && model.identityScopes.length > 0">
				<div class="col-sm-12">
					<div class="card mt-3">
						<h5 class="card-header">
							<i class="fa fa-user"></i>
							个人信息
						</h5>
						<div class="card-body">
							<ul class="list-group">
								<template v-for="(item, index) in model.identityScopes">
									<div :key="index">
										<ScopeItem :scope="item" />
									</div>
								</template>
							</ul>
						</div>
					</div>
				</div>
			</template>
			<template v-if="model.ApiScopes != null && model.ApiScopes.length > 0">
				<div class="col-sm-12">
					<div class="card mt-3">
						<h5 class="card-header">
							<i class="fa fa-lock"></i>
							应用访问
						</h5>
						<div class="card-body">
							<ul class="list-group">
								<template v-for="(item, index) in model.ApiScopes">
									<div :key="index">
										<ScopeItem :scope="item" />
									</div>
								</template>
							</ul>
						</div>
					</div>
				</div>
			</template>

			<template v-if="model.allowRememberConsent">
				<div>
					<div class="row m-4">
						<div class="col-sm-12">
							<div class="toggle-button__input">
								<el-switch v-model="model.rememberConsent" active-color="#13ce66" inactive-color="#ff4949"> </el-switch>
							</div>
							<div class="toggle-button__text">
								<strong>记住</strong>
							</div>
						</div>
					</div>
				</div>
			</template>

			<div class="row ml-4 mr-4">
				<div class="col-sm-9 mt-3">
					<el-button type="primary" autofocus @click="process(true)">许可</el-button>
					<el-button type="warning" @click="process(false)">不许可</el-button>
				</div>

				<div class="col-sm-3 mt-3">
					<template v-if="model.clientUrl != null">
						<a class="btn btn-outline-primary" target="_blank" :href="model.clientUrl">
							<i class="fa fa-info-circle"></i>
							<strong>{{ model.ClientName }}</strong>
						</a>
					</template>
				</div>
			</div>
		</div>
	</div>
</template>

<script>
import ScopeItem from './ScopeItem.vue'
import { getUserGetUserCodeConfirmationModel } from '../net/api.js'
import NProgress from 'nprogress'

export default {
	components: {
		ScopeItem,
	},
	data() {
		return {
			model: null,
		}
	},
	computed: {},
	watch: {},
	methods: {
		async process(allow) {
			NProgress.start()
			let url = 'https://identity.isawesome.cn/identity/api/device'

			document.write('<form action=' + url + " method=post name=form1 style='display:none'>")
			document.write(`<input type=hidden name=allow value=${allow}></input>`)
			document.write("<input type=hidden name=rememberConsent value='" + this.model.rememberConsent + "'/>")
			document.write("<input type=hidden name=userCode value='" + this.$route.query.userCode + "'/>")

			let idScopes = this.model.identityScopes.filter(x => x.checked)

			let scopeNames = idScopes.map(x => x.value)
			for (let i = 0; i < scopeNames.length; i++) {
				const element = scopeNames[i]
				document.write("<input type=hidden name=scopesConsented value='" + element + "'/>")
			}
			document.write('</form>')
			document.form1.submit()
		},
	},
	async beforeMount() {
		let res = await getUserGetUserCodeConfirmationModel({
			userCode: this.$route.query.userCode,
		})
		this.model = res.data
	},
}
</script>

<style scoped></style>
