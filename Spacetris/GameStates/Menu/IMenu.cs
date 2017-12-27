using SFML.Graphics;
using System;

namespace Spacetris.GameStates.Menu
{
    public interface IMenu : IGameInput
    {
        event EventHandler<MenuItemType> MenuItemSelected;

        void Initialize(RenderWindow target);

        void DrawBackground(RenderWindow target);

        void DrawMenu(RenderWindow target);

        void Update(RenderWindow target, float deltaTime);

        bool EnableMenuItem(MenuItemType menuItem, bool enable, bool select = true);
    }
}
