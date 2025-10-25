using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.UI;

namespace VTuber.EventSystem.Events
{
    public class VDayPlacingCondition : VPlacingCondition
    {
        public List<int> requiredValue;

        public VDayPlacingCondition(CellRange row) : base(row)
        {
            var str = row.Columns[VPlacingConditionHeaderIndex.TargetValue].Value;
            requiredValue = new List<int>();
            foreach (var s in str.Split(',')) requiredValue.Add(int.Parse(s.Trim()));
        }

        public override bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            return requiredValue.Contains(slot.Coordination.x + 1);
        }
    }
}