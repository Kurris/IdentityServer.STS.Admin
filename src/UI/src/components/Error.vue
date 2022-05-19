<template>
	<div id="error">
		错误消息:{{ error }}
		<br />
		ErrorId: {{ $route.query.errorId }}
	</div>
</template>

<script>
import { getError } from '../net/api.js'

export default {
	components: {},
	data() {
		return {
			error: null,
		}
	},
	async mounted() {
		this.error = await getError({ errorId: this.$route.query.errorId })
	},
}
</script>
<style scoped></style>
