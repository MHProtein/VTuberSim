using UnityEngine.EventSystems;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleCreatorSlot : VUIBehaviour
    {
        public VEventDataUI Item => _item;
        private VEventDataUI _item;
        
        public void SetItem(VEventDataUI item)
        {
            _item = item;
        }

        public void RemoveItem()
        {
            _item = null;
        }
    }
}