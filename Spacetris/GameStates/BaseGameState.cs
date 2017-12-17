using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.Extensions;
using System.Collections.Generic;

namespace Spacetris.GameStates
{
    public abstract class BaseGameState
    {
        public static readonly Dictionary<Keyboard.Key, string> AllowedKeyboardChars = new Dictionary<Keyboard.Key, string>
        {
            { Keyboard.Key.A, "A" },
            { Keyboard.Key.B, "B" },
            { Keyboard.Key.C, "C" },
            { Keyboard.Key.D, "D" },
            { Keyboard.Key.E, "E" },
            { Keyboard.Key.F, "F" },
            { Keyboard.Key.G, "G" },
            { Keyboard.Key.H, "H" },
            { Keyboard.Key.I, "I" },
            { Keyboard.Key.J, "J" },
            { Keyboard.Key.K, "K" },
            { Keyboard.Key.L, "L" },
            { Keyboard.Key.M, "M" },
            { Keyboard.Key.N, "N" },
            { Keyboard.Key.O, "O" },
            { Keyboard.Key.P, "P" },
            { Keyboard.Key.Q, "Q" },
            { Keyboard.Key.R, "R" },
            { Keyboard.Key.S, "S" },
            { Keyboard.Key.T, "T" },
            { Keyboard.Key.U, "U" },
            { Keyboard.Key.V, "V" },
            { Keyboard.Key.W, "W" },
            { Keyboard.Key.X, "X" },
            { Keyboard.Key.Y, "Y" },
            { Keyboard.Key.Z, "Z" },
            { Keyboard.Key.Num0, "0" },
            { Keyboard.Key.Num1, "1" },
            { Keyboard.Key.Num2, "2" },
            { Keyboard.Key.Num3, "3" },
            { Keyboard.Key.Num4, "4" },
            { Keyboard.Key.Num5, "5" },
            { Keyboard.Key.Num6, "6" },
            { Keyboard.Key.Num7, "7" },
            { Keyboard.Key.Num8, "8" },
            { Keyboard.Key.Num9, "9" },
            { Keyboard.Key.Space, " " },
            { Keyboard.Key.Slash, "/" },
            { Keyboard.Key.Dash, "-" }
        };

        protected static void DrawText(RenderTarget target, Font font, string value, float x, float y,
            Color color, int size, bool bold = true, bool center = false, bool drawShadow = true)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var text = new Text()
            {
                DisplayedString = value,
                Font = font,
                CharacterSize = (uint)size,
                FillColor = color,
                Style = bold ? Text.Styles.Bold : Text.Styles.Regular,
                OutlineThickness = 1,
                OutlineColor = new Color(0, 0, 0, 189),
            };
            if (center)
            {
                text.Center(x, y);
            }
            else
            {
                text.Position = new Vector2f(x, y);
            }

            if (drawShadow)
            {
                target.Draw(text.Shadow(2, center));
            }
            target.Draw(text);
        }

        protected virtual void LoadContent()
        {

        }
    }
}
