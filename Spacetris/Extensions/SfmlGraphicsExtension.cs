using SFML.Graphics;
using SFML.System;

namespace Spacetris.Extensions;

public static class SfmlGraphicsExtension
{
    public static Vector2f Center(this Text text, float x, float y)
    {
        FloatRect textRect = text.GetLocalBounds();

        text.Origin = new Vector2f(
            textRect.Left + textRect.Width / 2.0f,
            textRect.Top + textRect.Height / 2.0f);

        text.Position = new Vector2f(x, y);

        return new Vector2f(textRect.Width, textRect.Height);
    }

    public static Text Shadow(this Text text, int offset = 2, bool center = true)
    {
        Text shadowText = new Text
        {
            DisplayedString = text.DisplayedString,
            Font = text.Font,
            CharacterSize = text.CharacterSize,
            FillColor = new Color(0, 0, 0, 150),
            Style = text.Style,
            OutlineThickness = text.OutlineThickness,
            OutlineColor = new Color(0, 0, 0, 150),
        };

        if (center)
        {
            shadowText.Center(text.Position.X + offset, text.Position.Y + offset);
        }
        else
        {
            shadowText.Position = new Vector2f(text.Position.X + offset, text.Position.Y + offset);
        }

        return shadowText;
    }
}