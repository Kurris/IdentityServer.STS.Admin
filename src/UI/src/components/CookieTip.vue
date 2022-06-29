<template>
	<div id="cookieTip">
		<span
			>为了让您有更好的交互体验,网站将会使用cookie来保持登录,您的个人数据不会被获取和分析,除非您同意认证应用!
			<a href="https://www.cookiesandyou.com/">了解更多</a>
		</span>
		<div class="confirm" @click="confirm">
			<i class="el-icon-close" style="color: #005cc5; font-size: 20px; font-weight: bold"> </i>
		</div>
	</div>
</template>

<script>
export default {
	data() {
		return {
			show: this.isShow,
		}
	},
	methods: {
		confirm() {
			this.$emit('confirm')
		},
	},
}
</script>
<style scoped>
#cookieTip {
	display: flex;
	justify-content: space-around;
	align-items: center;
	height: 40px;
	width: 60%;
	font-size: 15px;
	color: #949699;
}

.confirm {
	display: flex;
	justify-content: center;
	align-items: center;
	color: black;
}

.confirm:hover {
	cursor: pointer;
}
</style>
