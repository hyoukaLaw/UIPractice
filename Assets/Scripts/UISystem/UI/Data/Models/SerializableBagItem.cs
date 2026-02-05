using System;
using UnityEngine;

namespace UIModule.Data.Models
{
    [Serializable]
    public class SerializableBagItem
    {
        [SerializeField]
        private string _id;
        [SerializeField]
        private string _name;
        [SerializeField]
        private int _count;
        [SerializeField]
        private Sprite _icon;

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

        public int GetCount()
        {
            return _count;
        }

        public void SetCount(int count)
        {
            _count = count;
        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public void SetIcon(Sprite icon)
        {
            _icon = icon;
        }
    }
}
