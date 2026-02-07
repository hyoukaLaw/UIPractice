using System.Collections.Generic;
using UIModule.Data.Models;
using UnityEngine;

namespace UIModule.Core.UISystem
{
    public class InitialData:SingletonTemplate<InitialData>
    {
        public Dictionary<int, bool> CharacterStoryNew = new ();

        public bool CharacterNew = true;

        public CharacterConfig CharacterConfig;

        public InitialData()
        {
            CharacterConfig = Resources.Load<CharacterConfig>("Config/CharacterConfig");
        }
    }
}