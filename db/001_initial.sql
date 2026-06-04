create table if not exists usuarios (
    id_usuario integer generated always as identity primary key,
    nome varchar(100) not null,
    email varchar(100) not null unique,
    senha_hash varchar(500) not null,
    tipo varchar(20) not null check (tipo in ('ALUNO', 'PROFESSOR')),
    data_cadastro timestamp not null default current_timestamp
);

create table if not exists aulas (
    id_aula integer generated always as identity primary key,
    id_professor integer not null references usuarios(id_usuario),
    titulo varchar(150) not null,
    descricao text,
    nivel varchar(50),
    data_criacao timestamp not null default current_timestamp
);

create table if not exists materiais_aula (
    id_material integer generated always as identity primary key,
    id_aula integer not null references aulas(id_aula) on delete cascade,
    tipo varchar(50) not null,
    url_arquivo varchar(255) not null,
    descricao text
);

create table if not exists matriculas (
    id_matricula integer generated always as identity primary key,
    id_aluno integer not null references usuarios(id_usuario),
    id_aula integer not null references aulas(id_aula),
    data_matricula timestamp not null default current_timestamp,
    status varchar(30) not null default 'ATIVA',
    constraint matriculas_status_check check (status in ('ATIVA', 'CANCELADA', 'CONCLUIDA')),
    constraint matriculas_aluno_aula_unique unique (id_aluno, id_aula)
);

create table if not exists gravacoes (
    id_gravacao integer generated always as identity primary key,
    id_matricula integer not null references matriculas(id_matricula),
    caminho_arquivo varchar(255) not null,
    data_envio timestamp not null default current_timestamp,
    observacao_aluno text
);

create table if not exists feedbacks (
    id_feedback integer generated always as identity primary key,
    id_gravacao integer not null references gravacoes(id_gravacao),
    id_professor integer not null references usuarios(id_usuario),
    comentario text not null,
    comentario_aluno text,
    data_feedback timestamp not null default current_timestamp
);

create index if not exists ix_aulas_professor on aulas(id_professor);
create index if not exists ix_materiais_aula on materiais_aula(id_aula);
create index if not exists ix_matriculas_aluno on matriculas(id_aluno);
create index if not exists ix_matriculas_aula on matriculas(id_aula);
create index if not exists ix_gravacoes_matricula on gravacoes(id_matricula);
create index if not exists ix_feedbacks_gravacao on feedbacks(id_gravacao);
