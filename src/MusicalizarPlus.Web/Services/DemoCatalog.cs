namespace MusicalizarPlus.Services;

public static class DemoCatalog
{
    public static IReadOnlyList<LessonCard> PersonalizedLessons { get; } =
    [
        new("fundamentos", "Fundamentos", "Aprenda os fundamentos do instrumento, postura correta e primeiros acordes.", "Iniciante", "photo-fundamentos"),
        new("ritmos-essenciais", "Ritmos Essenciais", "Pratique padrões rítmicos básicos usados em músicas populares.", "Iniciante / Intermediário", "photo-ritmos"),
        new("acordes-maiores-menores", "Acordes Maiores e Menores", "Explore novos acordes e aprenda como aplicá-los em músicas reais.", "Intermediário", "photo-acordes"),
        new("tecnicas-dedilhado", "Técnicas de Dedilhado", "Exercícios para desenvolver coordenação e fluidez no dedilhado.", "Intermediário / Avançado", "photo-dedilhado"),
        new("primeiras-batidas", "Primeiras Batidas", "Treine batidas simples com troca de acordes em andamento lento.", "Iniciante", "photo-primeiras"),
        new("repertorio-inicial", "Repertório Inicial", "Aplique acordes básicos em músicas curtas para ganhar confiança.", "Básico", "photo-repertorio")
    ];

    public static IReadOnlyList<LessonCard> GeneralLessons { get; } =
    [
        new("partes-violao", "Nome das partes do violão", "Conheça as partes do violão e suas funções principais.", "Iniciante", "photo-violao"),
        new("partes-viola", "Nome das partes da viola", "Identifique corpo, braço, tarraxas e cordas da viola.", "Iniciante", "photo-viola"),
        new("trocar-cordas", "Como trocar as cordas dos instrumentos", "Passo a passo para trocar cordas com segurança.", "Básico", "photo-cordas"),
        new("manutencao-instrumentos", "Como fazer a manutenção dos instrumentos", "Cuidados essenciais antes e depois do estudo.", "Básico", "photo-manutencao"),
        new("afinacao-ouvido", "Afinação pelo ouvido", "Exercícios simples para perceber cordas desafinadas e ajustar com calma.", "Básico", "photo-viola"),
        new("postura-estudo", "Postura para estudar melhor", "Organize corpo, instrumento e rotina para evoluir sem tensão.", "Iniciante", "photo-fundamentos"),
        new("leitura-cifras", "Leitura de cifras", "Entenda símbolos comuns e como transformar cifras em prática musical.", "Intermediário", "photo-acordes"),
        new("aquecimento-dedos", "Aquecimento dos dedos", "Prepare as mãos antes de tocar com movimentos curtos e progressivos.", "Básico", "photo-dedilhado")
    ];

    public static IReadOnlyList<StudentCard> Students { get; } =
    [
        new("João Alves", "Iniciante", "Violão", "3 meses de Curso", "avatar-joao"),
        new("Ana Maria", "Intermediário", "Viola", "8 meses de Curso", "avatar-ana"),
        new("Daniela Roc.", "Avançado", "Violão", "1 ano de Curso", "avatar-daniela"),
        new("Rui Barbosa", "Iniciante", "Viola", "1 mês de Curso", "avatar-rui"),
        new("Marina Costa", "Intermediário", "Violão", "6 meses de Curso", "avatar-marina"),
        new("Caio Lima", "Básico", "Viola", "2 meses de Curso", "avatar-caio"),
        new("Beatriz Rocha", "Iniciante", "Violão", "4 meses de Curso", "avatar-ana"),
        new("Lucas Pereira", "Básico", "Guitarra", "5 meses de Curso", "avatar-rui"),
        new("Helena Martins", "Intermediário", "Violão", "10 meses de Curso", "avatar-daniela"),
        new("Pedro Nunes", "Avançado", "Viola", "1 ano e 4 meses de Curso", "avatar-caio"),
        new("Sofia Almeida", "Básico", "Cavaquinho", "7 meses de Curso", "avatar-marina"),
        new("Mateus Lima", "Iniciante", "Violão", "2 meses de Curso", "avatar-joao"),
        new("Clara Dias", "Intermediário", "Guitarra", "9 meses de Curso", "avatar-ana"),
        new("Gustavo Melo", "Básico", "Baixo", "6 meses de Curso", "avatar-rui"),
        new("Lara Campos", "Avançado", "Violão", "1 ano e 2 meses de Curso", "avatar-daniela"),
        new("Renan Souza", "Iniciante", "Viola", "1 mês de Curso", "avatar-caio")
    ];

    public static IReadOnlyList<StudentCard> ProfessorStudents { get; } =
        Students.Take(6).ToList();

    public static IReadOnlyList<PracticeCard> Practices { get; } =
    [
        new("fundamentos", "Fundamentos", "João Alves", "Iniciante", "Violão", "12/11/2025", "photo-pratica-1", true),
        new("ritmos-essenciais", "Ritmos Essen.", "Ana Maria", "Intermediário", "Viola", "13/11/2025", "photo-pratica-2", false),
        new("tecnicas-dedilhado", "Técnicas de D.", "Daniela Roc.", "Avançado", "Violão", "13/11/2025", "photo-pratica-3", false),
        new("acordes-maiores", "Acordes Mai.", "Rui Barbosa", "Iniciante", "Viola", "14/11/2025", "photo-pratica-4", true)
    ];

    public static LessonCard GetLesson(string? slug) =>
        PersonalizedLessons.Concat(GeneralLessons).FirstOrDefault(lesson => lesson.Slug == slug)
        ?? PersonalizedLessons[0];

    public static PracticeCard GetPractice(string? slug) =>
        Practices.FirstOrDefault(practice => practice.Slug == slug) ?? Practices[0];
}

public sealed record LessonCard(string Slug, string Title, string Description, string Level, string PhotoClass);
public sealed record StudentCard(string Name, string Level, string Instrument, string Time, string AvatarClass);
public sealed record PracticeCard(string Slug, string Title, string Student, string Level, string Instrument, string Date, string PhotoClass, bool HasAlert);
