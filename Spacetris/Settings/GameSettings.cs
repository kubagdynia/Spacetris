using Spacetris.Extensions;

namespace Spacetris.Settings;

public static class GameSettings
{
    public static event EventHandler<SettingsPropertyType> GameSettingsChanged;

    private const int TopScores = 5;

    private const string FileName = "settings.dat";

    private static bool _settingsShouldBeSaved;

    public static bool IsMusic
    {
        get => _settings.IsMusic;
        set
        {
            if (_settings.IsMusic != value)
            {
                _settings.IsMusic = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.IsMusic);
                MarkToSaveSettings();
            }
        }
    }

    public static bool IsSound
    {
        get => _settings.IsSound;
        set
        {
            if (_settings.IsSound != value)
            {
                _settings.IsSound = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.IsSound);
                MarkToSaveSettings();
            }
        }
    }

    public static byte MusicVolume
    {
        get => _settings.MusicVolume;
        set
        {
            if (_settings.MusicVolume != value)
            {
                _settings.MusicVolume = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.MusicVolume);
                MarkToSaveSettings();
            }
        }
    }

    public static byte SoundVolume
    {
        get => _settings.SoundVolume;
        set
        {
            if (_settings.SoundVolume != value)
            {
                _settings.SoundVolume = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.SoundVolume);
                MarkToSaveSettings();
            }
        }
    }

    private static List<ScoreLine> Scores => _settings.Scores;

    public static string FontsPath
    {
        get => _settings.FontsPath;
        set
        {
            if (_settings.FontsPath != value)
            {
                _settings.FontsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.FontsPath);
                MarkToSaveSettings();
            }
        }
    }

    public static string TilesetsPath
    {
        get => _settings.TilesetsPath;
        set
        {
            if (_settings.TilesetsPath != value)
            {
                _settings.TilesetsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.TilesetsPath);
                MarkToSaveSettings();
            }
        }
    }

    public static string BackgroundPath
    {
        get => _settings.BackgroundPath;
        set
        {
            if (_settings.BackgroundPath != value)
            {
                _settings.BackgroundPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.BackgroundPath);
                MarkToSaveSettings();
            }
        }
    }

    public static string SoundsPath
    {
        get => _settings.SoundsPath;
        set
        {
            if (_settings.SoundsPath != value)
            {
                _settings.SoundsPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.SoundsPath);
                MarkToSaveSettings();
            }
        }
    }

    public static string MusicPath
    {
        get => _settings.MusicPath;
        set
        {
            if (_settings.MusicPath != value)
            {
                _settings.MusicPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.MusicPath);
                MarkToSaveSettings();
            }
        }
    }

    public static string ImagesPath
    {
        get => _settings.ImagesPath;
        set
        {
            if (_settings.ImagesPath != value)
            {

                _settings.ImagesPath = value;
                GameSettingsChanged?.Invoke(null, SettingsPropertyType.ImagesPath);
                MarkToSaveSettings();
            }
        }
    }

    private static Settings _settings = new();

    public static void Save()
    {
        DataOperations.SaveData(_settings, FileName);
        _settingsShouldBeSaved = false;
    }

    public static void Load()
    {
        var settings = DataOperations.LoadData<Settings>(FileName, out bool fileExists);
        if (settings != null)
        {
            _settings = settings;
        }
        else if (!fileExists)
        {
#if DEBUG
            "Create settings file".Log();
#endif
            // If file is not exists then create file with default data
            Save();
        }

        // Sort and crop the scores
        if (_settings.Scores.Count > TopScores)
        {
            _settings.Scores = Scores.OrderByDescending(c => c.Score).ThenByDescending(c => c.Level).ThenByDescending(c => c.Lines).Take(TopScores).ToList();
            GameSettingsChanged?.Invoke(null, SettingsPropertyType.Scores);
            Save();
        }
        else
        {
            _settings.Scores = Scores.OrderByDescending(c => c.Score).ThenByDescending(c => c.Level).ThenByDescending(c => c.Lines).ToList();
        }

        _settingsShouldBeSaved = false;
    }

    public static bool IsEnoughPointsForTopScores(int score)
    {
        if (Scores.Count < TopScores)
        {
            return true;
        }

        bool isEnoughPoints = Scores.OrderByDescending(c => c.Score).Take(TopScores).Any(c => c.Score < score);

        return isEnoughPoints;
    }

    public static void AddScore(ScoreLine score)
    {
        if (score.Score <= 0 || string.IsNullOrWhiteSpace(score.Name))
        {
            return;
        }

        if (IsEnoughPointsForTopScores(score.Score))
        {
            Scores.Add(score);
            _settings.Scores = Scores.OrderByDescending(c => c.Score).ThenByDescending(c => c.Level).ThenByDescending(c => c.Lines).Take(TopScores).ToList();
            GameSettingsChanged?.Invoke(null, SettingsPropertyType.Scores);
            Save();
        }
    }

    public static List<ScoreLine> GetScores()
    {
        return Scores;
    }

    public static void CleanUp()
    {
        if (_settingsShouldBeSaved)
        {
            Save();
        }
    }

    private static void MarkToSaveSettings()
    {
        _settingsShouldBeSaved = true;
    }
}