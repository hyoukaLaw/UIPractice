using System;
using System.Collections.Generic;
using UIModule.Data.Models;
using UnityEngine;

namespace UIModule.Interfaces
{
    public interface ICharacterView
    {
        void SetCharacterName(string name);
        void SetLevel(int level);
        void SetCharacterList(List<GameObject> characterList);
        void SetStoryRedDot(bool show);
        void SetCgRedDot(bool show);
        event Action OnCloseClick;
        event Action OnStoryPanelClick;
        event Action OnCgPanelClick;
    }
}
