using SFML.Graphics;
using SFML.System;
using Spacetris.DataStructures;
using Spacetris.Settings;

namespace Spacetris.GameStates.Menu;

public partial class Menu
{
    private static Func<object, object, object> ScoresCustomSection()
    {
        return (arg1, _) =>
        {
            if (arg1 is not RenderTarget target)
            {
                return null;
            }

            var scorePosition = new Point2(100 + _scoreOffset.X, 180 + _scoreOffset.Y);

            var rectangle = new RectangleShape(new Vector2f(750, 250))
            {
                Position = new Vector2f(scorePosition.X, scorePosition.Y),
                FillColor = new Color(100, 100, 100, 50),
                OutlineColor = new Color(100, 100, 100, 100),
                OutlineThickness = 3
            };

            target.Draw(rectangle);

            DrawText(target, ItemFont, "Rank", scorePosition.X + 10, scorePosition.Y + 5, ScoresColor2, 20);

            DrawText(target, ItemFont, "Name", scorePosition.X + 100, scorePosition.Y + 5, ScoresColor2, 20);
            DrawText(target, ItemFont, "Scores", scorePosition.X + 440, scorePosition.Y + 5, ScoresColor2, 20);
            DrawText(target, ItemFont, "Lines", scorePosition.X + 570, scorePosition.Y + 5, ScoresColor2, 20);
            DrawText(target, ItemFont, "Level", scorePosition.X + 670, scorePosition.Y + 5, ScoresColor2, 20);

            var offset = 1;
            foreach (ScoreLine scoreLine in GameSettings.GetScores())
            {
                DrawText(target, ItemFont, offset.ToString(), scorePosition.X + 40, scorePosition.Y + 13 + offset * 40,
                    ScoresColor, 20, true, true);
                DrawText(target, ItemFont, scoreLine.Name, scorePosition.X + 100, scorePosition.Y + offset * 40,
                    ScoresColor, 20);
                DrawText(target, ItemFont, scoreLine.Score.ToString(), scorePosition.X + 483,
                    scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);
                DrawText(target, ItemFont, scoreLine.Lines.ToString(), scorePosition.X + 602,
                    scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);
                DrawText(target, ItemFont, scoreLine.Level.ToString(), scorePosition.X + 700,
                    scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);

                offset++;
            }

            return null;
        };
    }
}