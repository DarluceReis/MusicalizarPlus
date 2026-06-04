# Musicalizar+

## Documentação técnica e funcional

### Identificação do projeto

**Nome do sistema:** Musicalizar+  
**Tipo de aplicação:** Plataforma web educacional para ensino musical  
**Tecnologias principais:** .NET 10, Blazor Server, Minimal API, PostgreSQL 18 e SQL manual com Npgsql  
**Público-alvo:** professores de música, alunos iniciantes/intermediários e avaliadores acadêmicos  
**Objetivo acadêmico:** demonstrar uma solução funcional para organização de aulas, envio de práticas em vídeo e acompanhamento pedagógico entre professor e aluno.

---

## 1. Visão geral

O Musicalizar+ é uma plataforma web criada para apoiar o processo de aprendizagem musical. O sistema permite que professores publiquem aulas gerais para todos os alunos, criem aulas personalizadas para alunos específicos, anexem materiais de apoio, acompanhem práticas enviadas em vídeo e registrem feedbacks. Na visão do aluno, a plataforma permite visualizar aulas disponíveis, acessar conteúdos personalizados, enviar gravações práticas, acompanhar o histórico de práticas e responder aos feedbacks recebidos.

A proposta central é aproximar professor e aluno dentro de um ambiente único, reduzindo a dispersão de materiais em mensagens, pastas soltas ou links externos. A aplicação foi construída com foco em usabilidade, separação de responsabilidades e possibilidade de evolução futura para armazenamento em nuvem.

---

## 2. Problema abordado

Em aulas de música, principalmente em modelos híbridos ou remotos, é comum que:

- professores enviem vídeos, PDFs e orientações por canais diferentes;
- alunos tenham dificuldade para localizar materiais e feedbacks anteriores;
- práticas enviadas pelos alunos se percam em conversas;
- o professor não tenha uma visão organizada das práticas pendentes;
- conteúdos gerais e personalizados fiquem misturados.

O Musicalizar+ resolve esse problema centralizando aulas, materiais, vídeos, práticas, feedbacks e respostas em uma única plataforma.

---

## 3. Objetivos

### 3.1 Objetivo geral

Desenvolver uma plataforma web para gestão de aulas musicais, envio de práticas por alunos e acompanhamento pedagógico por professores, utilizando .NET, Blazor e PostgreSQL.

### 3.2 Objetivos específicos

- Permitir cadastro e login de alunos e professores.
- Restringir o acesso às áreas internas apenas a usuários autenticados.
- Exibir uma área específica para alunos e outra para professores.
- Permitir que professores criem aulas públicas e personalizadas.
- Permitir upload de vídeos e documentos de apoio.
- Permitir que alunos enviem vídeos de prática.
- Permitir que professores visualizem práticas recebidas.
- Permitir registro de feedback do professor.
- Permitir resposta textual e envio de vídeo-resposta pelo aluno.
- Exibir notificações quando houver novas práticas ou respostas.
- Armazenar informações estruturadas em banco PostgreSQL.
- Utilizar comandos SQL diretamente, sem Entity Framework.
- Manter o código organizado em camadas.

---

## 4. Escopo do sistema

### 4.1 Funcionalidades incluídas

- Autenticação de usuários.
- Cadastro de alunos e professores.
- Perfis com dados pessoais.
- Listagem de aulas para alunos.
- Busca de aulas.
- Visualização de aula em formato de vídeo.
- Download de material complementar.
- Envio de prática pelo aluno.
- Histórico de práticas.
- Criação de aula pública pelo professor.
- Criação de aula personalizada para aluno específico.
- Edição de aulas públicas e personalizadas.
- Visualização de alunos vinculados ao professor.
- Busca de alunos pelo professor.
- Visualização de práticas recebidas.
- Feedback do professor.
- Resposta do aluno ao feedback.
- Notificações para novas atividades.
- Armazenamento local de arquivos.
- Estrutura preparada para armazenamento em AWS S3.

### 4.2 Funcionalidades futuras possíveis

- Publicação em ambiente de produção com domínio próprio.
- Autenticação com JWT ou Identity.
- Recuperação de senha por e-mail.
- Painel administrativo.
- Pagamentos ou assinaturas.
- Transcodificação automática de vídeos.
- Controle avançado de permissões por turma.
- Integração com CloudFront para distribuição de vídeos.
- Relatórios de evolução do aluno.

---

