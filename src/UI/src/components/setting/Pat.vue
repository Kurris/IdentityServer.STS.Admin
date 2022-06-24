<template>
	<div id="pat">
		<p>
			<el-alert show-icon :closable="false" title="如何使用? curl -i -H 'Authorization:Bearer YourToken' http://host:port/api/weatherForecast"></el-alert>
		</p>
		<div v-loading="isLoading">
			<div>
				<div style="display: flex">
					<el-select v-model="value" placeholder="请选择">
						<el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value"> </el-option>
					</el-select>
					<el-input v-model="description"></el-input>
					<el-button @click="create">创建token</el-button>
				</div>
				<p>
					<el-alert v-if="token != null" type="success" show-icon :title="token" description="请您妥善保管以上token, 这将不会再次显示"></el-alert>
				</p>
				<template v-for="item in pats">
					<el-card :key="item.key" shadow="hover" style="font-size: 15px">
						<div>
							<div style="display: flex; justify-content: space-between">
								<span style="font-weight: bold">
									{{ item.description }}
								</span>
								<el-button type="danger" plain @click="removePat(item.key, item.description)">撤销令牌</el-button>
							</div>
							<template v-if="item.isPermanent">
								<i class="el-icon-warning-outline" style="color: #e5bc7a; margin-right: 5px"> </i>
								<el-link style="font-size: 10px" type="warning" :underline="false">此 token 永久不失效</el-link>
							</template>
							<template v-else>
								<div style="font-size: 10px">
									<span><b>创建于:</b>{{ item.createTime }}</span>
									<span><b>将在:</b>{{ item.expiredTime }}<b>过期</b></span>
								</div>
							</template>
						</div>
					</el-card>
				</template>
				<p style="font-size: 10px; color: #596069">
					个人令牌是OAuth2.0中的reference token的实现,能够适用于无需账号密码登录的场景,例如在命令行脚本,测试期间生成长周期令牌,并且能够替代Bearer Token的使用.
					<el-link
						type="primary"
						style="font-size: 10px; margin-bottom: 2.5px"
						:underline="false"
						href="https://docs.github.com/cn/rest/overview/other-authentication-methods#basic-authentication"
						>详细查看</el-link
					>
				</p>
			</div>
		</div>
	</div>
</template>

<script>
import { createPAT, getAllPats, removePat } from '../../net/api'
export default {
	data() {
		return {
			setting: {},
			token: null,
			options: [
				{
					value: 3600,
					label: '一小时',
				},
				{
					value: 86400,
					label: '1天',
				},
				{
					value: 604800,
					label: '7天',
				},
				{
					value: 2592000,
					label: '1个月',
				},
				{
					value: 7776000,
					label: '3个月',
				},
				{
					value: 15552000,
					label: '6个月',
				},
				{
					value: null,
					label: '永久',
				},
			],
			value: 3600,
			description: null,
			pats: [],
			isLoading: false,
		}
	},
	methods: {
		async create() {
			let res = await createPAT({
				lifeTime: this.value,
				description: this.description,
			})
			this.token = res.data
			await this.refresh()
		},
		async removePat(key, description) {
			this.$confirm('是否确定撤销 ' + description + ' 的访问权限', '警告', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			}).then(async () => {
				await removePat({ key })
				await this.refresh()
			})
		},
		async refresh() {
			this.isLoading = true
			let res = await getAllPats().finally(() => {
				this.isLoading = false
			})
			this.pats = res.data
		},
	},
	async beforeMount() {
		await this.refresh()
	},
}
</script>
<style scoped>
.flex {
	display: flex;
	justify-content: space-between;
	align-items: center;
}

>>> .el-card__body {
	padding: 15px;
}
div.el-card + div.el-card {
	margin-top: 10px;
}

span + span {
	margin-left: 10px;
}
</style>
