echo "==========删除Migrations=========="
rm -rf Migrations

#create migraions
echo "==========生成迁移=========="
dotnet ef migrations add Init -c IdsConfigurationDbContext -o Migrations/Configuration
dotnet ef migrations add Init -c IdentityDbContext -o Migrations/Identity
dotnet ef migrations add Init -c IdsPersistedGrantDbContext -o Migrations/PersistedGrant

#update database
echo "==========更新数据=========="
dotnet ef database update --context IdsConfigurationDbContext
dotnet ef database update --context IdentityDbContext
dotnet ef database update --context IdsPersistedGrantDbContext

echo "==========处理完成=========="