## 5. Perfis de usuário

### 5.1 Aluno

O aluno utiliza a plataforma para acessar aulas, assistir vídeos, baixar materiais, enviar práticas e acompanhar feedbacks.

Funcionalidades principais:

- Login e cadastro.
- Visualização de aulas gerais.
- Visualização de aulas personalizadas.
- Busca de aulas.
- Acesso à página “Meus dados”.
- Edição de dados de perfil.
- Envio de vídeo de prática.
- Consulta do histórico de práticas.
- Visualização de feedback.
- Resposta ao feedback com texto ou vídeo.

### 5.2 Professor

O professor utiliza a plataforma para publicar conteúdos e acompanhar o desempenho dos alunos.

Funcionalidades principais:

- Login e cadastro.
- Tela inicial com aulas publicadas.
- Criação de aulas públicas.
- Criação de aulas personalizadas.
- Upload de vídeo da aula.
- Upload de material complementar.
- Edição de aulas.
- Visualização de alunos.
- Busca de alunos.
- Visualização de práticas enviadas.
- Envio de feedback.
- Visualização de respostas do aluno.
- Notificações de novas práticas/respostas.
- Edição do perfil profissional.

---

## 6. Regras de negócio

- Apenas usuários autenticados podem acessar aulas, dados de perfil, práticas e áreas internas.
- Alunos visualizam aulas públicas e aulas personalizadas destinadas a eles.
- Professores visualizam somente seus próprios alunos, aulas e práticas associadas.
- Aula pública não exige vínculo com aluno específico.
- Aula personalizada deve estar associada a um aluno.
- Práticas enviadas por alunos devem aparecer no histórico do aluno e na área de práticas do professor responsável.
- Feedbacks ficam associados à prática enviada.
- O aluno só deve ver uma prática como avaliada quando houver feedback do professor.
- Quando não houver feedback, o status deve indicar que a prática aguarda avaliação.
- Notificações aparecem apenas quando existem atividades novas.
- Após a visualização da notificação/prática, o indicador de novidade pode ser removido.
- Arquivos enviados não são salvos no banco como binário; o sistema armazena metadados e caminhos/chaves dos arquivos.

---

## 7. Arquitetura da solução

O projeto foi organizado em camadas para separar responsabilidades e facilitar manutenção.

```text
MusicalizarPlus.slnx
db/
src/
  MusicalizarPlus.Api
  MusicalizarPlus.Application
  MusicalizarPlus.Contracts
  MusicalizarPlus.Domain
  MusicalizarPlus.Infrastructure
  MusicalizarPlus.Web
docs/
```

### 7.1 MusicalizarPlus.Domain

Contém as entidades e enumerações principais do domínio:

- Usuario
- Aula
- MaterialAula
- Matricula
- Gravacao
- Feedback
- TipoUsuario
- StatusMatricula

### 7.2 MusicalizarPlus.Contracts

Contém DTOs utilizados para comunicação entre camadas e endpoints:

- requisições de cadastro;
- requisições de login;
- respostas de usuários;
- criação/listagem de aulas;
- materiais;
- matrículas;
- gravações;
- feedbacks.

### 7.3 MusicalizarPlus.Application

Contém serviços de aplicação e interfaces:

- regras de validação;
- orquestração das operações;
- contratos de repositórios;
- abstração de hash de senha;
- retorno padronizado por meio de `ServiceResult`.

### 7.4 MusicalizarPlus.Infrastructure

Contém a implementação de acesso a dados:

- conexão com PostgreSQL usando Npgsql;
- comandos SQL manuais;
- repositórios concretos;
- mapeamento de tipos;
- hash de senha com PBKDF2.

O projeto não utiliza Entity Framework, conforme requisito definido para o desenvolvimento.

### 7.5 MusicalizarPlus.Api

Contém a Minimal API responsável por expor endpoints HTTP:

- autenticação;
- usuários;
- aulas;
- materiais;
- matrículas;
- gravações;
- feedbacks.

### 7.6 MusicalizarPlus.Web

Contém a interface Blazor Server:

- páginas de login e cadastro;
- visão do aluno;
- visão do professor;
- componentes compartilhados;
- controle de upload;
- armazenamento local/S3;
- dados de demonstração utilizados na interface.

---

## 8. Tecnologias utilizadas

