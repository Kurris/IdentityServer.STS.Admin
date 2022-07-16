<template>
	<div id="device"></div>
</template>

<script>
export default {
	beforeMount() {
		if (this.$route.query.userCode === undefined) {
			this.$router.push('/userCodeCapture')
		} else {
			this.$router.push({
				path: '/userCodeConfirmation',
				query: {
					userCode: this.$route.query.userCode,
				},
			})
		}
	},
}
</script>
