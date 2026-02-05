using UIModule.Data;

namespace UIModule.Data.Models
{
    public class CharacterPanelModel : BasePanelModel
    {
        private CharacterConfig _characterConfig;
        private int _selectedIndex;

        public CharacterPanelModel()
        {
            PanelType = UIPanelType.Character;
            _selectedIndex = 0;
        }

        public CharacterConfig GetCharacterConfig()
        {
            return _characterConfig;
        }

        public void SetCharacterConfig(CharacterConfig config)
        {
            _characterConfig = config;
        }

        public int GetSelectedIndex()
        {
            return _selectedIndex;
        }

        public void SetSelectedIndex(int index)
        {
            _selectedIndex = index;
        }

        public SerializableCharacterData GetSelectedCharacter()
        {
            if (_characterConfig == null)
            {
                return null;
            }
            var characters = _characterConfig.GetCharacters();
            if (characters == null || characters.Count == 0)
            {
                return null;
            }
            if (_selectedIndex >= 0 && _selectedIndex < characters.Count)
            {
                return characters[_selectedIndex];
            }
            return null;
        }
    }
}