﻿using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.GameStates;
using Spacetris.GameStates.Menu;
using Spacetris.GameStates.Worlds;
using Spacetris.Settings;

namespace Spacetris
{
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
        }

        protected override void Initialize()
        {
            _gameState = SpacetrisGameState.Menu;

            _world = new World01();
            _world.WorldStateChanged += PlayFieldStateChanged;
            _world.Initialize(Window);

            _menu = new Menu();
            _menu.MenuItemSelected += MenuItemSelected;
            _menu.Initialize(Window);

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
                    _menu.DrawBackground(Window);
                    if (_world.WorldState == WorldState.Pause)
                    {
                        _world.DrawWorld(Window, false, 15);
                    }
                    _menu.DrawMenu(Window);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void KeyPressed(object sender, KeyEventArgs e)
        {
            switch (_gameState)
            {
                case SpacetrisGameState.Game:
                    _world.KeyPressed(Window, sender, e);
                    break;
                case SpacetrisGameState.Menu:
                    _menu.KeyPressed(Window, sender, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void KeyReleased(object sender, KeyEventArgs e) => _world.KeyReleased(Window, sender, e);

        protected override void JoystickConnected(object sender, JoystickConnectEventArgs arg) => _world.JoystickConnected(sender, arg);

        protected override void JoystickDisconnected(object sender, JoystickConnectEventArgs arg) => _world.JoystickDisconnected(sender, arg);

        protected override void JoystickButtonPressed(object sender, JoystickButtonEventArgs arg) => _world.JoystickButtonPressed(Window, sender, arg);

        protected override void JoystickButtonReleased(object sender, JoystickButtonEventArgs arg) => _world.JoystickButtonReleased(Window, sender, arg);

        protected override void JoystickMoved(object sender, JoystickMoveEventArgs arg) => _world.JoystickMoved(Window, sender, arg);

        protected override void Quit()
        {
#if DEBUG
            Console.WriteLine("Quit Game :(");
#endif
        }

        protected override void Resize(uint width, uint height)
        {

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
                    Console.WriteLine("Game Over");
                    _menu.EnableMenuItem(MenuItemType.Continue, false, false);
                    _gameState = SpacetrisGameState.Menu;
                    break;
                case WorldState.Pause:
                    Console.WriteLine("Pause");
                    _menu.EnableMenuItem(MenuItemType.Continue, true);
                    _gameState = SpacetrisGameState.Menu;
                    break;
            }
        }
    }
}