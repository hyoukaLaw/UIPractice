using System;
using UnityEngine;

namespace UIModule.Data.Models
{
    [Serializable]
    public class SerializableCharacterData
    {
        [SerializeField]
        private string _id;
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _title;
        [SerializeField]
        private int _level;
        [SerializeField]
        private Sprite _avatarSprite;
        [SerializeField]
        private Sprite _portraitSprite;
        [SerializeField] 
        private bool _hasNewStory;
        [SerializeField] 
        private bool _hasNewCg;

        public string GetId()
        {
            return _id;
        }

        public void SetId(string id)
        {
            _id = id;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public string GetTitle()
        {
            return _title;
        }

        public void SetTitle(string title)
        {
            _title = title;
        }

        public int GetLevel()
        {
            return _level;
        }

        public void SetLevel(int level)
        {
            _level = level;
        }

        public Sprite GetAvatarSprite()
        {
            return _avatarSprite;
        }

        public void SetAvatarSprite(Sprite sprite)
        {
            _avatarSprite = sprite;
        }

        public Sprite GetPortraitSprite()
        {
            return _portraitSprite;
        }

        public void SetPortraitSprite(Sprite sprite)
        {
            _portraitSprite = sprite;
        }
        
        public bool GetHasNewStory()
        {
            return _hasNewStory;
        }
        
        public void SetHasNewStory(bool isNewCharacter)
        {
            _hasNewStory = isNewCharacter;
        }
        
        public bool GetHasNewCg()
        {
            return _hasNewCg;
        }
    }
}
