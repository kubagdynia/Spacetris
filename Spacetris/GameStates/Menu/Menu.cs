using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.BackgroundEffects;
using Spacetris.DataStructures;
using Spacetris.Extensions;
using Spacetris.GameStates.Worlds;
using Spacetris.Managers;
using Spacetris.Settings;

namespace Spacetris.GameStates.Menu;

public partial class Menu : BaseGameState, IMenu
{
    private const string GameName = "Spacetris";

    private const int YesNoVolumeStep = 2;

    public event EventHandler<MenuItemType> MenuItemSelected;

    private IBackgroundEffects _starfield;
    private int _centerX;

    private static Point2 _scoreOffset = Point2.Zero;
    private int _scoreOffsetStep = 1;
    private readonly int _scoreOffsetMin = -10;
    private readonly int _scoreOffsetMax = 10;

    private const float TotalDelay = 0.05f;
    private float _totalTimer;

    private const int MenuFirstItemPositionY = 200;
    private const int MenuNextItemOffsetPositionY = 70;

    private int _menuTitleSize = 140;
    private const int MenuTitleSizeMin = 130;
    private const int MenuTitleSizeMax = 150;
    private int _menuTitleSizeStep = 1;

    private int _menuMadeByAlphaStep = 2;

    private Sprite _gameControllerSprite;
    private Sprite _controlsSprite;

    private static readonly Color MenuTitleColor = new(255, 216, 48, 189);
    private static readonly Color MenuItemsColor = new(242, 51, 51, 189);
    private static readonly Color MenuItemsColorDark = new(153, 33, 33, 109);
    private static readonly Color ScoresColor = new(255, 55, 55, 189);
    private static readonly Color ScoresColor2 = new(255, 216, 48, 89);
    private static Color _madeByColor = new(255, 216, 48, 0);

    private readonly string[] _madeByList =
    [
        "Game made by kubagdynia : https://github.com/kubagdynia/Spacetris",
        "Music \"Happy 8bit Loop 01\" by Tristan Lohengrin : http://tristanlohengrin.wixsite.com/studio"
    ];
    
    private int _myByListIndex;

    private MenuItem _selectedMenuItem;

    private static Sound _menuSoundBeep;
    private static Sound _menuSoundSelect;

    private static Font TitleFont => AssetManager.Instance.Font.Get(AssetManagerItemName.TetrisFont);

    private static Font ItemFont => AssetManager.Instance.Font.Get(AssetManagerItemName.SlkscrFont);

    private static Font MadeByFont => AssetManager.Instance.Font.Get(AssetManagerItemName.ArialFont);

    private readonly MenuItem[] _menuItems =
    [
        new MenuItem(MenuItemType.NewGane, 0, 1),
        new MenuItem(MenuItemType.Continue, 0, 2, false),
        new MenuItem("High Scores", MenuItemType.Scores, 0, 3)
        {
            SubMenuItems =
            [
                new MenuItem(MenuItemType.ScoresDetails, 0, 1, MenuItemType.Scores, MenuItemFunctionType.CustomPage, ScoresCustomSection()),
                new MenuItem(MenuItemType.Back, 200, 2, MenuItemType.Scores)
            ]
        },
        new MenuItem(MenuItemType.Config, 0, 4)
        {
            SubMenuItems =
            [
                new MenuItem(MenuItemType.Sound, 0, 1, MenuItemType.Config, MenuItemFunctionType.YesNo, SwitchYesNoMenuItem(MenuItemType.Sound)),
                new MenuItem(MenuItemType.Music, 0, 2, MenuItemType.Config, MenuItemFunctionType.YesNo, SwitchYesNoMenuItem(MenuItemType.Music)),
                new MenuItem(MenuItemType.Back, 0, 3, MenuItemType.Config)
            ]
        },
        new MenuItem(MenuItemType.Quit, 0, 5)
    ];

    public Menu()
    {
        RecalculateMenuItemsPosition(_menuItems);

        _selectedMenuItem = _menuItems.Single(c => c.Position == 1);
    }

    public void Initialize(RenderWindow target)
    {
        _starfield = new Starfield(target.Size.X, target.Size.Y);
        _centerX = (int)target.Size.X / 2;

        GetMusic().Volume = GameSettings.MusicVolume;
        GetMusic().Loop = true;
        if (GameSettings.IsMusic)
        {
            GetMusic().Play();
        }

        GameSettings.GameSettingsChanged += GameSettingsChanged;
    }

