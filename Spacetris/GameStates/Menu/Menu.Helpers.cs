using SFML.Audio;
using Spacetris.Managers;
using Spacetris.Settings;

namespace Spacetris.GameStates.Menu;

public partial class Menu
{
    private void AdjustVolume(int step)
        => _selectedMenuItem.FunctionObject?.Invoke((bool)_selectedMenuItem.FunctionObject(null, null), step);

    private static Music GetMusic()
        => AssetManager.Instance.Music.Get(AssetManagerItemName.Music01);

    private void RecalculateMenuItemsPosition(MenuItem[] items)
    {
        if (items == null || items.Length == 0)
        {
            return;
        }

        var index = 0;
        var enableItems = items.OrderBy(c => c.Position).Where(c => c.Enable);

        var menuItems = enableItems as IList<MenuItem> ?? enableItems.ToList();
        if (menuItems.Any(c => c.FunctionType == MenuItemFunctionType.CustomPage))
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Y = MenuFirstItemPositionY + MenuNextItemOffsetPositionY * 4 * index++;
            }
        }
        else
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Y = MenuFirstItemPositionY + MenuNextItemOffsetPositionY * index++;
                RecalculateMenuItemsPosition(menuItem.SubMenuItems);
            }
        }
    }

    private static bool IsNewValue(object arg1, object arg2, byte value, int minValue, int maxValue, out byte newValue)
    {
        newValue = 0;

        if ((bool)arg1)
        {
            var tmpValue = value + (int)arg2;
            if (tmpValue < minValue)
            {
                tmpValue = minValue;
            }
            else if (tmpValue > maxValue)
            {
                tmpValue = maxValue;
            }

            if (tmpValue != value)
            {
                newValue = Convert.ToByte(tmpValue);
                return true;
            }

            return false;
        }

        return false;
    }

    private static void GameSettingsChanged(object sender, SettingsPropertyType settingsPropertyType)
    {
        switch (settingsPropertyType)
        {
            case SettingsPropertyType.IsMusic:
                break;
            case SettingsPropertyType.IsSound:
                break;
            case SettingsPropertyType.MusicVolume:
                GetMusic().Volume = GameSettings.MusicVolume;
                break;
            case SettingsPropertyType.SoundVolume:
                break;
            case SettingsPropertyType.FontsPath:
                break;
            case SettingsPropertyType.TilesetsPath:
                break;
            case SettingsPropertyType.BackgroundPath:
                break;
            case SettingsPropertyType.SoundsPath:
                break;
            case SettingsPropertyType.MusicPath:
                break;
            case SettingsPropertyType.ImagesPath:
                break;
            case SettingsPropertyType.Scores:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(settingsPropertyType));
        }
    }
}
