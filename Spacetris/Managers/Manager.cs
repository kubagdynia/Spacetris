namespace Spacetris.Managers;

public class Manager<T> : IManager<T> where T : class
{
    private static List<ManagerItem<T>> _items;

        private static List<ManagerItem<T>> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<ManagerItem<T>>();
                }
                return _items;
            }
        }

        public T Load(string name, string filename) => Load(name, filename, false);

        public T Load(AssetManagerItemName name, string filename) => Load(name.ToString(), filename);

        public T Load(string name, string filename, bool overrideItem) => Load(name, filename, overrideItem, null);

        public T Load(AssetManagerItemName name, string filename, bool overrideItem) => Load(name.ToString(), filename, overrideItem);

        public T Load(string name, string filename, object parent) => Load(name, filename, false, parent);

        public T Load(AssetManagerItemName name, string filename, object parent) => Load(name.ToString(), filename, parent);

        public T Load(string name, string filename, bool overrideItem, object parent)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (Exists(name, parent))
            {
                if (overrideItem)
                {
                    Remove(name, parent);
                }
                else
                {
                    return Get(name, parent);
                }
            }

            // Create instance of T class and send filename to its contructor
            var instance = Activator.CreateInstance(typeof(T), filename) as T;

            if (parent == null)
            {
                Items.Add(new ManagerItem<T>(name, instance));
                return instance;
            }

            Items.Add(new ManagerItem<T>(name, parent, instance));

            return instance;
        }

        public T Load(AssetManagerItemName name, string filename, bool overrideItem, object parent) => Load(name.ToString(), filename, overrideItem, parent);

        public void Remove(string name) => Remove(name, null);

        public void Remove(AssetManagerItemName name) => Remove(name.ToString());

        public void Remove(string name, object parent)
        {
            if (parent == null)
            {
                Items.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                return;
            }

            Items.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Parent.Equals(parent));
        }

        public void Remove(AssetManagerItemName name, object parent) => Remove(name.ToString(), parent);

        public void RemoveAll() => Items.Clear();

        public void RemoveParent(object parent)
        {
            Items.RemoveAll(c => c.Parent.Equals(parent));
        }

        public bool Exists(string name) => Exists(name, null);

        public bool Exists(AssetManagerItemName name) => Exists(name.ToString());

        public bool Exists(string name, object parent)
        {
            if (parent == null)
            {
                return !string.IsNullOrWhiteSpace(name) && Items.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            return !string.IsNullOrWhiteSpace(name) && Items.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Parent.Equals(parent));
        }

        public bool Exists(AssetManagerItemName name, object parent) => Exists(name.ToString(), parent);

        public T Get(string name) => Get(name, null);

        public T Get(AssetManagerItemName name) => Get(name.ToString());

        public T Get(string name, object parent)
        {
            if (!Exists(name, parent))
            {
                return default(T);
            }

            if (parent == null)
            {
                return Items.SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))?.Resource;
            }

            return Items.SingleOrDefault(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Parent.Equals(parent))?.Resource;
        }

        public T Get(AssetManagerItemName name, object parent) => Get(name.ToString(), parent);

        public Dictionary<object, List<T>> GetGroupedByParent() => GetGroupedByParent(null);

        public Dictionary<object, List<T>> GetGroupedByParent(string name)
        {
            var grouped = new Dictionary<object, List<T>>();

            foreach (var item in Items.Where(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || name == null))
            {
                if (!grouped.ContainsKey(item.Parent))
                {
                    grouped.Add(item.Parent, new List<T> { item.Resource });
                    continue;
                }

                var groupedItem = grouped[item.Parent];
                groupedItem.Add(item.Resource);
            }

            return grouped;
        }
}