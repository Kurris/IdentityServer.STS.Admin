<template>
	<div id="scopeItem">
		<div class="scopeItem">
			<el-checkbox v-model="scope.checked" :disabled="scope.required">
				{{ scope.displayName }}
				<i v-if="scope.emphasize" class="el-icon-info"></i>
				<template v-if="scope.required">(必须)</template>
				<input v-if="scope.required" type="hidden" name="ScopesConsented" :value="scope.Value" />
			</el-checkbox>
			<template v-if="scope.description != null">
				<div>{{ scope.description }}</div>
			</template>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		scope: {
			required: true,
		},
	},
}
</script>
<style scoped>
.scopeItem {
	margin-bottom: 10px;
	font-size: 12px;
}
</style>
