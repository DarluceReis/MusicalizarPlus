using System.Text.Json;

namespace MusicalizarPlus.Services;

public sealed class ProfilePreferencesStore(IWebHostEnvironment environment)
{
    private readonly string statePath = Path.Combine(environment.ContentRootPath, "App_Data", "profile-preferences.json");
    private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
    private readonly Dictionary<int, ProfilePreferences> profiles = [];

    public ProfilePreferences Get(int userId)
    {
        Load();
        if (!profiles.TryGetValue(userId, out var profile))
        {
            profile = new ProfilePreferences();
            profiles[userId] = profile;
        }

        return profile;
    }

    public void Save(int userId, ProfilePreferences profile)
    {
        Load();
        profiles[userId] = profile;
        Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);
        File.WriteAllText(statePath, JsonSerializer.Serialize(profiles, jsonOptions));
    }

    private void Load()
    {
        if (profiles.Count > 0 || !File.Exists(statePath))
        {
            return;
        }

        var stored = JsonSerializer.Deserialize<Dictionary<int, ProfilePreferences>>(File.ReadAllText(statePath), jsonOptions);
        if (stored is null)
        {
            return;
        }

        foreach (var item in stored)
        {
            profiles[item.Key] = item.Value;
        }
    }
}

public sealed class ProfilePreferences
{
    public string Pronoun { get; set; } = "Ele/dele";
    public string Level { get; set; } = "Iniciante";
    public string Instrument { get; set; } = "Violão";
    public string Professor { get; set; } = LearningContentStore.DemoProfessorName;
    public string Phone { get; set; } = "";
    public List<string> TeachingLevels { get; set; } = ["Iniciante", "Intermediário", "Avançado"];
    public List<string> TeachingInstruments { get; set; } = ["Violão", "Viola"];
}
