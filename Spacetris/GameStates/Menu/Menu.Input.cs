using SFML.Graphics;
using SFML.Window;
using Spacetris.DataStructures;

namespace Spacetris.GameStates.Menu;

public partial class Menu
{
    public void KeyPressed(RenderWindow target, object sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Down or Keyboard.Key.S or Keyboard.Key.Up or Keyboard.Key.W or Keyboard.Key.Escape:
                HandleNavigationKeys(e.Code);
                break;

            case Keyboard.Key.Left when _selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo:
                AdjustVolume(-YesNoVolumeStep);
                break;

            case Keyboard.Key.Right when _selectedMenuItem.FunctionType == MenuItemFunctionType.YesNo:
                AdjustVolume(YesNoVolumeStep);
                break;

            case Keyboard.Key.Enter:
                HandleEnterKey();
                break;
        }
    }

    public void KeyReleased(RenderWindow target, object sender, KeyEventArgs e)
    {
        
    }

    public void JoystickConnected(object sender, JoystickConnectEventArgs arg)
    {
        
    }

    public void JoystickDisconnected(object sender, JoystickConnectEventArgs arg)
    {
        
    }

    public void JoystickButtonPressed(RenderWindow target, object sender, JoystickButtonEventArgs arg)
    {
        switch (arg.Button)
        {
            case 0: // Press A button
                HandleAButtonPress();
                break;
            case 1: // Press B button
                HandleBButtonPress();
                break;
        }
    }

    public void JoystickButtonReleased(RenderWindow target, object sender, JoystickButtonEventArgs arg)
    {
        
    }

    public void JoystickMoved(RenderWindow target, object sender, JoystickMoveEventArgs arg)
    {
        switch (arg.Axis)
        {
            // Move Down
            case Joystick.Axis.PovY when Math.Abs(arg.Position + 100) < GamepadMinimumInputTolerance:
                var nextSelectedMenuItem = GetMenuItems()
                    .OrderBy(c => c.Position)
                    .FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position > _selectedMenuItem.Position);

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }
                break;

            // Move Up
            case Joystick.Axis.PovY when Math.Abs(arg.Position - 100) < GamepadMinimumInputTolerance:
                nextSelectedMenuItem = GetMenuItems()
                    .OrderByDescending(c => c.Position)
                    .FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage && c.Position < _selectedMenuItem.Position);

                if (nextSelectedMenuItem != null)
                {
                    PlaySound(_menuSoundBeep);
                    _selectedMenuItem = nextSelectedMenuItem;
                }
                break;

            // Move Left
            default:
                switch (_selectedMenuItem.FunctionType)
                {
                    case MenuItemFunctionType.YesNo when arg.Axis == Joystick.Axis.PovX && Math.Abs(arg.Position + 100) < GamepadMinimumInputTolerance:
                        _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), -YesNoVolumeStep);
                        break;

                    // Move Right
                    case MenuItemFunctionType.YesNo when arg.Axis == Joystick.Axis.PovX && Math.Abs(arg.Position - 100) < GamepadMinimumInputTolerance:
                        _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), YesNoVolumeStep);
                        break;
                }
                break;
        }
    }

    private void HandleNavigationKeys(Keyboard.Key key)
    {
        var nextSelectedMenuItem = key switch
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
    }

    private void HandleEnterKey()
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
            _scoreOffset = Point2.Zero;
            _scoreOffsetStep = 1;
            _selectedMenuItem = _selectedMenuItem.SubMenuItems
                .OrderBy(c => c.Position)
                .FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage);
        }

        if (_selectedMenuItem != null)
        {
            PlaySound(_menuSoundSelect);
            MenuItemSelected?.Invoke(this, _selectedMenuItem.Item);
        }
    }

    private void HandleAButtonPress()
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
            _scoreOffset = Point2.Zero;
            _scoreOffsetStep = 1;
            _selectedMenuItem = _selectedMenuItem.SubMenuItems
                .OrderBy(c => c.Position)
                .FirstOrDefault(c => c.Enable && c.FunctionType != MenuItemFunctionType.CustomPage);
        }

        if (_selectedMenuItem != null)
        {
            PlaySound(_menuSoundSelect);
            MenuItemSelected?.Invoke(this, _selectedMenuItem.Item);
        }
    }

    private void HandleBButtonPress()
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
    }
}
