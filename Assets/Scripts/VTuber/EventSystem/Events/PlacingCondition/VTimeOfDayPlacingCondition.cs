using System.Collections.Generic;
using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.UI;

namespace VTuber.EventSystem.Events
{
    public class VTimeOfDayPlacingCondition : VPlacingCondition
    {
        public List<int> requiredValue;
        public VTimeOfDayPlacingCondition(CellRange row) : base(row)
        {
            var str = row.Columns[VPlacingConditionHeaderIndex.TargetValue].Value;
            requiredValue = new List<int>();
            foreach (var s in str.Split(','))
            {
                requiredValue.Add(int.Parse(s.Trim()));
            }
        }

        public override bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            return requiredValue.Contains(slot.Coordination.x);
        }
    }
}