using UIModule.Data;

namespace UIModule.Data.Models
{
    public abstract class BasePanelModel
    {
        public UIPanelType PanelType { get; set; }
        public int InstanceId { get; set; }
        public object[] Args { get; set; }

        protected BasePanelModel()
        {
            InstanceId = System.Guid.NewGuid().GetHashCode();
        }
    }
}