namespace UIModule.Data
{
    public class UIPanelData
    {
        public UIPanelType PanelType { get; set; }
        public bool IsModal { get; set; }
        public bool IsActive { get; set; }
        public int InstanceId { get; set; }

        public UIPanelData(UIPanelType panelType, bool isModal = false)
        {
            PanelType = panelType;
            IsModal = isModal;
            IsActive = true;
            InstanceId = GetHashCode();
        }
    }
}
