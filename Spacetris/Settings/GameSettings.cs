using System;
using System.Collections.Generic;
using System.Linq;

namespace Spacetris.Settings
{
    public static class GameSettings
    {
        private const string FileName = "settings.dat";

        public static bool IsMusic
        {
            get => _settings.IsMusic;
            set => _settings.IsMusic = value;
        }

        public static bool IsSound
        {
            get => _settings.IsSound;
            set => _settings.IsSound = value;
        }

        public static byte MusicVolume
        {
            get => _settings.MusicVolume;
            set => _settings.MusicVolume = value;
        }

        public static byte SoundVolume
        {
            get => _settings.SoundVolume;
            set => _settings.SoundVolume = value;
        }

        public static List<ScoreLine> Scores
        {
            get => _settings.Scores;
        }

        public static string FontsPath
        {
            get => _settings.FontsPath;
            set => _settings.FontsPath = value;
        }

        public static string TilesetsPath
        {
            get => _settings.TilesetsPath;
            set => _settings.TilesetsPath = value;
        }

        public static string BackgroundPath
        {
            get => _settings.BackgroundPath;
            set => _settings.BackgroundPath = value;
        }

        public static string SoundsPath
        {
            get => _settings.SoundsPath;
            set => _settings.SoundsPath = value;
        }

        public static string MusicPath
        {
            get => _settings.MusicPath;
            set => _settings.MusicPath = value;
        }

        private static Settings _settings = new Settings();

        public static void Save()
        {
            DataOperations.SaveData(_settings, FileName);
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
                Console.WriteLine("Create settings file");
#endif
                // If file is not exists then create file with default data
                Save();
            }
        }

        public static bool IsEnoughPointsForTop5(int score)
        {
            if (Scores.Count < 5)
            {
                return true;
            }

            bool isEnoughPoints = Scores.OrderByDescending(c => c.Score).Take(5).Any(c => c.Score < score);

            return isEnoughPoints;
        }

        public static void AddScore(ScoreLine score)
        {
            if (score == null || score.Score <= 0 || string.IsNullOrWhiteSpace(score.Name))
            {
                return;
            }

            if (IsEnoughPointsForTop5(score.Score))
            {
                Scores.Add(score);
                _settings.Scores = Scores.OrderByDescending(c => c.Score).Take(5).ToList();
                Save();
            }
        }
    }
}
