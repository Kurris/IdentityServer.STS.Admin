<template>
	<div>
		<div style="font-size: 14px">
			<span>{{ tipText }}</span>
			<el-link :type="type" :underline="false" style="padding-bottom: 3px" :href="href" @click="click">{{ hrefText }}</el-link>
		</div>
	</div>
</template>
<script>
export default {
	props: {
		type: {
			type: String,
			default: 'primary',
		},
		tipText: {
			required: true,
			type: String,
		},
		href: {
			type: String,
		},
		hrefText: {
			required: true,
			type: String,
		},
	},
	methods: {
		click() {
			this.$emit('click')
		},
	},
}
</script>
