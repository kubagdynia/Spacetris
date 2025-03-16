using SFML.Audio;
using SFML.Graphics;
using Spacetris.BackgroundEffects;
using Spacetris.DataStructures;
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
    private const int MenuItemShadowWidth = 285;
    private const int MenuItemShadowHeight = 65;
    private const int MenuItemShadowOffsetX = 145;
    private const int MenuItemShadowOffsetY = 30;

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

    /// <summary>
    /// Initializes the menu with the specified render window.
    /// </summary>
    /// <param name="target">The render window to initialize the menu with.</param>
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
            result = [.. _menuItems.Where(c => c.Enable)];
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
}