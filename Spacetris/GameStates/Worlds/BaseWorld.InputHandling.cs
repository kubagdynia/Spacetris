using SFML.Graphics;
using SFML.Window;
using Spacetris.DataStructures;
using Spacetris.Extensions;
using Spacetris.Settings;

namespace Spacetris.GameStates.Worlds;

public abstract partial class BaseWorld 
{
    public virtual void KeyPressed(RenderWindow target, object sender, KeyEventArgs e)
    {
        if (e.Code != Keyboard.Key.Down)
        {
            _isKeyDownEnabled = true;
        }

        if (WorldState == WorldState.GameOver)
        {
            HandleGameOverKeyPress(e);
        }
        else if (e.Code == Keyboard.Key.Escape || (e.Code == Keyboard.Key.P && WorldState != WorldState.GameOver))
        {
            PauseGame();
        }
        else if (IsMovementKey(e.Code))
        {
            if (WorldState is WorldState.NewGame or WorldState.Continue)
            {
                WorldState = WorldState.Playing;
            }

            HandleMovementKeyPress(target, e.Code);
        }
    }

    private void HandleGameOverKeyPress(KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
        {
            WorldState = WorldState.Quit;
        }
        else if (GameSettings.IsEnoughPointsForTopScores(GameState.Score))
        {
            HandleHighScoreKeyPress(e);
        }
    }

    private void HandleHighScoreKeyPress(KeyEventArgs e)
    {
        if (AllowedKeyboardChars.ContainsKey(e.Code) && _userName.Length < 20)
        {
            _userName += AllowedKeyboardChars[e.Code];
        }
        else
        {
            switch (e.Code)
            {
                case Keyboard.Key.Backspace when _userName.Length > 0:
                    _userName = _userName.Remove(_userName.Length - 1);
                    break;
                case Keyboard.Key.Enter:
                    GameSettings.AddScore(new ScoreLine(_userName, GameState.Lines, GameState.Level, GameState.Score));
                    WorldState = WorldState.Quit;
                    break;
            }
        }
    }

    private void PauseGame()
    {
        _timer?.Dispose();
        WorldState = WorldState.Pause;
    }

    private bool IsMovementKey(Keyboard.Key key)
    {
        return (_readyForRotate && key is Keyboard.Key.Up or Keyboard.Key.W) ||
               key == Keyboard.Key.Left || key == Keyboard.Key.A ||
               key == Keyboard.Key.Right || key == Keyboard.Key.D ||
               key == Keyboard.Key.Space || (key is Keyboard.Key.Down or Keyboard.Key.S && _isKeyDownEnabled);
    }

    private void HandleMovementKeyPress(RenderWindow target, Keyboard.Key key)
    {
        if (_readyForRotate && key is Keyboard.Key.Up or Keyboard.Key.W)
        {
            _readyForRotate = false;
            if (RotateTetromino())
            {
                PlaySound(GameSoundMoveTetromino);
            }
        }
        else
        {
            switch (key)
            {
                case Keyboard.Key.Left or Keyboard.Key.A:
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, -1);
                    break;
                case Keyboard.Key.Right or Keyboard.Key.D:
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 1);
                    break;
                case Keyboard.Key.Down or Keyboard.Key.S when _isKeyDownEnabled:
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 0, 1);
                    break;
                case Keyboard.Key.Space:
                    FindTetrominoLandingPosition(out Point2[] landingBlocksPosition);
                    MoveTetromino(target, 0, landingBlocksPosition[0].Y - GameState.CurrentTetrominoBlocksPosition[0].Y);
                    MoveTetromino(target, 0, 1);
                    break;
            }
        }
    }

    public virtual void KeyReleased(RenderWindow target, object sender, KeyEventArgs e)
    {
        if (WorldState != WorldState.Playing)
        {
            return;
        }

        switch (e.Code)
        {
            case Keyboard.Key.Up or Keyboard.Key.W:
                _readyForRotate = true;
                break;
            case Keyboard.Key.Down or Keyboard.Key.S when !_isKeyDownEnabled:
                _isKeyDownEnabled = true;
                break;
        }
    }

    public virtual void JoystickConnected(object sender, JoystickConnectEventArgs arg)
    {
#if DEBUG
        $"Controller connected: {arg.JoystickId}".Log();
#endif
    }

    public virtual void JoystickDisconnected(object sender, JoystickConnectEventArgs arg)
    {
#if DEBUG
        $"Controller disconnected: {arg.JoystickId}".Log();
#endif
    }

    public virtual void JoystickButtonPressed(RenderWindow target, object sender, JoystickButtonEventArgs arg)
    {
#if DEBUG
        $"Controller ({arg.JoystickId}) Button Pressed: {arg.Button})".Log();
#endif
        if (WorldState is WorldState.NewGame or WorldState.Continue)
        {
            WorldState = WorldState.Playing;
        }

        switch (arg.Button)
        {
            // Press Bumper button
            case 4 or 5:
                FindTetrominoLandingPosition(out Point2[] landingBlocksPosition);
                MoveTetromino(target, 0, landingBlocksPosition[0].Y - GameState.CurrentTetrominoBlocksPosition[0].Y);
                MoveTetromino(target, 0, 1);
                break;
            // Press Menu or B button
            case 7 or 1:
                _timer?.Dispose();
                WorldState = WorldState.Pause;
                break;
        }
    }

    public virtual void JoystickButtonReleased(RenderWindow target, object sender, JoystickButtonEventArgs arg)
    {
#if DEBUG
        $"Controller ({arg.JoystickId}) Button Released: {arg.Button})".Log();
#endif
    }

    public virtual void JoystickMoved(RenderWindow target, object sender, JoystickMoveEventArgs arg)
    {
#if DEBUG
        $"Controller ({arg.JoystickId}) Moved: Axis({arg.Axis}), Position({arg.Position})".Log();
#endif
        if (arg.Axis is Joystick.Axis.PovX or Joystick.Axis.PovY)
        {
            if (WorldState is WorldState.NewGame or WorldState.Continue)
            {
                WorldState = WorldState.Playing;
            }

            switch (arg.Axis)
            {
                case Joystick.Axis.PovX when Math.Abs(arg.Position + 100) < GamepadMinimumInputTolerance:
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, -1);
                    break;
                case Joystick.Axis.PovX when Math.Abs(arg.Position - 100) < GamepadMinimumInputTolerance:
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 1);
                    break;
                case Joystick.Axis.PovY when Math.Abs(arg.Position + 100) < GamepadMinimumInputTolerance:
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 0, 1);
                    break;
                default:
                {
                    if (_readyForRotate && arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position - 100) < GamepadMinimumInputTolerance)
                    {
                        _readyForRotate = false;
                        if (RotateTetromino())
                        {
                            PlaySound(GameSoundMoveTetromino);
                        }
                    }
                    else if (arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position) < GamepadMinimumInputTolerance)
                    {
                        _readyForRotate = true;
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// Find the position of tetromino landing
    /// </summary>
    private void FindTetrominoLandingPosition(out Point2[] landingBlocksPosition)
    {
        landingBlocksPosition = new Point2[GameState.CurrentTetrominoBlocksPosition.Length];

        GameState.CurrentTetrominoBlocksPosition.CopyTo(landingBlocksPosition, 0);

        while (CheckWorldCollider(landingBlocksPosition))
        {
            for (var i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                landingBlocksPosition[i].Y++;
            }
        }

        // Put tetromino over the landing position
        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
        {
            landingBlocksPosition[i].Y--;
        }
    }
}