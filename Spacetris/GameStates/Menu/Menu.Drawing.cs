using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.GameStates.Worlds;
using Spacetris.Settings;

namespace Spacetris.GameStates.Menu;

public partial class Menu
{
    public void DrawBackground(RenderWindow target)
        => target.Draw(_starfield);

    public void DrawMenu(RenderWindow target)
    {
        DrawText(target, MadeByFont, _madeByList[_myByListIndex], _centerX, 155, _madeByColor, 10, true, true);

        // Draw menu title
        DrawText(target, TitleFont, GameName, _centerX, 100, MenuTitleColor, _menuTitleSize, true, true);

        // Draw menu items
        foreach (var menuItem in GetMenuItems())
        {
            if (_selectedMenuItem.Item == menuItem.Item)
            {
                // Draw selected item
                DrawItemShadow(target, menuItem);
                DrawMenuItem(target, menuItem, _centerX, MenuItemsColor, 50);
            }
            else
            {
                // Drawable not selected item
                DrawMenuItem(target, menuItem, _centerX, MenuItemsColor, 40, false);
            }
        }
    }

    public void DrawGameController(RenderWindow target)
    {
        if (Joystick.IsConnected(0))
        {
            target.Draw(_gameControllerSprite);
        }
    }

    public void DrawControls(RenderWindow target)
    {
        if (_selectedMenuItem.Parent == MenuItemType.None)
        {
            target.Draw(_controlsSprite);
        }
    }

    public void DrawAllLayers(RenderWindow target, IWorld world)
    {
        DrawBackground(target);
        if (world.WorldState == WorldState.Pause)
        {
            //world.DrawWorld(target, false, 15);
        }
        DrawMenu(target);
        DrawGameController(target);
        DrawControls(target);
    }

    private void DrawMenuItem(RenderTarget target, MenuItem menuItem, int x, Color color, int size, bool bold = true)
    {
        if (menuItem.FunctionType == MenuItemFunctionType.CustomPage)
        {
            menuItem.FunctionObject?.Invoke(target, null);
            return;
        }

        var text = menuItem.Name;

        if (menuItem.FunctionType == MenuItemFunctionType.YesNo)
        {
            if (menuItem.FunctionObject != null)
            {
                text += (bool)menuItem.FunctionObject(null, null) ? " Yes" : " No";
            }
        }

        DrawText(target, TitleFont, text, x, menuItem.Y, color, size, bold, true);

        // Draw volume bar
        if (menuItem.FunctionType == MenuItemFunctionType.YesNo)
        {
            var rectSize = menuItem.Item switch
            {
                MenuItemType.Sound => GameSettings.SoundVolume,
                MenuItemType.Music => GameSettings.MusicVolume,
                _ => 0
            };

            if (rectSize < 1)
            {
                rectSize = 1;
            }

            var rect = new RectangleShape(new Vector2f(rectSize / 50f * size, 10))
            {
                Position = new Vector2f(x + (bold ? 130 : 105), menuItem.Y),
                FillColor = bold ? MenuItemsColor : MenuItemsColorDark,
                OutlineColor = MenuItemsColorDark,
                OutlineThickness = 0
            };

            target.Draw(rect);
        }
    }

    private void DrawItemShadow(RenderWindow target, MenuItem menuItem)
    {
        var rectangle = new RectangleShape(new Vector2f(MenuItemShadowWidth + (menuItem.Parent != MenuItemType.None ? 200 : 0), MenuItemShadowHeight))
        {
            Position = new Vector2f(_centerX - MenuItemShadowOffsetX - (menuItem.Parent != MenuItemType.None ? 100 : 0), menuItem.Y - MenuItemShadowOffsetY),
            FillColor = new Color(100, 100, 100, 50)
        };

        target.Draw(rectangle);
    }
}
