# Musicalizar+

Plataforma web para ensino musical, criada com .NET 10, Blazor Server, Minimal API e PostgreSQL 18. O projeto usa SQL manual com Npgsql, sem Entity Framework.

## Documentação

A documentação completa para TCC está em:

```text
docs/DOCUMENTACAO_TCC.md
```

Ela descreve funcionalidades, arquitetura, banco de dados, endpoints, fluxos de aluno/professor, segurança, armazenamento de arquivos e instruções de execução.

## Estrutura

```text
src/MusicalizarPlus.Web             Interface Blazor Server
src/MusicalizarPlus.Api             Backend Minimal API
src/MusicalizarPlus.Application     Serviços e regras de aplicação
src/MusicalizarPlus.Infrastructure  Repositórios SQL manual com Npgsql
src/MusicalizarPlus.Contracts       DTOs compartilhados
src/MusicalizarPlus.Domain          Entidades e enumerações
db/                                 Scripts SQL
docs/                               Documentação do projeto
```

## Pré-requisitos

- .NET 10 SDK
- PostgreSQL 18
- Git
- Git LFS

## Clonar

```powershell
git clone https://github.com/DarluceReis/MusicalizarPlus.git
cd MusicalizarPlus
git lfs pull
```

## Banco

Crie o banco no PostgreSQL 18 rodando na porta 5433:

```powershell
createdb -h localhost -p 5433 -U postgres musicalizarplus
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/001_initial.sql
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/002_seed.sql
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/003_demo_logins.sql
```

## Configuração local

Copie os arquivos de exemplo:

```powershell
copy src\MusicalizarPlus.Api\appsettings.Development.example.json src\MusicalizarPlus.Api\appsettings.Development.json
copy src\MusicalizarPlus.Web\appsettings.Development.example.json src\MusicalizarPlus.Web\appsettings.Development.json
```

Depois ajuste a senha do PostgreSQL em:

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

## Contas de demonstração

Senha padrão:

```text
P@ssw0rd
```

Professor:

```text
vinicius.prof@musicalizarplus.local
```

Alunos:

```text
joao.aluno@musicalizarplus.local
ana.aluno@musicalizarplus.local
daniela.aluno@musicalizarplus.local
rui.aluno@musicalizarplus.local
```

## Verificar

```powershell
dotnet restore MusicalizarPlus.slnx
dotnet build MusicalizarPlus.slnx
```
