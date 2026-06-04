using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace MusicalizarPlus.Services;

public sealed class LearningContentStore
{
    public const string DemoProfessorName = "Vinicius Alves";

    private readonly List<LessonContent> lessons = [];
    private readonly List<PracticeSubmission> practices = [];
    private readonly IFileStorage fileStorage;
    private readonly string statePath;
    private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    public LearningContentStore(IFileStorage fileStorage, IWebHostEnvironment environment)
    {
        this.fileStorage = fileStorage;
        statePath = Path.Combine(environment.ContentRootPath, "App_Data", "learning-content.json");

        if (!LoadState())
        {
            lessons.AddRange(DemoCatalog.GeneralLessons.Select(lesson => LessonContent.FromCard(lesson, true, null, DemoProfessorName)));
            lessons.AddRange(DemoCatalog.PersonalizedLessons.Select(lesson => LessonContent.FromCard(lesson, false, "João Alves", DemoProfessorName)));
            practices.AddRange(DemoCatalog.Practices.Select(practice => PracticeSubmission.FromCard(practice, DemoProfessorName)));
            SaveState();
        }
    }

    public IReadOnlyList<LessonContent> PublicLessons => lessons.Where(lesson => lesson.IsPublic).ToList();

    public IReadOnlyList<LessonContent> PublicLessonsForProfessor(string professorName) =>
        lessons.Where(lesson => lesson.IsPublic && SameProfessor(lesson.ProfessorName, professorName)).ToList();