| Tecnologia | Uso no projeto |
|---|---|
| .NET 10 | Plataforma principal |
| Blazor Server | Interface web interativa |
| ASP.NET Core Minimal API | Endpoints backend |
| PostgreSQL 18 | Banco de dados relacional |
| Npgsql | Comunicação direta com PostgreSQL |
| PBKDF2 | Hash seguro de senhas |
| AWS SDK S3 | Preparação para armazenamento em nuvem |
| Git LFS | Versionamento de vídeos e PDFs de demonstração |
| HTML/CSS/JavaScript | Estrutura visual, animações e interações |

---

## 9. Banco de dados

O banco foi modelado para representar usuários, aulas, materiais, matrículas, gravações e feedbacks.

### 9.1 Tabelas principais

#### usuarios

Armazena alunos e professores.

Campos principais:

- `id_usuario`
- `nome`
- `email`
- `senha_hash`
- `tipo`
- `data_cadastro`

#### aulas

Armazena aulas criadas por professores.

Campos principais:

- `id_aula`
- `id_professor`
- `titulo`
- `descricao`
- `nivel`
- `data_criacao`

#### materiais_aula

Armazena referências para materiais vinculados às aulas.

Campos principais:

- `id_material`
- `id_aula`
- `tipo`
- `url_arquivo`
- `descricao`

#### matriculas

Relaciona alunos e aulas.

Campos principais:

- `id_matricula`
- `id_aluno`
- `id_aula`
- `data_matricula`
- `status`

#### gravacoes

Armazena práticas enviadas por alunos.

Campos principais:

- `id_gravacao`
- `id_matricula`
- `caminho_arquivo`
- `data_envio`
- `observacao_aluno`

#### feedbacks

Armazena feedbacks dos professores e respostas dos alunos.

Campos principais:

- `id_feedback`
- `id_gravacao`
- `id_professor`
- `comentario`
- `comentario_aluno`
- `data_feedback`

### 9.2 Scripts SQL

Os scripts estão na pasta `db/`:

- `001_initial.sql`: criação das tabelas e índices;
- `002_seed.sql`: dados iniciais;
- `003_demo_logins.sql`: usuários e dados para demonstração.

---

## 10. API

### 10.1 Endpoints principais

| Método | Rota | Finalidade |
|---|---|---|
| GET | `/` | Verifica status da API |
| POST | `/api/auth/login` | Realiza login |
| POST | `/api/usuarios` | Cadastra usuário |
| GET | `/api/usuarios/{id}` | Busca usuário |
| PUT | `/api/usuarios/{id}` | Atualiza usuário |
| PUT | `/api/usuarios/{id}/senha` | Altera senha |
| GET | `/api/aulas` | Lista aulas |
| GET | `/api/aulas/{id}` | Busca aula |
| POST | `/api/aulas` | Cria aula |
| GET | `/api/aulas/{idAula}/materiais` | Lista materiais da aula |
| POST | `/api/materiais` | Cadastra material |
| GET | `/api/matriculas/aluno/{idAluno}` | Lista matrículas do aluno |
| POST | `/api/matriculas` | Cria matrícula |
| GET | `/api/gravacoes/{id}` | Busca gravação |
| GET | `/api/gravacoes/matricula/{idMatricula}` | Lista gravações por matrícula |
| POST | `/api/gravacoes` | Cria gravação |
| GET | `/api/feedbacks/gravacao/{idGravacao}` | Lista feedbacks de uma gravação |
| POST | `/api/feedbacks` | Cria feedback |

---

## 11. Funcionalidades detalhadas

### 11.1 Login

O login permite que alunos e professores acessem a plataforma. A aplicação identifica o tipo de usuário e direciona para a área correta.

Fluxo:

1. O usuário informa e-mail e senha.
2. O sistema valida as credenciais.
3. O hash da senha é verificado com PBKDF2.
4. O usuário autenticado é direcionado para sua área.

### 11.2 Cadastro

Permite criação de conta de aluno ou professor.

Campos principais:

- função;
- nome;
- e-mail;
- senha;
- confirmação de senha.

### 11.3 Área do aluno

A área do aluno possui:

- lista de aulas personalizadas;
- lista de aulas gerais;
- busca;
- acesso ao perfil;
- histórico de práticas;
- envio de gravações.

### 11.4 Aulas do aluno

Cada aula possui:

- título;
- descrição;
- nível;
- vídeo;
- botão visual de play;
- material complementar, quando disponível.

### 11.5 Envio de prática

O aluno pode enviar um vídeo de prática para avaliação.

