# Midias locais de desenvolvimento

Esta pasta e apenas para desenvolvimento local.

```text
media/videos/professores  aulas em video enviadas pelos professores
media/videos/alunos       praticas/gravacoes enviadas pelos alunos
media/materiais           PDFs, cifras e arquivos auxiliares
```

Em producao, os arquivos devem ir para storage de objetos, por exemplo S3, Azure Blob, Cloudflare R2 ou MinIO. O PostgreSQL deve guardar apenas metadados: dono, tipo, nome original, content-type, tamanho, caminho/chave do arquivo e data de envio.
