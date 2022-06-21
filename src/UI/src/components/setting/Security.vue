<template>
	<div id="security">
		<Password />
		<TwoFactorAuthentication class="mg50" />
	</div>
</template>

<script>
import Password from '../Password.vue'
import TwoFactorAuthentication from './TwoFactorAuthentication.vue'
export default {
	components: { Password, TwoFactorAuthentication },
}
</script>
<style scoped>
.mg50 {
	margin-top: 50px;
}
</style>
