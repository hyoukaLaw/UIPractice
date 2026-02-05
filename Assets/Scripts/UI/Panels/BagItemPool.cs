using System.Collections.Generic;
using UIModule.Core;
using UIModule.Data.Models;
using UnityEngine;

namespace UIModule.Panels
{
    public class BagItemPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject _itemPrefab;
        [SerializeField]
        private Transform _parent;
        private BagConfig _bagConfig;
        private Stack<MonoBagListItem> _pool = new Stack<MonoBagListItem>();
        private Dictionary<int, MonoBagListItem> _activeItems = new Dictionary<int, MonoBagListItem>();

        public void Initialize(BagConfig bagConfig)
        {
            _bagConfig = bagConfig;
        }

        public void InitializePool(int poolSize)
        {
            ClearPool();
            Log.LogInfo($"InitializePool poolSize:{poolSize}");
            for (int i = 0; i < poolSize; i++)
            {
                GameObject itemObj = Instantiate(_itemPrefab, _parent);
                MonoBagListItem listItem = itemObj.GetComponent<MonoBagListItem>();
                if (listItem != null)
                {
                    listItem.gameObject.SetActive(false);
                    _pool.Push(listItem);
                }
            }
        }

        public MonoBagListItem GetOrCreate(int cellIndex)
        {
            if (_activeItems.TryGetValue(cellIndex, out var item))
            {
                return item;
            }

            item = GetFromPool();
            if (item != null)
            {
                item.transform.SetParent(_parent, false);
                item.gameObject.SetActive(true);
                _activeItems[cellIndex] = item;
            }

            return item;
        }

        public void UpdateItemData(int cellIndex, int dataIndex)
        {
            if (_activeItems.TryGetValue(cellIndex, out var item))
            {
                if (_bagConfig != null)
                {
                    var items = _bagConfig.GetItems();
                    if (items != null && dataIndex >= 0 && dataIndex < items.Count)
                    {
                        item.SetData(items[dataIndex], dataIndex);
                    }
                }
            }
        }

        public void ReturnToPool(int cellIndex)
        {
            if (_activeItems.TryGetValue(cellIndex, out var item))
            {
                item.gameObject.SetActive(false);
                _pool.Push(item);
                _activeItems.Remove(cellIndex);
            }
        }

        public void ClearActiveItems()
        {
            foreach (var item in _activeItems.Values)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                    _pool.Push(item);
                }
            }
            _activeItems.Clear();
        }

        public void ClearPool()
        {
            ClearActiveItems();
            foreach (var item in _pool)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            _pool.Clear();
        }

        private MonoBagListItem GetFromPool()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }

            GameObject itemObj = Instantiate(_itemPrefab, _parent);
            return itemObj.GetComponent<MonoBagListItem>();
        }

        public int GetActiveItemCount()
        {
            return _activeItems.Count;
        }

        public MonoBagListItem GetActiveItem(int cellIndex)
        {
            _activeItems.TryGetValue(cellIndex, out var item);
            return item;
        }
    }
}