    protected override void LoadContent()
    {
        // Load sounds
        _menuSoundBeep = LoadSound("beep.wav");
        _menuSoundSelect = LoadSound("select.wav");

        // Load sprites
        _gameControllerSprite = LoadGameControllerSprite();
        _controlsSprite = LoadControlsControllerSprite();
    }

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
        var rectangle = new RectangleShape(new Vector2f(285 + (menuItem.Parent != MenuItemType.None ? 200 : 0), 65))
        {
            Position = new Vector2f(_centerX - 145 - (menuItem.Parent != MenuItemType.None ? 100 : 0), menuItem.Y - 30),
            FillColor = new Color(100, 100, 100, 50)
        };

        target.Draw(rectangle);
    }

    private MenuItem[] GetMenuItems()
    {
        MenuItem[] result;

        if (_selectedMenuItem != null && _selectedMenuItem.Parent != MenuItemType.None)
        {
            // Get sub-menu items
            result = _menuItems.SingleOrDefault(c => c.Enable && c.Item == _selectedMenuItem.Parent)?.SubMenuItems;

        }
        else
        {
            // Get menu items
            result = _menuItems.Where(c => c.Enable).ToArray();
        }

        return result;
    }

    public void Update(RenderWindow target, float deltaTime)
    {
        _totalTimer += deltaTime;

        if (_totalTimer > TotalDelay)
        {
            // Move menu title
            _menuTitleSize += _menuTitleSizeStep;
            if (_menuTitleSize is > MenuTitleSizeMax or < MenuTitleSizeMin)
            {
                _menuTitleSizeStep = -_menuTitleSizeStep;
            }

            if (_selectedMenuItem.Parent == MenuItemType.Scores)
            {
                // Move score box
                _scoreOffset.X += _scoreOffsetStep;
                if (_scoreOffset.X > _scoreOffsetMax || _scoreOffset.X < _scoreOffsetMin)
                {
                    _scoreOffsetStep = -_scoreOffsetStep;
                }
            }

            // Update alpha channel of "Made by"
            if (_madeByColor.A is >= 155 or 0)
            {
                _menuMadeByAlphaStep = -_menuMadeByAlphaStep;
            }

            var newAlphaValue = _madeByColor.A - _menuMadeByAlphaStep;
            if (newAlphaValue <= 0)
            {
                newAlphaValue = 0;

                // Switch "Made by" item
                if (++_myByListIndex + 1 > _madeByList.Length)
                {
                    _myByListIndex = 0;
                }
            }

            _madeByColor = new Color(_madeByColor.R, _madeByColor.G, _madeByColor.B, Convert.ToByte(newAlphaValue));

            _totalTimer = 0;
        }

        _starfield.Update(deltaTime);
    }

    public bool EnableMenuItem(MenuItemType menuItem, bool enable, bool select = true)
    {
        var item = _menuItems.FirstOrDefault(c => c.Item == menuItem);

        if (item == null)
        {
            return false;
        }

        if (item.Enable == enable)
        {
            return true;
        }

        item.Enable = enable;

        RecalculateMenuItemsPosition(_menuItems);

        _selectedMenuItem = select ? item : _menuItems.Where(c => c.Enable).MinBy(c => c.Position);

        return true;
    }

    public void KeyPressed(RenderWindow target, object sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Down or Keyboard.Key.S or Keyboard.Key.Up or Keyboard.Key.W or Keyboard.Key.Escape:
            {
                var nextSelectedMenuItem = e.Code switch
                {
                    Keyboard.Key.Down or Keyboard.Key.S => GetMenuItems()
                        .FirstOrDefault(c =>
                            c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage &&
                            c.Position > _selectedMenuItem.Position),
                    Keyboard.Key.Up or Keyboard.Key.W => GetMenuItems()
                        .OrderByDescending(c => c.Position)
                        .FirstOrDefault(c =>
                            c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage &&
                            c.Position < _selectedMenuItem.Position),
                    Keyboard.Key.Escape when _selectedMenuItem.Parent != MenuItemType.None =>
                        _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent),
                    _ => _selectedMenuItem
                };

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }

                break;
            }
            
            // Left
            case Keyboard.Key.Left when _selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo:
                _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), -YesNoVolumeStep);
                break;
            
            // Right
            case Keyboard.Key.Right when _selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo:
                _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), YesNoVolumeStep);
                break;
            
            // Enter
            case Keyboard.Key.Enter:
            {
                if (_selectedMenuItem.Item == MenuItemType.Back && _selectedMenuItem.Parent != MenuItemType.None)
                {
                    _selectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }
                else if (_selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo)
                {
                    _selectedMenuItem.FunctionObject?.Invoke(!(bool)_selectedMenuItem.FunctionObject(null, null), null);
                }
                else if (_selectedMenuItem.SubMenuItems != null)
                {
#if DEBUG
                    "SUB Menu".Log();
#endif
                    _scoreOffset = Point2.Zero;
                    _scoreOffsetStep = 1;
                    _selectedMenuItem = _selectedMenuItem.SubMenuItems.OrderBy(c => c.Position).FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage);
                }

                if (_selectedMenuItem != null)
                {
                    PlaySound(_menuSoundSelect);
                    MenuItemSelected?.Invoke(this, _selectedMenuItem.Item);
                }

                break;
            }
        }
    }

    public void KeyReleased(RenderWindow target, object sender, KeyEventArgs e)
    {

    }

    public void JoystickConnected(object sender, JoystickConnectEventArgs arg)
    {
#if DEBUG
        $"Controller connected: {arg.JoystickId}".Log();
#endif            
    }

    public void JoystickDisconnected(object sender, JoystickConnectEventArgs arg)
    {
#if DEBUG
        $"Controller disconnected: {arg.JoystickId}".Log();
#endif
    }

    public void JoystickButtonPressed(RenderWindow target, object sender, JoystickButtonEventArgs arg)
    {
#if DEBUG
        $"Controller ({arg.JoystickId}) Button Pressed: {arg.Button})".Log();
#endif
        switch (arg.Button)
        {
            // Press A button
            case 0:
            {
                if (_selectedMenuItem.Item == MenuItemType.Back && _selectedMenuItem.Parent != MenuItemType.None)
                {
                    _selectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }
                else if (_selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo)
                {
                    _selectedMenuItem.FunctionObject?.Invoke(!(bool)_selectedMenuItem.FunctionObject(null, null), null);
                }
                else if (_selectedMenuItem.SubMenuItems != null)
                {
#if DEBUG
                    "SUB Menu".Log();
#endif
                    _scoreOffset = Point2.Zero;
                    _scoreOffsetStep = 1;
                    _selectedMenuItem = _selectedMenuItem.SubMenuItems.OrderBy(c => c.Position).FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage);
                }

                if (_selectedMenuItem != null)
                {
                    PlaySound(_menuSoundSelect);
                    MenuItemSelected?.Invoke(this, _selectedMenuItem.Item);
                }

                break;
            }
            // Press B button
            case 1:
            {
                var nextSelectedMenuItem = _selectedMenuItem;

                if (_selectedMenuItem.Parent != MenuItemType.None)
                {
                    nextSelectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }

                break;
            }
        }
    }

    public void JoystickButtonReleased(RenderWindow target, object sender, JoystickButtonEventArgs arg)
    {
#if DEBUG
        $"Controller ({arg.JoystickId}) Button Released: {arg.Button})".Log();
#endif
    }

    public void JoystickMoved(RenderWindow target, object sender, JoystickMoveEventArgs arg)
    {
#if DEBUG
        $"Controller ({arg.JoystickId}) Moved: Axis({arg.Axis}), Position({arg.Position})".Log();
#endif
        switch (arg.Axis)
        {
            // Move Down
            case Joystick.Axis.PovY when Math.Abs(arg.Position + 100) < GamepadMinimumInputTolerance:
            {
                var nextSelectedMenuItem =
                    GetMenuItems().OrderBy(c => c.Position).FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position > _selectedMenuItem.Position);

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }

                break;
            }
            // Move Up
            case Joystick.Axis.PovY when Math.Abs(arg.Position - 100) < GamepadMinimumInputTolerance:
            {
                var nextSelectedMenuItem =
                    GetMenuItems().OrderByDescending(c => c.Position).FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position < _selectedMenuItem.Position);

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }

                break;
            }
            // Move Left
            default:
            {
                switch (_selectedMenuItem.FunctionType)
                {
                    case MenuItemFunctionType.YesNo when
                        arg.Axis == Joystick.Axis.PovX && Math.Abs(arg.Position + 100) < GamepadMinimumInputTolerance:
                        _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), -YesNoVolumeStep);
                        break;
                    // Move Right
                    case MenuItemFunctionType.YesNo when
                        arg.Axis == Joystick.Axis.PovX && Math.Abs(arg.Position - 100) < GamepadMinimumInputTolerance:
                        _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), YesNoVolumeStep);
                        break;
                }

                break;
            }
        }
    }

    private static Music GetMusic()
    {
        return AssetManager.Instance.Music.Get(AssetManagerItemName.Music01);
    }

    private void RecalculateMenuItemsPosition(MenuItem[] items)
    {
        if (items == null || items.Length == 0)
        {
            return;
        }

        var index = 0;
        var enableItems = items.OrderBy(c => c.Position).Where(c => c.Enable);

        var menuItems = enableItems as IList<MenuItem> ?? enableItems.ToList();
        if (menuItems.Any(c => c.FunctionType == MenuItemFunctionType.CustomPage))
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Y = MenuFirstItemPositionY + MenuNextItemOffsetPositionY * 4 * index++;
            }
        }
        else
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Y = MenuFirstItemPositionY + MenuNextItemOffsetPositionY * index++;
                RecalculateMenuItemsPosition(menuItem.SubMenuItems);
            }
        }
    }

    private static bool IsNewValue(object arg1, object arg2, byte value, int minValue, int maxValue, out byte newValue)
    {
        newValue = 0;

        if ((bool)arg1)
        {
            var tmpValue = value + (int)arg2;
            if (tmpValue < minValue)
            {
                tmpValue = minValue;
            }
            else if (tmpValue > maxValue)
            {
                tmpValue = maxValue;
            }

            if (tmpValue != value)
            {
                newValue = Convert.ToByte(tmpValue);
                return true;
            }

            return false;
        }

        return false;
    }

    private static void GameSettingsChanged(object sender, SettingsPropertyType settingsPropertyType)
    {
        switch (settingsPropertyType)
        {
            case SettingsPropertyType.IsMusic:
#if DEBUG
                $"IsMusic changed: {GameSettings.IsMusic}".Log();
#endif
                break;
            case SettingsPropertyType.IsSound:
#if DEBUG
                $"IsSound changed: {GameSettings.IsSound}".Log();
#endif
                break;
            case SettingsPropertyType.MusicVolume:
#if DEBUG
                $"Music volume changed: {GameSettings.MusicVolume}".Log();
#endif
                GetMusic().Volume = GameSettings.MusicVolume;
                break;
            case SettingsPropertyType.SoundVolume:
#if DEBUG
                $"Sound volume changed: {GameSettings.SoundVolume}".Log();
#endif
                break;
            case SettingsPropertyType.FontsPath:
#if DEBUG
                $"Fonts path changed: {GameSettings.FontsPath}".Log();
#endif
                break;
            case SettingsPropertyType.TilesetsPath:
#if DEBUG
                $"Tilesets path changed: {GameSettings.TilesetsPath}".Log();
#endif
                break;
            case SettingsPropertyType.BackgroundPath:
#if DEBUG
                $"Background path changed: {GameSettings.BackgroundPath}".Log();
#endif
                break;
            case SettingsPropertyType.SoundsPath:
#if DEBUG
                $"Sounds path changed: {GameSettings.SoundsPath}".Log();
#endif
                break;
            case SettingsPropertyType.MusicPath:
#if DEBUG
                $"Music path changed: {GameSettings.MusicPath}".Log();
#endif
                break;
            case SettingsPropertyType.ImagesPath:
#if DEBUG
                $"Images path changed: {GameSettings.ImagesPath}".Log();
#endif
                break;
            case SettingsPropertyType.Scores:
#if DEBUG
                $"Scores changed: {GameSettings.GetScores().Count()}".Log();
#endif
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(settingsPropertyType));
        }
    }
}