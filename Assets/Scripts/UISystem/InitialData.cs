using System.Collections.Generic;

namespace UIModule.Core.UISystem
{
    public class InitialData:SingletonTemplate<InitialData>
    {
        public static  Dictionary<int, bool> CharacterStoryNew = new ();

        public static bool CharacterNew = true;
    }
}