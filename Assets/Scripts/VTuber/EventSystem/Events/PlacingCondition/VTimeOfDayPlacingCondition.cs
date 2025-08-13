using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.UI;

namespace VTuber.EventSystem.Events
{
    public class VTimeOfDayPlacingCondition : VPlacingCondition
    {
        public int requiredValue;
        public VTimeOfDayPlacingCondition(CellRange row) : base(row)
        {
            requiredValue = int.Parse(row.Columns[VPlacingConditionHeaderIndex.TargetValue].Value.Trim()) - 1;
        }

        public override bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            return slot.Coordination.y == requiredValue;
        }
    }
}