using SFML.Graphics;
using Spacetris.GameStates.Worlds;

namespace Spacetris.GameStates.Menu;

public interface IMenu : IGameInput
{
    event EventHandler<MenuItemType> MenuItemSelected;

    void Initialize(RenderWindow target);

    void DrawBackground(RenderWindow target);

    void DrawMenu(RenderWindow target);

    void DrawGameController(RenderWindow target);

    void DrawControls(RenderWindow target);

    void DrawAllLayers(RenderWindow target, IWorld world);

    void Update(RenderWindow target, float deltaTime);

    bool EnableMenuItem(MenuItemType menuItem, bool enable, bool select = true);
}