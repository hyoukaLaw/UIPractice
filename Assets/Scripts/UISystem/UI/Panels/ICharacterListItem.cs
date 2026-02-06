using System;
using UIModule.Data.Models;

namespace UIModule.Interfaces
{
    public interface ICharacterListItem
    {
        void SetData(SerializableCharacterData data, int index);
        void SetSelected(bool selected);
        void SetRedDot(bool show);
        event Action<int> OnClickEvent;
        int GetIndex();
    }
}
