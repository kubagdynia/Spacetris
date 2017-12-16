using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Spacetris
{
    public class SpacetrisGame : Game
    {
        public SpacetrisGame()
            : base(new Vector2u(960, 540), "Spacetris", Color.Black)
        {

        }

        protected override void LoadContent()
        {

        }

        protected override void Initialize()
        {

        }

        protected override void Update()
        {

        }

        protected override void Render()
        {
            // Draw blue rectangle
            RectangleShape rectangle = new RectangleShape(new Vector2f(200, 200))
            {
                FillColor = Color.Blue
            };
            // Set the rectangle in the center of the screen
            rectangle.Position = new Vector2f(
                (Window.Size.X / 2) - rectangle.Size.X / 2,
                (Window.Size.Y / 2) - rectangle.Size.Y / 2);

            Window.Draw(rectangle);
        }

        protected override void KeyPressed(object sender, KeyEventArgs e)
        {

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
            Console.WriteLine("Quit Game :(");
        }

        protected override void Resize(uint width, uint height)
        {

        }
    }
}
