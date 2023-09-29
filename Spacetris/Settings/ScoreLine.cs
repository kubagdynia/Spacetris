using System.Text.Json.Serialization;

namespace Spacetris.Settings;

public class ScoreLine
{
    public string Name { get; set; }
    public int Lines { get; set; }
    public int Level { get; set; }
    public int Score { get; set; }
    public DateTime CreatedUtc { get; set; }

    [JsonConstructor]
    public ScoreLine(string name, int lines, int level, int score, DateTime createdUtc)
    {
        Name = name;
        Lines = lines;
        Level = level;
        Score = score;
        CreatedUtc = createdUtc;
    }

    public ScoreLine(string name, int lines, int level, int score)
        : this(name, lines, level, score, DateTime.UtcNow)
    {

    }
}