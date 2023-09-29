namespace Spacetris.GameStates.Menu;

public class MenuItem
{
    public string Name { get; }
    public int Y { get; set; }
    public int Position { get; }
    public MenuItemType Item { get; }
    public bool Enable { get; set; }
    public MenuItem[] SubMenuItems { get; set; }
    public MenuItemType Parent { get; }
    public MenuItemFunctionType FunctionType { get; }
    public readonly Func<object, object, object> FunctionObject;

    public MenuItem(string name, MenuItemType item, int y, int position, bool enable = true,
        MenuItemType parent = MenuItemType.None, MenuItemFunctionType functionType = MenuItemFunctionType.None, Func<object, object, object> functionObject = null)
    {
        Name = name;
        Item = item;
        Y = y;
        Position = position;
        Enable = enable;
        Parent = parent;
        FunctionType = functionType;
        FunctionObject = functionObject;
    }

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