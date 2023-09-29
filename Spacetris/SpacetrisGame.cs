using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.Extensions;
using Spacetris.GameStates;
using Spacetris.GameStates.Menu;
using Spacetris.GameStates.Worlds;
using Spacetris.Managers;
using Spacetris.Settings;

namespace Spacetris;

public class SpacetrisGame : Game
{
    private SpacetrisGameState _gameState;
    
    private float _totalTickTimer;

    private IWorld _world;

    private IMenu _menu;
    
    public SpacetrisGame()
        : base(new Vector2u(960, 540), "Spacetris", Color.Black)
    {

    }
    
    protected override void LoadContent()
        {
            GameSettings.Load();

            // Load music
            AssetManager.Instance.Music.Load(AssetManagerItemName.Music01, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameSettings.MusicPath, "music.ogg"));

            // Load textures
            AssetManager.Instance.Texture.Load(AssetManagerItemName.GamepadTexture, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameSettings.ImagesPath, "gamepad.png"));
            AssetManager.Instance.Texture.Load(AssetManagerItemName.ControlsTexture, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameSettings.ImagesPath, "controls.png"));

            // Load fonts
            AssetManager.Instance.Font.Load(AssetManagerItemName.TetrisFont, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameSettings.FontsPath, "Tetris.ttf"));
            AssetManager.Instance.Font.Load(AssetManagerItemName.SlkscrFont, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameSettings.FontsPath, "slkscr.ttf"));
            AssetManager.Instance.Font.Load(AssetManagerItemName.ArialFont, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GameSettings.FontsPath, "arial.ttf"));
        }

        protected override void Initialize()
        {
            _gameState = SpacetrisGameState.Menu;

            _menu = new Menu();
            _menu.MenuItemSelected += MenuItemSelected;
            _menu.Initialize(Window);

            _world = new World01();
            _world.WorldStateChanged += PlayFieldStateChanged;
            _world.Initialize(Window);

            _world.CreateNewTetromino();
        }

        protected override void Update()
        {
            switch (_gameState)
            {
                case SpacetrisGameState.Game:
                    _totalTickTimer += DeltaTime;

                    _world.Update(Window, DeltaTime);

                    // Tick
                    if (_totalTickTimer > GameState.TotalFallTickDelay)
                    {
                        _world.MoveTetromino(Window, 0, 1);

                        _totalTickTimer = 0;
                    }
                    break;
                case SpacetrisGameState.Menu:
                    _menu.Update(Window, DeltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Render()
        {
            switch (_gameState)
            {
                case SpacetrisGameState.Game:
                    _world.DrawAllLayers(Window);
                    break;
                case SpacetrisGameState.Menu:
                    _menu.DrawAllLayers(Window, _world);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void KeyPressed(object sender, KeyEventArgs e) => GetGameInput().KeyPressed(Window, sender, e);

        protected override void KeyReleased(object sender, KeyEventArgs e) => GetGameInput().KeyReleased(Window, sender, e);

        protected override void JoystickConnected(object sender, JoystickConnectEventArgs arg) => GetGameInput().JoystickConnected(sender, arg);

        protected override void JoystickDisconnected(object sender, JoystickConnectEventArgs arg) => GetGameInput().JoystickDisconnected(sender, arg);

        protected override void JoystickButtonPressed(object sender, JoystickButtonEventArgs arg) => GetGameInput().JoystickButtonPressed(Window, sender, arg);

        protected override void JoystickButtonReleased(object sender, JoystickButtonEventArgs arg) => GetGameInput().JoystickButtonReleased(Window, sender, arg);

        protected override void JoystickMoved(object sender, JoystickMoveEventArgs arg) => GetGameInput().JoystickMoved(Window, sender, arg);

        protected override void Quit()
        {
            GameSettings.CleanUp();
#if DEBUG
            "Quit Game :(".Log();
#endif
        }

        protected override void Resize(uint width, uint height)
        {

        }

        private IGameInput GetGameInput()
        {
            switch (_gameState)
            {
                case SpacetrisGameState.Game:
                    return _world;
                case SpacetrisGameState.Menu:
                    return _menu;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MenuItemSelected(object sender, MenuItemType e)
        {
            switch (e)
            {
                case MenuItemType.NewGane:
                    _world.WorldState = WorldState.NewGame;
                    _gameState = SpacetrisGameState.Game;
                    break;
                case MenuItemType.Continue:
                    _world.WorldState = WorldState.Continue;
                    _gameState = SpacetrisGameState.Game;
                    break;
                case MenuItemType.Scores:
                    break;
                case MenuItemType.Quit:
                    Window.Close();
                    break;
                case MenuItemType.Sound:
                    break;
                case MenuItemType.Music:
                    break;
            }
        }

        private void PlayFieldStateChanged(object sender, WorldState e)
        {
            switch (e)
            {
                case WorldState.Quit:
#if DEBUG
                    "Game Over".Log();
#endif
                    _menu.EnableMenuItem(MenuItemType.Continue, false, false);
                    _gameState = SpacetrisGameState.Menu;
                    break;
                case WorldState.Pause:
#if DEBUG
                    "Pause".Log();
#endif
                    _menu.EnableMenuItem(MenuItemType.Continue, true);
                    _gameState = SpacetrisGameState.Menu;
                    break;
            }
        }
    
}