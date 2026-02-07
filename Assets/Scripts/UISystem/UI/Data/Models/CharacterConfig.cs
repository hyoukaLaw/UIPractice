using System.Collections.Generic;
using UnityEngine;

namespace UIModule.Data.Models
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "UI/Character Config")]
    public class CharacterConfig : ScriptableObject
    {
        [SerializeField]
        private List<SerializableCharacterData> _characters;

        public List<SerializableCharacterData> GetCharacters()
        {
            return _characters;
        }

        public void SetCharacters(List<SerializableCharacterData> characters)
        {
            _characters = characters;
        }

        public SerializableCharacterData GetCharacterById(int id)
        {
            if (_characters == null)
            {
                return null;
            }
            foreach (var character in _characters)
            {
                if (character != null && character.GetId() == id)
                {
                    return character;
                }
            }
            return null;
        }

        public int GetCharacterIndexById(int id)
        {
            if (_characters == null)
            {
                return -1;
            }
            for (int i = 0; i < _characters.Count; i++)
            {
                if (_characters[i] != null && _characters[i].GetId() == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
