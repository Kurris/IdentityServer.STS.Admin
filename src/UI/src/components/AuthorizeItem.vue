<template>
	<div id="authorizeItem" class="authorizeItem">
		<div class="content">
			<div class="icon">
				<slot name="img"></slot>
			</div>
			<div class="text">
				<span style="font-weight: bold">
					{{ title }}
				</span>
				<slot></slot>
				<br />
				<span style="font-size: 10px; color: #91969b">
					{{ description }}
				</span>
			</div>
		</div>
		<div>
			<i class="el-icon-arrow-down"></i>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		iconSrc: {
			type: String,
		},
		title: {
			tyoe: String,
		},
		description: {
			tyoe: String,
		},
		isDropdown: {
			tyoe: Boolean,
			default: false,
		},
	},
}
</script>
<style scoped>
.content {
	display: flex;
	font-size: 14px;
}

.text {
	margin-left: 10px;
	line-height: 20px;
}

.authorizeItem {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
</style>
