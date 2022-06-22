<template>
	<div id="grants" v-loading="isLoading">
		<h1>OAuth授权应用</h1>
		<el-divider></el-divider>
		<p style="font-size: 10px" v-if="!isLoading">
			您已授权 <b>{{ grants.length }}个</b> 应用能够访问你的账号
		</p>
		<div v-if="grants.length > 0">
			<template v-for="grant in grants">
				<AuthorizeItem style="margin-top: 20px" :key="grant.clientId" :description="'创建于' + grant.created + '	拥有者:'" :isDropdown="true" :scopeLength="4">
					<template slot="img">
						<el-avatar style="box-shadow: 1.2px 1.5px 4px 1px #f9f9fa" :src="grant.clientLogoUrl" :size="56" />
					</template>
					<el-link type="primary" :underline="false" @click.stop="goClient(grant.clientUrl)">{{ grant.clientName }}</el-link>
					<template slot="dropdown">
						<div style="font-size: 12px">
							<div style="display: flex">
								<div style="margin-right: 50px">创建于 {{ grant.created }}</div>
								<template v-if="grant.expires != null">
									<div>将在 {{ grant.expires }} 过期</div>
								</template>
							</div>
							<p v-if="grant.identityGrantNames.length != 0">
								<span>身份授权 : </span>
								<template v-for="(name, index) in grant.identityGrantNames">
									<el-tag type="" effect="plain" :key="index">
										{{ name }}
									</el-tag>
								</template>
							</p>
							<p v-if="grant.apiGrantNames.length != 0">
								<span class="granttype">API授权 : </span>
								<template v-for="(name, index) in grant.apiGrantNames">
									<el-tag type="" effect="plain" :key="index">
										{{ name }}
									</el-tag>
								</template>
							</p>
						</div>
						<p style="display: flex; justify-content: space-around">
							<el-button style="width: 40%" type="danger" @click="deleteById(grant.clientId, grant.clientName)">撤销访问授权</el-button>
							<el-button style="width: 40%" type="warning">报告滥用行为</el-button>
						</p>
					</template>
				</AuthorizeItem>
				<el-divider class="bottom" :key="grant.clientId"></el-divider>
			</template>
		</div>
	</div>
</template>

<script>
import { getGrants, deleteGrant } from '../../net/api.js'
import AuthorizeItem from '../AuthorizeItem.vue'

export default {
	components: {
		AuthorizeItem,
	},
	data() {
		return {
			grants: [],
			isLoading: false,
		}
	},
	methods: {
		async deleteById(id, name) {
			this.$confirm(`是否撤销 ${name} 访问权限?`, '警告', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			}).then(async () => {
				await deleteGrant({
					clientId: id,
				})

				await this.refresh()
			})
		},
		goClient(url) {
			window.open(url, '_blank')
		},
		async refresh() {
			this.isLoading = true
			let res = await getGrants().finally(() => {
				this.isLoading = false
			})
			this.grants = res.data
		},
	},
	async beforeMount() {
		await this.refresh()
	},
}
</script>
<style scoped>
.el-divider--horizontal.bottom {
	margin: unset;
}
</style>
