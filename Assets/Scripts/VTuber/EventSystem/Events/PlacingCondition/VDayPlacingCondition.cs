using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.UI;

namespace VTuber.EventSystem.Events
{
    public class VDayPlacingCondition : VPlacingCondition
    {
        public int requiredValue;
        public VDayPlacingCondition(CellRange row) : base(row)
        {
            requiredValue = int.Parse(row.Columns[VPlacingConditionHeaderIndex.TargetValue].Value.Trim()) - 1;
        }

        public override bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            return slot.Coordination.x == requiredValue;
        }
    }
}