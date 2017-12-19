using SFML.Graphics;
using SFML.Window;
using System;

namespace Spacetris.GameStates.Menu
{
    public interface IMenu
    {
        event EventHandler<MenuItemType> MenuItemSelected;

        void Initialize(RenderWindow target);

        void DrawBackground(RenderWindow target);

        void DrawMenu(RenderWindow target);

        void Update(RenderWindow target, float deltaTime);

        bool EnableMenuItem(MenuItemType menuItem, bool enable, bool select = true);

        void KeyPressed(RenderWindow target, object sender, KeyEventArgs e);

        void KeyReleased(RenderWindow target, object sender, KeyEventArgs e);
    }
}
