<template>
	<div id="successed">
		<el-result icon="success" :title="$route.query.title" :subTitle="$route.query.subTitle">
			<template slot="extra">
				<a :href="$route.query.returnUrl">
					<el-button type="primary" size="medium">返回</el-button>
				</a>
			</template>
		</el-result>
	</div>
</template>
