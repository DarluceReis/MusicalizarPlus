do $$
declare
    senha_padrao text := 'PBKDF2-SHA256.100000.mrh0JkMF87Axl99BiB1bUw==.GUYlCUcNhFwy83OvTAK32ug5tTCn6igqhHZxnPsG5sg=';
    v_id_professor integer;
    v_id_aluno_ana integer;
    v_id_aluno_bruno integer;
    v_id_aula_ritmo integer;
    v_id_aula_canto integer;
    v_id_matricula_ana integer;
    v_id_matricula_bruno integer;
    v_id_gravacao integer;
begin
    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Prof. Mariana Lopes', 'mariana.prof@musicalizarplus.local', senha_padrao, 'PROFESSOR')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_id_professor;

    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Ana Souza', 'ana.aluno@musicalizarplus.local', senha_padrao, 'ALUNO')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_id_aluno_ana;

    insert into usuarios (nome, email, senha_hash, tipo)
    values ('Bruno Martins', 'bruno.aluno@musicalizarplus.local', senha_padrao, 'ALUNO')
    on conflict (email) do update
        set nome = excluded.nome,
            senha_hash = excluded.senha_hash,
            tipo = excluded.tipo
    returning id_usuario into v_id_aluno_bruno;

    select id_aula into v_id_aula_ritmo
    from aulas
    where id_professor = v_id_professor
      and titulo = 'Ritmo e pulsacao para iniciantes'
    limit 1;

    if v_id_aula_ritmo is null then
        insert into aulas (id_professor, titulo, descricao, nivel)
        values (
            v_id_professor,
            'Ritmo e pulsacao para iniciantes',
            'Exercicios para sentir pulsacao, marcar tempo e reconhecer padroes ritmicos simples.',
            'Iniciante'
        )
        returning id_aula into v_id_aula_ritmo;
    end if;

    select id_aula into v_id_aula_canto
    from aulas
    where id_professor = v_id_professor
      and titulo = 'Percepcao melodica e canto'
    limit 1;

    if v_id_aula_canto is null then
        insert into aulas (id_professor, titulo, descricao, nivel)
        values (
            v_id_professor,
            'Percepcao melodica e canto',
            'Atividades de escuta, repeticao vocal e afinacao em pequenos motivos melodicos.',
            'Basico'
        )
        returning id_aula into v_id_aula_canto;
    end if;

    insert into materiais_aula (id_aula, tipo, url_arquivo, descricao)
    select v_id_aula_ritmo, 'PDF', '/materiais/ritmo-pulsacao.pdf', 'Resumo da aula e exercicios de ritmo'
    where not exists (
        select 1 from materiais_aula
        where id_aula = v_id_aula_ritmo and url_arquivo = '/materiais/ritmo-pulsacao.pdf'
    );

    insert into materiais_aula (id_aula, tipo, url_arquivo, descricao)
    select v_id_aula_canto, 'AUDIO', '/materiais/percepcao-melodica.mp3', 'Audio de apoio para treino melodico'
    where not exists (
        select 1 from materiais_aula
        where id_aula = v_id_aula_canto and url_arquivo = '/materiais/percepcao-melodica.mp3'
    );

    insert into matriculas (id_aluno, id_aula, status)
    values (v_id_aluno_ana, v_id_aula_ritmo, 'ATIVA')
    on conflict (id_aluno, id_aula) do update
        set status = excluded.status
    returning id_matricula into v_id_matricula_ana;

    insert into matriculas (id_aluno, id_aula, status)
    values (v_id_aluno_bruno, v_id_aula_canto, 'ATIVA')
    on conflict (id_aluno, id_aula) do update
        set status = excluded.status
    returning id_matricula into v_id_matricula_bruno;

    select id_gravacao into v_id_gravacao
    from gravacoes
    where id_matricula = v_id_matricula_ana
      and caminho_arquivo = '/gravacoes/ana-ritmo-exercicio-1.mp3'
    limit 1;

    if v_id_gravacao is null then
        insert into gravacoes (id_matricula, caminho_arquivo, observacao_aluno)
        values (
            v_id_matricula_ana,
            '/gravacoes/ana-ritmo-exercicio-1.mp3',
            'Primeira tentativa do exercicio de pulsacao.'
        )
        returning id_gravacao into v_id_gravacao;
    end if;

    insert into feedbacks (id_gravacao, id_professor, comentario, comentario_aluno)
    select
        v_id_gravacao,
        v_id_professor,
        'Boa estabilidade de pulso. Na proxima tentativa, mantenha a acentuacao mais leve no segundo tempo.',
        null
    where not exists (
        select 1 from feedbacks
        where id_gravacao = v_id_gravacao
          and id_professor = v_id_professor
          and comentario like 'Boa estabilidade de pulso.%'
    );
end $$;
