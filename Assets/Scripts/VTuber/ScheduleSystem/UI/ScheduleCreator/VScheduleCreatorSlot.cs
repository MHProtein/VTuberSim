using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleCreatorSlot : VUIBehaviour
    {
        public VEventDataUI Item { get; private set; }

        public void SetItem(VEventDataUI item)
        {
            Item = item;
        }

        public void RemoveItem()
        {
            Item = null;
        }
    }
}