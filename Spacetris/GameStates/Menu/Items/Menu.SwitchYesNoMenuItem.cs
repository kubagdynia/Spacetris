using Spacetris.Settings;

namespace Spacetris.GameStates.Menu;

public partial class Menu
{
    private static Func<object, object, object> SwitchYesNoMenuItem(MenuItemType menuItemType)
    {
        return menuItemType switch
        {
            MenuItemType.Sound => HandleSoundMenuItem,
            MenuItemType.Music => HandleMusicMenuItem,
            _ => throw new ArgumentOutOfRangeException(nameof(menuItemType))
        };
    }

    private static object HandleSoundMenuItem(object arg1, object arg2)
    {
        if (arg1 != null && arg2 != null)
        {
            if (IsNewValue(arg1, arg2, GameSettings.SoundVolume, 0, 100, out byte newValue))
            {
                GameSettings.SoundVolume = newValue;
            }
        }
        else if (arg1 != null)
        {
            GameSettings.IsSound = (bool)arg1;
        }

        return GameSettings.IsSound;
    }

    private static object HandleMusicMenuItem(object arg1, object arg2)
    {
        if (arg1 != null && arg2 != null)
        {
            if (IsNewValue(arg1, arg2, GameSettings.MusicVolume, 0, 100, out byte newValue))
            {
                GameSettings.MusicVolume = newValue;
            }
        }
        else if (arg1 != null)
        {
            GameSettings.IsMusic = (bool)arg1;
            if (GameSettings.IsMusic)
            {
                GetMusic().Play();
            }
            else
            {
                GetMusic().Stop();
            }
        }

        return GameSettings.IsMusic;
    }
}