Fluxo:

1. O aluno acessa a aula.
2. Clica em enviar prática.
3. Seleciona um vídeo.
4. O sistema salva o arquivo.
5. A prática aparece no histórico do aluno.
6. A prática aparece na visão do professor.

### 11.6 Histórico de práticas

O histórico permite acompanhar:

- data de envio;
- aula relacionada;
- status;
- feedback recebido;
- resposta enviada.

Status possíveis:

- aguardando avaliação do professor;
- prática avaliada.

### 11.7 Feedback

O professor visualiza a prática enviada e registra uma orientação.

O aluno pode:

- visualizar o feedback;
- responder com mensagem;
- enviar vídeo-resposta.

### 11.8 Área do professor

A área do professor possui:

- painel inicial;
- aulas gerais postadas;
- criação de nova aula;
- edição de aulas;
- visualização de alunos;
- visualização de práticas;
- notificações.

### 11.9 Aulas públicas

Aula pública é disponibilizada para todos os alunos da plataforma.

Campos:

- título;
- descrição;
- nível;
- vídeo;
- material complementar.

### 11.10 Aulas personalizadas

Aula personalizada é destinada a um aluno específico.

Campos:

- aluno;
- título;
- descrição;
- nível;
- vídeo;
- material complementar.

### 11.11 Perfil

O perfil permite alterar dados exibidos na plataforma.

Dados do aluno:

- nome;
- e-mail;
- pronome;
- instrumento;
- nível;
- professor;
- foto de perfil.

Dados do professor:

- nome;
- e-mail;
- telefone;
- instrumentos ensinados;
- níveis de ensino;
- foto de perfil.

---

## 12. Armazenamento de arquivos

O sistema usa uma abstração chamada `IFileStorage`, permitindo trocar o provedor de armazenamento sem alterar as páginas principais.

### 12.1 Armazenamento local

No modo local, arquivos são salvos em:

```text
src/MusicalizarPlus.Web/App_Data/uploads
```

Subpastas principais:

```text
aulas/videos
aulas/materiais
praticas/videos
perfil/fotos
```

### 12.2 Armazenamento em AWS S3

O projeto já possui preparação para S3 com `AWSSDK.S3`.

Configuração prevista:

```json
{
  "Storage": {
    "Provider": "S3",
    "S3": {
      "BucketName": "nome-do-bucket",
      "Region": "us-east-1",
      "Prefix": "musicalizarplus"
    }
  }
}
```

Em produção, a recomendação é:

- manter vídeos e PDFs em S3;
- manter o bucket privado;
- entregar arquivos por URLs assinadas ou por endpoint autenticado;
- não salvar binários diretamente no PostgreSQL;
- salvar apenas metadados e chaves dos arquivos.

---

## 13. Segurança

Medidas aplicadas:

- senhas armazenadas com hash PBKDF2-SHA256;
- uso de salt aleatório;
- 100.000 iterações no hash de senha;
- comparação segura com `FixedTimeEquals`;
- validação de dados nos serviços de aplicação;
- separação entre dados de aluno e professor;
- controle de acesso nas páginas internas;
- arquivos de configuração local ignorados no Git.

Cuidados adicionais recomendados para produção:

- usar HTTPS obrigatório;
- configurar autenticação robusta com cookies protegidos ou JWT;
- usar Secret Manager, Parameter Store ou variáveis de ambiente;
- limitar tamanho e tipo de upload;
- usar antivírus/validação de arquivos;
- configurar logs e monitoramento;
- restringir CORS;
- aplicar backup automático no PostgreSQL.

---

## 14. Execução local

### 14.1 Pré-requisitos

- .NET 10 SDK
- PostgreSQL 18
- Git
- Git LFS

### 14.2 Clonar o repositório

```powershell
git clone https://github.com/DarluceReis/MusicalizarPlus.git
cd MusicalizarPlus
git lfs pull
```

### 14.3 Configurar o banco

Crie o banco:

```powershell
createdb -h localhost -p 5433 -U postgres musicalizarplus
```

Execute os scripts:

```powershell
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/001_initial.sql
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/002_seed.sql
psql -h localhost -p 5433 -U postgres -d musicalizarplus -f db/003_demo_logins.sql
```

### 14.4 Configurar appsettings local

Copie o exemplo da API:

```powershell
copy src\MusicalizarPlus.Api\appsettings.Development.example.json src\MusicalizarPlus.Api\appsettings.Development.json
```