    public IReadOnlyList<LessonContent> PrivateLessonsFor(string studentName) =>
        lessons.Where(lesson => !lesson.IsPublic && string.Equals(lesson.StudentName, studentName, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<LessonContent> PrivateLessonsFor(string professorName, string studentName) =>
        lessons.Where(lesson =>
            !lesson.IsPublic
            && SameProfessor(lesson.ProfessorName, professorName)
            && string.Equals(lesson.StudentName, studentName, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<LessonContent> LessonsByType(bool isPublic) =>
        lessons.Where(lesson => lesson.IsPublic == isPublic).ToList();

    public IReadOnlyList<PracticeSubmission> Practices => practices.ToList();

    public IReadOnlyList<PracticeSubmission> PracticesForStudent(string studentName) =>
        practices.Where(practice => string.Equals(practice.Student, studentName, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<PracticeSubmission> PracticesForProfessor(string professorName) =>
        practices.Where(practice => SameProfessor(practice.ProfessorName, professorName)).ToList();

    public IReadOnlyList<PracticeSubmission> NewPracticesForProfessor(string professorName) =>
        practices
            .Where(practice =>
                SameProfessor(practice.ProfessorName, professorName)
                && (practice.HasNewActivity || practice.HasNewReplyForProfessor))
            .ToList();

    public IReadOnlyList<StudentCard> StudentsForProfessor(string professorName) =>
        IsDemoProfessor(professorName)
            ? DemoCatalog.ProfessorStudents
            : StudentsFromProfessorContent(professorName);

    public int NewPracticeCount => practices.Count(practice => practice.HasNewActivity);

    public LessonContent? GetLesson(string slug) =>
        lessons.FirstOrDefault(lesson => string.Equals(lesson.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public PracticeSubmission? GetPractice(string slug) =>
        practices.FirstOrDefault(practice => string.Equals(practice.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public PracticeSubmission? GetPracticeForProfessor(string slug, string professorName) =>
        practices.FirstOrDefault(practice =>
            string.Equals(practice.Slug, slug, StringComparison.OrdinalIgnoreCase)
            && SameProfessor(practice.ProfessorName, professorName));

    public async Task<LessonContent> SaveLessonAsync(LessonDraft draft)
    {
        var slug = CreateSlug(draft.Title);
        var isPublic = draft.IsPublic || string.IsNullOrWhiteSpace(draft.StudentName);
        var lesson = new LessonContent
        {
            Slug = slug,
            Title = draft.Title.Trim(),
            Description = draft.Description.Trim(),
            Level = draft.Level,
            PhotoClass = isPublic ? "photo-violao" : "photo-fundamentos",
            ProfessorName = draft.ProfessorName,
            IsPublic = isPublic,
            StudentName = isPublic ? null : draft.StudentName,
            Video = await fileStorage.SaveAsync(draft.Video, "aulas/videos"),
            Material = await fileStorage.SaveAsync(draft.Material, "aulas/materiais")
        };

        lessons.RemoveAll(item => item.Slug == slug);
        lessons.Insert(0, lesson);
        SaveState();
        return lesson;
    }

    public void UpdateLesson(LessonContent lesson)
    {
        var index = lessons.FindIndex(item => string.Equals(item.Slug, lesson.Slug, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            return;
        }

        lessons[index] = lesson;
        SaveState();
    }

    public async Task SavePracticeVideoAsync(string slug, IBrowserFile file, string studentName)
    {
        var practice = EnsurePracticeForStudent(slug, studentName);

        practice.VideoResponse = await fileStorage.SaveAsync(file, "praticas/videos");
        practice.HasNewActivity = true;
        SaveState();
    }

    public async Task SavePracticeVideoAsync(string slug, IFormFile file, string studentName)
    {
        var practice = EnsurePracticeForStudent(slug, studentName);

        practice.VideoResponse = await fileStorage.SaveAsync(file, "praticas/videos");
        practice.HasNewActivity = true;
        SaveState();
    }

    public void SaveFeedback(string slug, string professor, string text)
    {
        var practice = GetPractice(slug);
        if (practice is null)
        {
            return;
        }

        practice.FeedbackText = text.Trim();
        practice.FeedbackProfessor = professor;
        practice.FeedbackAt = DateTime.Now;
        practice.HasNewActivity = false;
        practice.HasNewFeedbackForStudent = true;
        SaveState();
    }

    public void SaveStudentReply(string slug, string text)
    {
        var practice = GetPractice(slug);
        if (practice is null)
        {
            return;
        }

        practice.StudentReply = text.Trim();
        practice.StudentReplyAt = DateTime.Now;
        practice.HasNewReplyForProfessor = true;
        SaveState();
    }

    public void MarkPracticeViewed(string slug)
    {
        var practice = GetPractice(slug);
        if (practice is null)
        {
            return;
        }

        practice.HasNewActivity = false;
        practice.HasNewReplyForProfessor = false;
        SaveState();
    }

    public void MarkFeedbackViewed(string slug)
    {
        var practice = GetPractice(slug);
        if (practice is null)
        {
            return;
        }

        practice.HasNewFeedbackForStudent = false;
        SaveState();
    }

    private static bool IsDemoProfessor(string professorName) =>
        SameProfessor(DemoProfessorName, professorName);

    private static bool SameProfessor(string? first, string? second) =>
        string.Equals(first, second, StringComparison.OrdinalIgnoreCase);

    private IReadOnlyList<StudentCard> StudentsFromProfessorContent(string professorName)
    {
        var studentNames = lessons
            .Where(lesson => SameProfessor(lesson.ProfessorName, professorName) && !string.IsNullOrWhiteSpace(lesson.StudentName))
            .Select(lesson => lesson.StudentName!)
            .Concat(practices.Where(practice => SameProfessor(practice.ProfessorName, professorName)).Select(practice => practice.Student))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return DemoCatalog.Students
            .Where(student => studentNames.Contains(student.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }

    private PracticeSubmission EnsurePracticeForStudent(string slug, string studentName)
    {
        var practice = practices.FirstOrDefault(item =>
            string.Equals(item.Slug, slug, StringComparison.OrdinalIgnoreCase)
            && string.Equals(item.Student, studentName, StringComparison.OrdinalIgnoreCase));
        if (practice is not null)
        {
            return practice;
        }

        var lesson = GetLesson(slug);
        var card = DemoCatalog.GetLesson(slug);
        practice = new PracticeSubmission
        {
            Slug = lesson?.Slug ?? card.Slug,
            Title = lesson?.Title ?? card.Title,
            Student = studentName,
            Level = lesson?.Level ?? card.Level,
            Instrument = "Violão",
            Date = DateTime.Now.ToString("dd/MM/yyyy"),
            PhotoClass = lesson?.PhotoClass ?? card.PhotoClass,
            ProfessorName = lesson?.ProfessorName ?? DemoProfessorName,
            HasNewActivity = true
        };

        practices.Insert(0, practice);
        return practice;
    }

    private bool LoadState()
    {
        if (!File.Exists(statePath))
        {
            return false;
        }

        var state = JsonSerializer.Deserialize<LearningContentState>(File.ReadAllText(statePath), jsonOptions);
        if (state is null)
        {
            return false;
        }

        lessons.AddRange(state.Lessons);
        practices.AddRange(state.Practices);
        foreach (var lesson in lessons.Where(lesson => string.IsNullOrWhiteSpace(lesson.ProfessorName)))
        {
            lesson.ProfessorName = DemoProfessorName;
        }
        foreach (var practice in practices.Where(practice => string.IsNullOrWhiteSpace(practice.ProfessorName)))
        {
            practice.ProfessorName = DemoProfessorName;
        }
        foreach (var lesson in lessons.Where(lesson => !lesson.IsPublic && string.IsNullOrWhiteSpace(lesson.StudentName)))
        {
            lesson.IsPublic = true;
        }
        SaveState();
        return lessons.Count > 0 && practices.Count > 0;
    }

    private void SaveState()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);
        var state = new LearningContentState(lessons, practices);
        File.WriteAllText(statePath, JsonSerializer.Serialize(state, jsonOptions));
    }

    private static string CreateSlug(string title)
    {
        var text = title.Trim().ToLowerInvariant()
            .Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a")
            .Replace("é", "e").Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o").Replace("ô", "o").Replace("õ", "o")
            .Replace("ú", "u")
            .Replace("ç", "c");

        var chars = text.Select(character => char.IsLetterOrDigit(character) ? character : '-').ToArray();
        return string.Join('-', new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}

public sealed record LearningContentState(List<LessonContent> Lessons, List<PracticeSubmission> Practices);

public sealed class LessonContent
{
    public required string Slug { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Level { get; set; }
    public required string PhotoClass { get; set; }
    public string ProfessorName { get; set; } = LearningContentStore.DemoProfessorName;
    public required bool IsPublic { get; set; }
    public string? StudentName { get; init; }
    public UploadedAsset? Video { get; set; }
    public UploadedAsset? Material { get; set; }

    public static LessonContent FromCard(LessonCard card, bool isPublic, string? studentName, string professorName) =>
        new()
        {
            Slug = card.Slug,
            Title = card.Title,
            Description = card.Description,
            Level = card.Level,
            PhotoClass = card.PhotoClass,
            ProfessorName = professorName,
            IsPublic = isPublic,
            StudentName = studentName
        };
}

public sealed class PracticeSubmission
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string Student { get; init; }
    public required string Level { get; init; }
    public required string Instrument { get; init; }
    public required string Date { get; init; }
    public required string PhotoClass { get; init; }
    public string ProfessorName { get; set; } = LearningContentStore.DemoProfessorName;
    public bool HasNewActivity { get; set; }
    public string? FeedbackText { get; set; }
    public string? FeedbackProfessor { get; set; }
    public DateTime? FeedbackAt { get; set; }
    public bool HasNewFeedbackForStudent { get; set; }
    public string? StudentReply { get; set; }
    public DateTime? StudentReplyAt { get; set; }
    public bool HasNewReplyForProfessor { get; set; }
    public UploadedAsset? VideoResponse { get; set; }

    public static PracticeSubmission FromCard(PracticeCard card, string professorName) =>
        new()
        {
            Slug = card.Slug,
            Title = card.Title,
            Student = card.Student,
            Level = card.Level,
            Instrument = card.Instrument,
            Date = card.Date,
            PhotoClass = card.PhotoClass,
            ProfessorName = professorName,
            HasNewActivity = card.HasAlert
        };
}

public sealed class LessonDraft
{
    public bool IsPublic { get; set; }
    public string ProfessorName { get; set; } = LearningContentStore.DemoProfessorName;
    public string? StudentName { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Level { get; set; } = "Iniciante";
    public IBrowserFile? Video { get; set; }
    public IBrowserFile? Material { get; set; }
}

public sealed class UploadedAsset
{
    public required string Key { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}
