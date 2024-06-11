using System.Text.Json.Serialization;

namespace Spacetris.Settings;

[method: JsonConstructor]
public class ScoreLine(string name, int lines, int level, int score, DateTime createdUtc)
{
    public string Name { get; set; } = name;
    public int Lines { get; set; } = lines;
    public int Level { get; set; } = level;
    public int Score { get; set; } = score;
    
    public DateTime CreatedUtc { get; set; } = createdUtc;

    public ScoreLine(string name, int lines, int level, int score)
        : this(name, lines, level, score, DateTime.UtcNow)
    {

    }
}