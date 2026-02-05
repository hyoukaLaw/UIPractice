using UIModule.Data;

namespace UIModule.Data.Models
{
    public class BagPanelModel : BasePanelModel
    {
        private BagConfig _bagConfig;
        private int _selectedIndex;

        public BagPanelModel()
        {
            PanelType = UIPanelType.Bag;
            _selectedIndex = 0;
        }

        public BagConfig GetBagConfig()
        {
            return _bagConfig;
        }

        public void SetBagConfig(BagConfig config)
        {
            _bagConfig = config;
        }

        public int GetSelectedIndex()
        {
            return _selectedIndex;
        }

        public void SetSelectedIndex(int index)
        {
            _selectedIndex = index;
        }

        public SerializableBagItem GetSelectedItem()
        {
            if (_bagConfig == null)
            {
                return null;
            }
            var items = _bagConfig.GetItems();
            if (items == null || items.Count == 0)
            {
                return null;
            }
            if (_selectedIndex >= 0 && _selectedIndex < items.Count)
            {
                return items[_selectedIndex];
            }
            return null;
        }
    }
}
