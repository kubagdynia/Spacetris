using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using Spacetris.BackgroundEffects;
using Spacetris.DataStructures;
using Spacetris.Settings;
using SFML.Audio;
using Spacetris.Extensions;
using Spacetris.Managers;

namespace Spacetris.GameStates.Menu
{
    public class Menu : BaseGameState, IMenu
    {
        private const string GameName = "Spacetris";

        public event EventHandler<MenuItemType> MenuItemSelected;

        private Starfield _starfield;
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

        private static readonly Color MenuTitleColor = new Color(255, 216, 48, 189);
        private static readonly Color MenuItemsColor = new Color(242, 51, 51, 189);
        private static readonly Color ScoresColor = new Color(255, 55, 55, 189);
        private static readonly Color ScoresColor2 = new Color(255, 216, 48, 89);
        private static Color MadeByColor = new Color(255, 216, 48, 0);

        private readonly string[] _madeByList =
        {
            "Game made by kubagdynia : https://github.com/kubagdynia/Spacetris",
            "Music \"Happy 8bit Loop 01\" by Tristan Lohengrin : http://tristanlohengrin.wixsite.com/studio"
        };
        private int _myByListIndex = 0;

        private MenuItem _selectedMenuItem;

        private static Sound _menuSoundBeep;
        private static Sound _menuSoundSelect;

        private static Font TitleFont
        {
            get
            {
                return AssetManager.Instance.Font.Get("tetris");
            }
        }

        private static Font ItemFont
        {
            get
            {
                return AssetManager.Instance.Font.Get("slkscr");
            }
        }

        private static Font MadeByFont
        {
            get
            {
                return AssetManager.Instance.Font.Get("arial");
            }
        }

        private readonly MenuItem[] _menuItems = new MenuItem[]
        {
            new MenuItem(MenuItemType.NewGane, 0, 1),
            new MenuItem(MenuItemType.Continue, 0, 2, false),
            new MenuItem("High Scores", MenuItemType.Scores, 0, 3)
            {
                SubMenuItems = new[]
                {
                    new MenuItem(MenuItemType.ScoresDetails, 0, 1, MenuItemType.Scores, MenuItemFunctionType.CustomPage, ScoresCustomSection()),
                    new MenuItem(MenuItemType.Back, 200, 2, MenuItemType.Scores),
                }
            },
            new MenuItem(MenuItemType.Config, 0, 4)
            {
                SubMenuItems = new[]
                {
                    new MenuItem(MenuItemType.Sound, 0, 1, MenuItemType.Config, MenuItemFunctionType.YesNo, SwitchYesNoMenuItem(MenuItemType.Sound)),
                    new MenuItem(MenuItemType.Music, 0, 2, MenuItemType.Config, MenuItemFunctionType.YesNo, SwitchYesNoMenuItem(MenuItemType.Music)),
                    new MenuItem(MenuItemType.Back, 0, 3, MenuItemType.Config),
                }
            },
            new MenuItem(MenuItemType.Quit, 0, 5)
        };

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
        }

        protected override void LoadContent()
        {
            // Load sounds
            _menuSoundBeep = LoadSound("beep.wav");
            _menuSoundSelect = LoadSound("select.wav");
        }

        public void DrawBackground(RenderWindow target)
        {
            target.Draw(_starfield);
        }

