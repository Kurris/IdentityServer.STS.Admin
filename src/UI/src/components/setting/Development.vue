<template>
	<div id="development">
		<div>
			<div class="flex">
				<b style="font-size: 30px">开发者设置 (waiting)</b>
				<template> </template>
			</div>
			<el-divider></el-divider>

			<div style="display: flex">
				<el-select v-model="value" placeholder="请选择">
					<el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value"> </el-option>
				</el-select>
				<el-input v-model="description"></el-input>
				<el-button @click="create">创建token</el-button>
			</div>
			<el-alert v-if="token != null" :title="token"></el-alert>
			<template v-for="item in pats">
				<el-card :key="item.key" shadow="never">
					{{ item.description }}
					<template v-if="item.isPermanent"> 永久 </template>
					<template v-else>
						{{ item.createTime }}
						<p>
							{{ item.expiredTime }}
						</p>
					</template>

					<el-button type="warning" @click="removePat(item.key)">删除</el-button>
				</el-card>
			</template>
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
			value: null,
			description: null,
			pats: [],
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
		async removePat(key) {
			await removePat({ key })
			await this.refresh()
		},
		async refresh() {
			let res = await getAllPats()
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
</style>
