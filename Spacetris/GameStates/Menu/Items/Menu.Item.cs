namespace Spacetris.GameStates.Menu;

public class MenuItem(
    string name,
    MenuItemType item,
    int y,
    int position,
    bool enable = true,
    MenuItemType parent = MenuItemType.None,
    MenuItemFunctionType functionType = MenuItemFunctionType.None,
    Func<object, object, object> functionObject = null)
{
    public string Name { get; } = name;
    public int Y { get; set; } = y;
    public int Position { get; } = position;
    public MenuItemType Item { get; } = item;
    public bool Enable { get; set; } = enable;
    public MenuItem[] SubMenuItems { get; set; }
    public MenuItemType Parent { get; } = parent;
    public MenuItemFunctionType FunctionType { get; } = functionType;
    public readonly Func<object, object, object> FunctionObject = functionObject;

    public MenuItem(MenuItemType item, int y, int position, bool enable = true, MenuItemType parent = MenuItemType.None)
        : this(item.ToString(), item, y, position, enable, parent)
    {

    }

    public MenuItem(MenuItemType item, int y, int position, MenuItemType parent)
        : this(item.ToString(), item, y, position, true, parent)
    {

    }

    public MenuItem(MenuItemType item, int y, int position, MenuItemType parent, MenuItemFunctionType functionType, Func<object, object, object> functionObject)
        : this(item.ToString(), item, y, position, true, parent, functionType, functionObject)
    {

    }
}