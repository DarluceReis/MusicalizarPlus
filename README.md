# MusicalizarPlus

Projeto em .NET 10 com Blazor, API separada e PostgreSQL usando SQL manual com Npgsql.

## Estrutura

```text
src/MusicalizarPlus.Web             Blazor Server
src/MusicalizarPlus.Api             Minimal API
src/MusicalizarPlus.Application     Regras de negocio e servicos
src/MusicalizarPlus.Infrastructure  Repositorios SQL manual com Npgsql
src/MusicalizarPlus.Contracts       DTOs compartilhados
src/MusicalizarPlus.Domain          Entidades e enums
db/001_initial.sql                  Script inicial do banco
```

## Banco

Crie o banco no PostgreSQL 18 rodando na porta 5433:

```powershell
createdb -h localhost -p 5433 -U postgres musicalizarplus
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/001_initial.sql
```

Se sua senha nao for `postgres`, ajuste a connection string em:

```text
src/MusicalizarPlus.Api/appsettings.Development.json
```

## Rodar

Em dois terminais:

```powershell
dotnet run --project src/MusicalizarPlus.Api --launch-profile http
dotnet run --project src/MusicalizarPlus.Web --launch-profile http
```

URLs locais:

```text
API: http://localhost:5298
Web: http://localhost:5030
```

## Midias e videos

No desenvolvimento local, os uploads devem ficar abaixo de:

```text
src/MusicalizarPlus.Web/wwwroot/media/videos/professores
src/MusicalizarPlus.Web/wwwroot/media/videos/alunos
src/MusicalizarPlus.Web/wwwroot/media/materiais
```

Para producao, nao guarde videos diretamente no banco. Use storage de objetos como S3, Azure Blob, Cloudflare R2 ou MinIO. No PostgreSQL salve apenas o caminho/chave do arquivo e metadados.

## Verificar

```powershell
dotnet restore MusicalizarPlus.slnx
dotnet build MusicalizarPlus.slnx
```