        public void DrawMenu(RenderWindow target)
        {
            DrawText(target, MadeByFont, _madeByList[_myByListIndex], _centerX, 155, MadeByColor, 10, true, true);

            // Draw menu title
            DrawText(target, TitleFont, GameName, _centerX, 100, MenuTitleColor, _menuTitleSize, true, true);

            // Draw menu items
            foreach (MenuItem menuItem in GetMenuItems())
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

        private void DrawMenuItem(RenderTarget target, MenuItem menuItem, int x, Color color, int size, bool bold = true)
        {
            if (menuItem.FunctionType == MenuItemFunctionType.CustomPage)
            {
                menuItem.FunctionObject?.Invoke(target);
                return;
            }

            var text = menuItem.Name;

            if (menuItem.FunctionType == MenuItemFunctionType.YesNo)
            {
                if (menuItem.FunctionObject != null)
                {
                    text += (bool)menuItem.FunctionObject(null) ? " Yes" : " No";
                }
            }

            DrawText(target, TitleFont, text, x, menuItem.Y, color, size, bold, true);
        }

        private void DrawItemShadow(RenderTarget target, MenuItem menuItem)
        {
            var rectangle = new RectangleShape(new Vector2f(285 + (menuItem.Parent != MenuItemType.None ? 100 : 0), 65))
            {
                Position = new Vector2f(_centerX - 145 - (menuItem.Parent != MenuItemType.None ? 50 : 0),
                    menuItem.Y - 30),
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
                if (_menuTitleSize > MenuTitleSizeMax || _menuTitleSize < MenuTitleSizeMin)
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
                if (MadeByColor.A >= 155 || MadeByColor.A == 0)
                {
                    _menuMadeByAlphaStep = -_menuMadeByAlphaStep;
                }

                int newAlphaValue = MadeByColor.A - _menuMadeByAlphaStep;
                if (newAlphaValue <= 0)
                {
                    newAlphaValue = 0;

                    // Switch "Made by" item
                    if (++_myByListIndex + 1 > _madeByList.Length)
                    {
                        _myByListIndex = 0;
                    }
                }

                MadeByColor = new Color(MadeByColor.R, MadeByColor.G, MadeByColor.B, Convert.ToByte(newAlphaValue));

                _totalTimer = 0;
            }

            _starfield.UpdateStarfield(deltaTime);
        }

        public bool EnableMenuItem(MenuItemType menuItem, bool enable, bool select = true)
        {
            MenuItem item = _menuItems.FirstOrDefault(c => c.Item == menuItem);

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

            if (select)
            {
                _selectedMenuItem = item;
            }
            else
            {
                _selectedMenuItem = _menuItems.Where(c => c.Enable).OrderBy(c => c.Position).FirstOrDefault();
            }

            return true;
        }

        public void KeyPressed(RenderWindow target, object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Down || e.Code == Keyboard.Key.S ||
                e.Code == Keyboard.Key.Up || e.Code == Keyboard.Key.W || e.Code == Keyboard.Key.Escape)
            {
                MenuItem nextSelectedMenuItem = _selectedMenuItem;

                if (e.Code == Keyboard.Key.Down || e.Code == Keyboard.Key.S)
                {
                    nextSelectedMenuItem = GetMenuItems().FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position > _selectedMenuItem.Position);
                }
                else if (e.Code == Keyboard.Key.Up || e.Code == Keyboard.Key.W)
                {
                    nextSelectedMenuItem = GetMenuItems().OrderByDescending(c => c.Position).FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position < _selectedMenuItem.Position);
                }
                else if (e.Code == Keyboard.Key.Escape && _selectedMenuItem.Parent != MenuItemType.None)
                {
                    nextSelectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }
            }

            if (e.Code == Keyboard.Key.Return)
            {
                if (_selectedMenuItem.Item == MenuItemType.Back && _selectedMenuItem.Parent != MenuItemType.None)
                {
                    _selectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }
                else if (_selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo)
                {
                    _selectedMenuItem.FunctionObject?.Invoke(!(bool)_selectedMenuItem.FunctionObject(null));
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

            // Press A button
            if (arg.Button == 0)
            {
                if (_selectedMenuItem.Item == MenuItemType.Back && _selectedMenuItem.Parent != MenuItemType.None)
                {
                    _selectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }
                else if (_selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo)
                {
                    _selectedMenuItem.FunctionObject?.Invoke(!(bool)_selectedMenuItem.FunctionObject(null));
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
            }
            // Press B button
            else if (arg.Button == 1)
            {
                MenuItem nextSelectedMenuItem = _selectedMenuItem;

                if (_selectedMenuItem.Parent != MenuItemType.None)
                {
                    nextSelectedMenuItem = _menuItems.SingleOrDefault(c => c.Item == _selectedMenuItem.Parent);
                }

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
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

            if (arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position + 100) < GamepadMinimumInputThreshold)
            {
                MenuItem nextSelectedMenuItem = _selectedMenuItem;

                // Move Down
                nextSelectedMenuItem =
                    nextSelectedMenuItem = GetMenuItems().FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position > _selectedMenuItem.Position);

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }
            }
            else if (arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position - 100) < GamepadMinimumInputThreshold)
            {
                MenuItem nextSelectedMenuItem = _selectedMenuItem;

                // Move Up
                nextSelectedMenuItem =
                    GetMenuItems().OrderByDescending(c => c.Position).FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position < _selectedMenuItem.Position);

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }
            }
        }

        private static Music GetMusic()
        {
            return AssetManager.Instance.Music.Get("music01");
        }

        private void RecalculateMenuItemsPosition(MenuItem[] items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            var index = 0;
            var enableItems = items.OrderBy(c => c.Position).Where(c => c.Enable);

            var menuItems = enableItems as IList<MenuItem> ?? enableItems.ToList();
            if (menuItems.Any(c => c.FunctionType == MenuItemFunctionType.CustomPage))
            {
                foreach (MenuItem menuItem in menuItems)
                {
                    menuItem.Y = MenuFirstItemPositionY + MenuNextItemOffsetPositionY * 4 * index++;
                }
            }
            else
            {
                foreach (MenuItem menuItem in menuItems)
                {
                    menuItem.Y = MenuFirstItemPositionY + MenuNextItemOffsetPositionY * index++;
                    RecalculateMenuItemsPosition(menuItem.SubMenuItems);
                }
            }
        }

        private static Func<object, object> ScoresCustomSection()
        {
            return (x) =>
            {
                if (!(x is RenderTarget target))
                {
                    return null;
                }

                Point2 scorePosition = new Point2(100 + _scoreOffset.X, 180 + _scoreOffset.Y);

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

                int offset = 1;
                foreach (ScoreLine scoreLine in GameSettings.Scores.OrderByDescending(c => c.Score))
                {
                    DrawText(target, ItemFont, offset.ToString(), scorePosition.X + 40, scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);
                    DrawText(target, ItemFont, scoreLine.Name, scorePosition.X + 100, scorePosition.Y + offset * 40, ScoresColor, 20);
                    DrawText(target, ItemFont, scoreLine.Score.ToString(), scorePosition.X + 483, scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);
                    DrawText(target, ItemFont, scoreLine.Lines.ToString(), scorePosition.X + 602, scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);
                    DrawText(target, ItemFont, scoreLine.Level.ToString(), scorePosition.X + 700, scorePosition.Y + 13 + offset * 40, ScoresColor, 20, true, true);

                    offset++;
                }

                return null;
            };
        }

        private static Func<object, object> SwitchYesNoMenuItem(MenuItemType menuItemType)
        {
            switch (menuItemType)
            {
                case MenuItemType.Sound:
                    return (x) =>
                    {
                        if (x != null)
                        {
                            GameSettings.IsSound = (bool)x;
                            GameSettings.Save();
                        }

                        return GameSettings.IsSound;
                    };
                case MenuItemType.Music:
                    return (x) =>
                    {
                        if (x != null)
                        {
                            GameSettings.IsMusic = (bool)x;
                            if (GameSettings.IsMusic)
                            {
                                GetMusic().Play();
                            }
                            else
                            {
                                GetMusic().Stop();
                            }
                            GameSettings.Save();

                        }
                        return GameSettings.IsMusic;
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
