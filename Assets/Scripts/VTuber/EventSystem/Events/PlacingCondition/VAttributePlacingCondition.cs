using Spire.Xls;
using VTuber.Character;
using VTuber.ScheduleSystem.UI;

namespace VTuber.EventSystem.Events
{
    public class VAttributePlacingCondition : VPlacingCondition
    {
        public string attributeName;
        public int requiredValue;


        public VAttributePlacingCondition(CellRange row) : base(row)
        {
            var strs = row.Columns[VPlacingConditionHeaderIndex.TargetValue].Value.Split(',');
            attributeName = strs[0].Trim();
            requiredValue = int.Parse(strs[1].Trim());
        }

        public override bool IsTrue(VCharacter character, VScheduleSlot slot)
        {
            if (character.AttributeManager.TryGetAttribute(attributeName, out var attribute))
                return attribute.Value >= requiredValue;
            return false;
        }
    }
}