using SFML.Graphics;
using SFML.Window;

namespace Spacetris.GameStates;

public interface IGameInput
{
    void KeyPressed(RenderWindow target, object sender, KeyEventArgs e);

    void KeyReleased(RenderWindow target, object sender, KeyEventArgs e);

    void JoystickConnected(object sender, JoystickConnectEventArgs arg);

    void JoystickDisconnected(object sender, JoystickConnectEventArgs arg);

    void JoystickButtonPressed(RenderWindow target, object sender, JoystickButtonEventArgs arg);

    void JoystickButtonReleased(RenderWindow target, object sender, JoystickButtonEventArgs arg);

    void JoystickMoved(RenderWindow target, object sender, JoystickMoveEventArgs arg);
}