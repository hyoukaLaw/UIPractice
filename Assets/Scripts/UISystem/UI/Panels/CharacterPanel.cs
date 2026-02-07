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
            Log.LogInfo($"CharacterPanel OnEnter");
            LoadCharacterConfig();
            var characterListItems = BuildCharacterListItems();
            InitializeCharacterView(characterListItems);
            SelectCharacter(0);
            RegisterCallback();
            MarkAllCharacterRedDotDirty();
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
            BindListItemClickEvents();
            BindCharacterRedDotCallbacks();
            BindPanelButtonCallbacks();
        }
        
        public void UnregisterCallback()
        {
            UnbindPanelButtonCallbacks();
            UnbindListItemClickEvents();
            UnbindCharacterRedDotCallbacks();
        }

        private void LoadCharacterConfig()
        {
            var characterConfig = Resources.Load<CharacterConfig>($"Config/CharacterConfig");
            _model.SetCharacterConfig(characterConfig);
        }

        private List<GameObject> BuildCharacterListItems()
        {
            var characterConfig = _model.GetCharacterConfig();
            var characterListItems = new List<GameObject>();
            var gameObjectItemPrefab = Resources.Load<GameObject>("Prefabs/UI/CharacterListItem");
            var index = 0;
            foreach (var item in characterConfig.GetCharacters())
            {
                var gameObjectItem = Object.Instantiate(gameObjectItemPrefab);
                var characterListItem = gameObjectItem.GetComponent<ICharacterListItem>();
                characterListItem.SetData(item, index++);
                _characterListItemUIs.Add(characterListItem);
                characterListItems.Add(gameObjectItem);
            }
            return characterListItems;
        }

        private void InitializeCharacterView(List<GameObject> characterListItems)
        {
            _characterView.SetCharacterList(characterListItems);
        }

        private void BindListItemClickEvents()
        {
            foreach (var item in _characterListItemUIs)
            {
                item.OnClickEvent += SelectCharacter;
            }
        }

        private void UnbindListItemClickEvents()
        {
            foreach (var item in _characterListItemUIs)
            {
                item.OnClickEvent -= SelectCharacter;
            }
        }

        private void BindCharacterRedDotCallbacks()
        {
            foreach (var item in _model.GetCharacterConfig().GetCharacters())
            {
                string redDotName = string.Format(RedDotNames.CHARACTER_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.BindRedDotNameAndReplayCurrent(redDotName, RefreshCharacterRedDot);
                string redDotNameStory = string.Format(RedDotNames.CHARACTER_STORY_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.BindRedDotNameAndReplayCurrent(redDotNameStory, RefreshCharacterStoryRedDot);
                string redDotNameCg = string.Format(RedDotNames.CHARACTER_CG_ID_TEMPLATE, item.GetId());
                RedDotManager.Singleton.BindRedDotNameAndReplayCurrent(redDotNameCg, RefreshCharacterCgRedDot);
            }
        }

        private void UnbindCharacterRedDotCallbacks()
        {
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

        private void BindPanelButtonCallbacks()
        {
            _characterView.OnCloseClick += CloseCurrent;
            _characterView.OnStoryPanelClick += OpenStory;
            _characterView.OnCgPanelClick += OpenConfirm;
        }

        private void UnbindPanelButtonCallbacks()
        {
            _characterView.OnCloseClick -= CloseCurrent;
            _characterView.OnStoryPanelClick -= OpenStory;
            _characterView.OnCgPanelClick -= OpenConfirm;
        }

        private void MarkAllCharacterRedDotDirty()
        {
            foreach (var item in _model.GetCharacterConfig().GetCharacters())
            {
                RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_STORY_NEW, item.GetId());
                RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_CG_NEW, item.GetId());
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
            RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_STORY_NEW, character.GetId());
            RedDotManager.Singleton.MarkRedDotUnitDirty(RedDotUnit.CHARACTER_CG_NEW, character.GetId());
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
