using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;

namespace UIModule.Panels
{
    public class BagListAdapter : MonoBehaviour, IInfiniteListAdapter
    {
        [SerializeField]
        private BagItemPool _itemPool;

        private BagConfig _bagConfig;

        public void SetBagConfig(BagConfig bagConfig)
        {
            _bagConfig = bagConfig;
            _itemPool.Initialize(bagConfig);
        }

        public int GetItemCount()
        {
            var items = _bagConfig != null ? _bagConfig.GetItems() : null;
            return items != null ? items.Count : 0;
        }

        public void EnsurePoolSize(int visibleCount)
        {
            _itemPool.InitializePool(visibleCount);
        }

        public bool TryGetOrCreateCell(int cellIndex, out RectTransform cellRect)
        {
            cellRect = null;
            var item = _itemPool.GetOrCreate(cellIndex);
            if (item == null)
            {
                return false;
            }
            cellRect = item.GetComponent<RectTransform>();
            return cellRect != null;
        }

        public void BindCell(int cellIndex, int dataIndex)
        {
            _itemPool.UpdateItemData(cellIndex, dataIndex);
        }

        public void ReleaseCell(int cellIndex)
        {
            _itemPool.ReturnToPool(cellIndex);
        }
    }
}
