<template>
	<div id="authorizeItem">
		<div :class="{ content: true, dropdown: isDropdown }" @click="dropdown">
			<div class="left">
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

			<div class="dropdown" v-if="isDropdown">
				<template v-if="dropdownStatus">
					<i class="el-icon-arrow-up"></i>
				</template>
				<template v-else>
					<i class="el-icon-arrow-down"></i>
				</template>
			</div>
		</div>
		<div id="dropdownContainer" :style="{ height: dropdownHeight }">
			<div id="slot">
				<slot name="dropdown"> </slot>
			</div>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			dropdownStatus: false,
		}
	},
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
		scopeLength: {
			type: Number,
			default: 1,
		},
	},
	methods: {
		dropdown() {
			if (this.isDropdown) {
				this.dropdownStatus = !this.dropdownStatus
			}
		},
	},
	computed: {
		dropdownHeight() {
			if (this.dropdownStatus) {
				return this.scopeLength * 40 + 'px'
			} else {
				return '0px'
			}
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

.dropdown {
	cursor: pointer;
}

#dropdownContainer {
	overflow-y: hidden;
	overflow-x: auto;
	margin-left: 50px;
	transition: all 0.3s ease;
}
</style>
