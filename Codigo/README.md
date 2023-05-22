# Comandos úteis
## Scaffolding para gerar classes no projeto Core
`dotnet ef dbcontext scaffold "server=localhost;port=3306;user=root;password=123456;database=GrupoMusical" MySql.EntityFrameworkCore -p Core -c GrupoMusicalContext -v -f`