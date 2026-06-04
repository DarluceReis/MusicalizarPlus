do $$
declare
    senha_padrao text := 'PBKDF2-SHA256.100000.mrh0JkMF87Axl99BiB1bUw==.GUYlCUcNhFwy83OvTAK32ug5tTCn6igqhHZxnPsG5sg=';
    v_professor integer;
    v_joao integer;
    v_ana integer;
    v_daniela integer;
    v_rui integer;
    v_aula_fundamentos integer;
    v_aula_ritmos integer;
    v_matricula_joao integer;
    v_matricula_ana integer;
    v_gravacao_joao integer;
begin
    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Vinicius Alves', 'vinicius.prof@musicalizarplus.local', senha_padrao, 'PROFESSOR')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_professor;

    insert into usuarios (nome, email, senha_hash, tipo)
    values ('João Alves', 'joao.aluno@musicalizarplus.local', senha_padrao, 'ALUNO')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_joao;

    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Ana Maria', 'ana.aluno@musicalizarplus.local', senha_padrao, 'ALUNO')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_ana;

    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Daniela Rocha', 'daniela.aluno@musicalizarplus.local', senha_padrao, 'ALUNO')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_daniela;

    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Rui Barbosa', 'rui.aluno@musicalizarplus.local', senha_padrao, 'ALUNO')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_rui;

    select id_aula into v_aula_fundamentos
    from aulas
    where id_professor = v_professor
      and titulo = 'Fundamentos'
    limit 1;

    if v_aula_fundamentos is null then
        insert into aulas (id_professor, titulo, descricao, nivel)
        values (
            v_professor,
            'Fundamentos',
            'Aprenda os fundamentos do instrumento, postura correta e primeiros acordes.',
            'Iniciante'
        )
        returning id_aula into v_aula_fundamentos;
    end if;

    select id_aula into v_aula_ritmos
    from aulas
    where id_professor = v_professor
      and titulo = 'Ritmos Essenciais'
    limit 1;

    if v_aula_ritmos is null then
        insert into aulas (id_professor, titulo, descricao, nivel)
        values (
            v_professor,
            'Ritmos Essenciais',
            'Pratique padrões rítmicos básicos usados em músicas populares.',
            'Iniciante / Intermediário'
        )
        returning id_aula into v_aula_ritmos;
    end if;

    insert into materiais_aula (id_aula, tipo, url_arquivo, descricao)
    select v_aula_fundamentos, 'PDF', '/media/materiais/fundamentos.pdf', 'Material de apoio da aula Fundamentos'
    where not exists (
        select 1 from materiais_aula
        where id_aula = v_aula_fundamentos
          and url_arquivo = '/media/materiais/fundamentos.pdf'
    );

    insert into matriculas (id_aluno, id_aula, status)
    values (v_joao, v_aula_fundamentos, 'ATIVA')
    on conflict (id_aluno, id_aula) do update
        set status = excluded.status
    returning id_matricula into v_matricula_joao;

    insert into matriculas (id_aluno, id_aula, status)
    values (v_ana, v_aula_ritmos, 'ATIVA')
    on conflict (id_aluno, id_aula) do update
        set status = excluded.status
    returning id_matricula into v_matricula_ana;

    insert into matriculas (id_aluno, id_aula, status)
    values (v_daniela, v_aula_fundamentos, 'ATIVA')
    on conflict (id_aluno, id_aula) do update
        set status = excluded.status;

    insert into matriculas (id_aluno, id_aula, status)
    values (v_rui, v_aula_ritmos, 'ATIVA')
    on conflict (id_aluno, id_aula) do update
        set status = excluded.status;

    select id_gravacao into v_gravacao_joao
    from gravacoes
    where id_matricula = v_matricula_joao
      and caminho_arquivo = '/media/videos/alunos/joao-fundamentos-demo.mp4'
    limit 1;

    if v_gravacao_joao is null then
        insert into gravacoes (id_matricula, caminho_arquivo, observacao_aluno)
        values (
            v_matricula_joao,
            '/media/videos/alunos/joao-fundamentos-demo.mp4',
            'Vídeo de prática enviado para avaliação.'
        )
        returning id_gravacao into v_gravacao_joao;
    end if;

    insert into feedbacks (id_gravacao, id_professor, comentario, comentario_aluno)
    select
        v_gravacao_joao,
        v_professor,
        'Boa evolução na troca dos acordes. Grave uma nova resposta mantendo o pulso mais constante.',
        null
    where not exists (
        select 1 from feedbacks
        where id_gravacao = v_gravacao_joao
          and id_professor = v_professor
          and comentario like 'Boa evolução na troca dos acordes.%'
    );
end $$;
