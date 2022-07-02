<template>
	<div id="authorizeItem">
		<div class="content">
			<div class="left">
				<div>
					<slot name="img"></slot>
				</div>
				<div class="text">
					<span style="font-weight: bold">
						{{ title }}
					</span>
					<slot></slot>
					<br />
					<span v-if="description" style="font-size: 10px; color: #91969b">
						{{ description }}
					</span>
					<span v-else style="font-size: 10px; color: #91969b">
						<slot name="description"></slot>
					</span>
				</div>
			</div>

			<div>
				<slot name="operation" />
			</div>
		</div>
		<div id="container">
			<div id="slot">
				<slot name="container"> </slot>
			</div>
		</div>

		<el-divider v-if="divider" style="margin: 0"></el-divider>
	</div>
</template>

<script>
export default {
	props: {
		title: {
			tyoe: String,
		},
		description: {
			tyoe: String,
		},
		divider: {
			type: Boolean,
			default: false,
		},
	},
}
</script>
<style scoped>
.content {
	display: flex;
	justify-content: space-between;
	font-size: 14px;
	margin-bottom: 20px;
}
.left {
	display: flex;
}
.text {
	margin-left: 10px;
	line-height: 20px;
}

#container {
	overflow-y: hidden;
	overflow-x: auto;
	margin-left: 50px;
	margin-bottom: 8px;
}

.el-divider--horizontal {
	margin: 0px;
	margin-bottom: 24px;
}
</style>
