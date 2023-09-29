namespace Spacetris.Managers;

public interface IManager<T>
{
    T Load(string name, string filename);

    T Load(string name, string filename, bool overrideItem);

    T Load(string name, string filename, object parent);

    T Load(string name, string filename, bool overrideItem, object parent);

    T Get(string name);

    T Get(string name, object parent);

    Dictionary<object, List<T>> GetGroupedByParent();

    Dictionary<object, List<T>> GetGroupedByParent(string name);

    void Remove(string name);

    void Remove(string name, object parent);

    void RemoveAll();

    void RemoveParent(object parent);

    bool Exists(string name);

    bool Exists(string name, object parent);
}