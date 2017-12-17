using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.GameStates.Menu;
using Spacetris.Settings;

namespace Spacetris
{
    public class SpacetrisGame : Game
    {
        private SpacetrisGameState _gameState;

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

            _menu = new Menu();
            _menu.MenuItemSelected += MenuItemSelected;
            _menu.Initialize(Window);
        }

        protected override void Update()
        {
            switch (_gameState)
            {
                case SpacetrisGameState.Game:
                    
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
                    
                    break;
                case SpacetrisGameState.Menu:
                    _menu.DrawBackground(Window);
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

                    break;
                case SpacetrisGameState.Menu:
                    _menu.KeyPressed(Window, sender, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void KeyReleased(object sender, KeyEventArgs e)
        {

        }

        protected override void JoystickButtonPressed(object sender, JoystickButtonEventArgs arg)
        {

        }

        protected override void JoystickButtonReleased(object sender, JoystickButtonEventArgs arg)
        {

        }

        protected override void JoystickConnected(object sender, JoystickConnectEventArgs arg)
        {

        }

        protected override void JoystickDisconnected(object sender, JoystickConnectEventArgs arg)
        {

        }

        protected override void JoystickMoved(object sender, JoystickMoveEventArgs arg)
        {

        }

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
                case MenuItemType.None:
                    break;
                case MenuItemType.NewGane:
                    break;
                case MenuItemType.Continue:
                    break;
                case MenuItemType.Scores:
                    break;
                case MenuItemType.Config:
                    break;
                case MenuItemType.Quit:
                    Window.Close();
                    break;
                case MenuItemType.ScoresDetails:
                    break;
                case MenuItemType.Sound:
                    break;
                case MenuItemType.Music:
                    break;
                case MenuItemType.Back:
                    break;
            }
        }
    }
}
