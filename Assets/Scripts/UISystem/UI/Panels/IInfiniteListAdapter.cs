using UnityEngine;

namespace UIModule.Interfaces
{
    public interface IInfiniteListAdapter
    {
        int GetItemCount();
        void EnsurePoolSize(int visibleCount);
        bool TryGetOrCreateCell(int cellIndex, out RectTransform cellRect);
        void BindCell(int cellIndex, int dataIndex);
        void ReleaseCell(int cellIndex);
    }
}
