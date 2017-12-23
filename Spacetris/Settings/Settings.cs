﻿using System.Collections.Generic;

namespace Spacetris.Settings
{
    public class Settings
    {
        public bool IsMusic { get; set; } = true;
        public bool IsSound { get; set; } = true;
        public byte MusicVolume { get; set; } = 30;
        public byte SoundVolume { get; set; } = 50;
        public List<ScoreLine> Scores { get; set; } = new List<ScoreLine>();
        public string FontsPath { get; set; } = "Content/Fonts/";
        public string TilesetsPath { get; set; } = "Content/Tilesets/";
        public string BackgroundPath { get; set; } = "Content/Background/";
        public string SoundsPath { get; set; } = "Content/Sounds/";
        public string MusicPath { get; set; } = "Content/Music/";
    }
}
