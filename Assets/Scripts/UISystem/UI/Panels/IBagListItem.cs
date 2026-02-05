using System;
using UIModule.Data.Models;

namespace UIModule.Interfaces
{
    public interface IBagListItem
    {
        void SetData(SerializableBagItem data, int index);
        void SetSelected(bool selected);
        int GetIndex();
        event Action<int> OnClickEvent;
    }
}
