<template>
	<div id="scopeItem">
		<div class="scopeItem">
			<el-checkbox v-model="scope.checked" :disabled="scope.required">
				{{ scope.displayName }}

				<el-tooltip v-if="scope.description != null" :content="scope.description" placement="bottom" effect="light">
					<i v-if="scope.emphasize" class="el-icon-info"></i>
				</el-tooltip>
				<template v-if="scope.required">(必须)</template>
				<input v-if="scope.required" type="hidden" name="ScopesConsented" :value="scope.Value" />
			</el-checkbox>
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