Depois edite a senha do PostgreSQL no arquivo:

```text
src/MusicalizarPlus.Api/appsettings.Development.json
```

Copie o exemplo da Web:

```powershell
copy src\MusicalizarPlus.Web\appsettings.Development.example.json src\MusicalizarPlus.Web\appsettings.Development.json
```

### 14.5 Restaurar e compilar

```powershell
dotnet restore MusicalizarPlus.slnx
dotnet build MusicalizarPlus.slnx
```

### 14.6 Rodar API

```powershell
dotnet run --project src/MusicalizarPlus.Api --launch-profile http
```

URL:

```text
http://localhost:5298
```

### 14.7 Rodar Web

Em outro terminal:

```powershell
dotnet run --project src/MusicalizarPlus.Web --launch-profile http
```

URL:

```text
http://localhost:5030
```

---

## 15. Contas de demonstração

Senha padrão das contas de demonstração:

```text
P@ssw0rd
```

### Professor com alunos

```text
vinicius.prof@musicalizarplus.local
```

Alunos vinculados:

- João Alves
- Ana Maria
- Daniela Roc.
- Rui Barbosa
- Marina Costa
- Caio Lima

### Alunos

```text
joao.aluno@musicalizarplus.local
ana.aluno@musicalizarplus.local
daniela.aluno@musicalizarplus.local
rui.aluno@musicalizarplus.local
```

---

## 16. Fluxos recomendados para demonstração

### 16.1 Fluxo do aluno

1. Entrar com uma conta de aluno.
2. Visualizar lista de aulas.
3. Usar o campo de busca.
4. Abrir uma aula.
5. Baixar material de apoio.
6. Enviar uma prática em vídeo.
7. Acessar histórico de práticas.
8. Verificar status da prática.
9. Responder a feedback, quando houver.

### 16.2 Fluxo do professor

1. Entrar com a conta do professor.
2. Visualizar painel inicial.
3. Criar aula pública.
4. Criar aula personalizada para um aluno.
5. Anexar vídeo e PDF.
6. Visualizar alunos.
7. Buscar aluno pelo nome.
8. Acessar práticas recebidas.
9. Abrir uma prática.
10. Enviar feedback.
11. Verificar notificação de resposta do aluno.

---

## 17. Versionamento

O projeto utiliza Git e GitHub.

Repositório:

```text
https://github.com/DarluceReis/MusicalizarPlus
```

Arquivos grandes, como vídeos e PDFs, são versionados com Git LFS.

Comandos úteis:

```powershell
git status
git add .
git commit -m "Mensagem do commit"
git push
```

---

## 18. Considerações sobre vídeos no TCC

Para apresentação acadêmica, é possível utilizar vídeos demonstrativos, desde que respeitados os direitos autorais e as licenças de uso. A opção mais segura é:

- gravar vídeos próprios;
- usar vídeos de bancos gratuitos com licença adequada;
- citar a fonte quando exigido;
- evitar músicas protegidas por direitos autorais;
- manter os vídeos apenas como demonstração técnica.

Para produção real, vídeos de professores e alunos devem ser privados e controlados por autenticação.

---

## 19. Hospedagem recomendada

Para uma versão online com baixo custo:

- hospedar a aplicação em AWS Elastic Beanstalk, App Runner ou EC2 simples;
- usar PostgreSQL em RDS ou banco gerenciado equivalente;
- usar S3 para vídeos e PDFs;
- manter bucket privado;
- usar CloudFront apenas se houver necessidade de distribuição otimizada;
- usar variáveis de ambiente para conexão e credenciais.

Para TCC, a execução local com GitHub e dados de demonstração é suficiente para validar o funcionamento.

---

## 20. Conclusão

O Musicalizar+ apresenta uma solução funcional para apoiar o ensino musical com interação entre professor e aluno. A plataforma contempla cadastro, autenticação, aulas públicas, aulas personalizadas, envio de vídeos, materiais complementares, histórico de práticas, feedbacks e respostas. A arquitetura em camadas, o uso de SQL manual com PostgreSQL e a separação entre interface, API, aplicação, domínio e infraestrutura tornam o projeto organizado e preparado para evolução.

O sistema atende ao objetivo de demonstrar uma plataforma educacional completa, com recursos visuais, fluxos práticos e base técnica compatível com boas práticas de desenvolvimento web moderno.
