using System.Collections.Generic;
using UnityEngine;

namespace UIModule.Data.Models
{
    [CreateAssetMenu(fileName = "BagConfig", menuName = "UI/Bag Config")]
    public class BagConfig : ScriptableObject
    {
        [SerializeField]
        private List<SerializableBagItem> _items;

        public List<SerializableBagItem> GetItems()
        {
            return _items;
        }

        public void SetItems(List<SerializableBagItem> items)
        {
            _items = items;
        }

        public SerializableBagItem GetItemById(string id)
        {
            if (_items == null)
            {
                return null;
            }
            foreach (var item in _items)
            {
                if (item != null && item.GetId() == id)
                {
                    return item;
                }
            }
            return null;
        }

        public int GetItemIndexById(string id)
        {
            if (_items == null)
            {
                return -1;
            }
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null && _items[i].GetId() == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
