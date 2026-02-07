using System.Collections.Generic;
using UIModule.Core;
using UIModule.Data;
using UIModule.Data.Models;
using UIModule.Interfaces;
using UnityEngine;

namespace UIModule.Panels
{
    public class CharacterPanel : BaseUIPanel
    {
        private string _characterId;
        private int _level;
        private ICharacterView _characterView;
        private CharacterPanelModel _model;
        private List<ICharacterListItem> _characterListItemUIs = new();

        public CharacterPanel(ICharacterView characterView, BasePanelModel model)
        {
            PanelType = UIPanelType.Character;
            IsModal = false;
            _characterView = characterView;
            _model = (CharacterPanelModel)model;
        }

        public override void OnEnter(params object[] args)
        {
            Log.LogInfo($"CharacterPanel OnEnter: CharacterId={_characterId}, Level={_level}");
            
            CharacterConfig characterConfig = Resources.Load<CharacterConfig>($"Config/CharacterConfig");
            _model.SetCharacterConfig(characterConfig);
            
            int index = 0;
            List<GameObject> characterListItems = new();
            GameObject gameObjectItemPrefab = Resources.Load<GameObject>("Prefabs/UI/CharacterListItem");
            foreach (var item in characterConfig.GetCharacters())
            { 
                GameObject gameObjectItem = Object.Instantiate(gameObjectItemPrefab);
                ICharacterListItem characterListItem = gameObjectItem.GetComponent<ICharacterListItem>();
                characterListItem.OnClickEvent += SelectCharacter;
                characterListItem.SetData(item, index++);
                _characterListItemUIs.Add(characterListItem);
                characterListItems.Add(gameObjectItem);
                string redDotName = string.Format(RedDotNames.CHARACTER_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.BindRedDotName(redDotName, RefreshCharacterRedDot);

                string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.BindRedDotName(redDotNameStory, RefreshCharacterStoryRedDot);
                
                string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.BindRedDotName(redDotNameCg, RefreshCharacterCgRedDot);
                
                RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_STORY_NEW, item.GetId());
                RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_CG_NEW, item.GetId());
            }
            _characterView.SetCharacterList(characterListItems);
            SelectCharacter(0);
            RegisterCallback();
        }

        public override void OnExit()
        {
            Log.LogInfo($"CharacterPanel OnExit");
            UnregisterCallback();
        }

        public override void OnPause()
        {
            Log.LogInfo($"CharacterPanel OnPause");
        }

        public override void OnResume()
        {
            Log.LogInfo($"CharacterPanel OnResume");
        }

        public void RegisterCallback()
        {
            _characterView.OnCloseClick += CloseCurrent;
            _characterView.OnStoryPanelClick += OpenStory;
            _characterView.OnCgPanelClick += OpenConfirm;
        }
        
        public void UnregisterCallback()
        {
            _characterView.OnCloseClick -= CloseCurrent;
            _characterView.OnStoryPanelClick -= OpenStory;
            _characterView.OnCgPanelClick -= OpenConfirm;
            foreach (var item in _characterListItemUIs)
            {
                item.OnClickEvent -= SelectCharacter;

            }

            foreach (var item in _model.GetCharacterConfig().GetCharacters())
            {
                string redDotName = string.Format(RedDotNames.CHARACTER_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.UnbindRedDotName(redDotName, RefreshCharacterRedDot);
                
                string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.UnbindRedDotName(redDotNameStory, RefreshCharacterStoryRedDot);

                string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.UnbindRedDotName(redDotNameCg, RefreshCharacterCgRedDot);
            }
        }

        private void CloseCurrent()
        {
            UIManager.Instance.HidePanel(UIPanelType.Character);
        }
        
        private void OpenStory()
        {
            UIManager.Instance.ShowPanel(UIPanelType.CharacterStory);
        }

        private void OpenConfirm()
        {
            UIManager.Instance.ShowPanel(UIPanelType.ModalConfirm);
        }

        private void SelectCharacter(int index)
        {
            _model.SetSelectedIndex(index);
            foreach (var item in _characterListItemUIs)
            {
                item.SetSelected(index == item.GetIndex());
            }
            var character = _model.GetCharacterConfig().GetCharacters()[index];
            _characterView.SetCharacterName(character.GetName());
            _characterView.SetLevel(character.GetLevel());
        }

        private void RefreshCharacterRedDot(string redDotName, int result, RedDotType redDotType)
        {
            int characterId;
            if (!RedDotManager.Singleton.TryParseRedDotNameWithId(redDotName, out characterId))
            {
                return;
            }
            foreach (var item in _characterListItemUIs)
            {
                if (item.GetCharacterId() == characterId)
                {
                    item.SetRedDot(result > 0);
                }
            }
        }

        private void RefreshCharacterStoryRedDot(string redDotName, int result, RedDotType redDotType)
        {
            int characterId;
            if (!RedDotManager.Singleton.TryParseRedDotNameWithId(redDotName, out characterId))
            {
                return;
            }

            var selectedCharacter = _model.GetSelectedCharacter();
            if (selectedCharacter != null && selectedCharacter.GetId() == characterId)
            {
                _characterView.SetStoryRedDot(result > 0);
            }
        }
        
        private void RefreshCharacterCgRedDot(string redDotName, int result, RedDotType redDotType)
        {
            int characterId;
            if (!RedDotManager.Singleton.TryParseRedDotNameWithId(redDotName, out characterId))
            {
                return;
            }

            var selectedCharacter = _model.GetSelectedCharacter();
            if (selectedCharacter != null && selectedCharacter.GetId() == characterId)
            {
                _characterView.SetCgRedDot(result > 0);
            }
        }
        
    }
}
