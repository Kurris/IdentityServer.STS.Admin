<template>
	<div class="externalAvatar">
		<div class="logo">
			<template v-if="loginProvider == 'GitHub'">
				<img v-if="size == 64" src="../assets/auth2logo/GitHub-64.png" />
				<img v-else src="../assets/auth2logo/GitHub-32.png" />
			</template>
			<template v-else-if="loginProvider == 'Weibo'">
				<img v-if="size == 64" src="../assets/auth2logo/WeiBo-64.png" />
				<img v-else src="../assets/auth2logo/WeiBo-32.png" />
			</template>
			<template v-else-if="loginProvider == 'Discord'">
				<img v-if="size == 64" src="../assets/auth2logo/Discord-64.svg" />
				<img v-else src="../assets/auth2logo/Discord-32.svg" />
			</template>
			<template v-else-if="loginProvider == 'Alipay'">
				<img v-if="size == 64" src="../assets/auth2logo/Alipay-64.svg" />
				<img v-else src="../assets/auth2logo/Alipay-32.svg" />
			</template>
			<template v-else>
				<el-avatar :size="64" />
			</template>
		</div>
	</div>
</template>

<script>
export default {
	props: {
		loginProvider: {
			required: true,
			type: String,
		},
		size: {
			default: 64,
		},
	},
}
</script>
<style scoped></style>
